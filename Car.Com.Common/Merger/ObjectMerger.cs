using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Car.Com.Common.Merger
{
  public class ObjectMerger
  {
    private static AssemblyBuilder _assemblyBuilder;
    private static ModuleBuilder _moduleBuilder;
    private static ObjectMergerPolicy _objectMergerPolicy;

    private static readonly IDictionary<String, Type> AnonymousTypes = new Dictionary<String, Type>();
    private static readonly Object Mutex = new Object();


    public static Object MergeObjects(Object obj1, Object obj2)
    {
      // Create a name from the names of both Types
      var name1 = String.Format("{0}_{1}", obj1.GetType(), obj2.GetType());
      var name2 = String.Format("{0}_{1}", obj2.GetType(), obj1.GetType());

      var newValues = CreateInstance(name1, obj1, obj2);
      if (newValues != null)
        return newValues;

      newValues = CreateInstance(name2, obj2, obj1);
      if (newValues != null)
        return newValues;


      lock (Mutex)
      {
        // Now that we're inside the lock - check one more time
        newValues = CreateInstance(name1, obj1, obj2);
        if (newValues != null)
          return newValues;

        // Merge list of PropertyDescriptors for both objects
        var propertyDescriptors = GetProperties(obj1, obj2);

        // Make sure static properties are properly initialized
        InitializeAssembly();

        // Create the type definition
        var newType = CreateType(name1, propertyDescriptors);

        // Add it to the cache
        AnonymousTypes.Add(name1, newType);

        // Return an instance of the new Type
        return CreateInstance(name1, obj1, obj2);
      }
    }

    public static Object MergeObjects(Object obj1, Object obj2, ObjectMergerPolicy policy)
    {
      _objectMergerPolicy = policy;

      return MergeObjects(obj1, obj2);
    }

    /// <summary>
    /// Instantiates an instance of an existing Type from cache
    /// </summary>
    private static Object CreateInstance(String name, Object values1, Object values2)
    {
      Object newValues = null;

      // Merge all values together into an array
      var allValues = MergeValues(values1, values2);

      // Check to see if type exists
      if (AnonymousTypes.ContainsKey(name))
      {
        var type = AnonymousTypes[name];

        if (type != null)
        {
          newValues = Activator.CreateInstance(type, allValues);
        }
        else
        {
          // Remove null type entry
          lock (Mutex)
          {
            AnonymousTypes.Remove(name);
          }
        }
      }

      return newValues;
    }


    /// <summary>
    /// Merge PropertyDescriptors for both objects
    /// </summary>
    private static PropertyDescriptor[] GetProperties(Object obj1, Object obj2)
    {
      // Dynamic list to hold merged list of properties
      var properties = new List<PropertyDescriptor>();
      var obj1PropertyDescriptor = TypeDescriptor.GetProperties(obj1);
      var obj2PropertyDescriptor = TypeDescriptor.GetProperties(obj2);

      // Add properties from object 1
      for (var i = 0; i < obj1PropertyDescriptor.Count; i++)
      {
        if (_objectMergerPolicy == null || !_objectMergerPolicy.Ignored.Contains(obj1PropertyDescriptor[i].GetValue(obj1)))
          properties.Add(obj1PropertyDescriptor[i]);
      }

      // Add properties from object 2
      for (var i = 0; i < obj2PropertyDescriptor.Count; i++)
      {
        if (_objectMergerPolicy == null || !_objectMergerPolicy.Ignored.Contains(obj2PropertyDescriptor[i].GetValue(obj2)))
          properties.Add(obj2PropertyDescriptor[i]);
      }

      return properties.ToArray();
    }


    /// <summary>
    /// Get the type of each property
    /// </summary>
    private static Type[] GetTypes(IEnumerable<PropertyDescriptor> propertyDescriptors)
    {
      return propertyDescriptors.Select(t => t.PropertyType).ToArray();
    }


    /// <summary>
    /// Merge the values of the two types into an object array
    /// </summary>
    private static Object[] MergeValues(Object obj1, Object obj2)
    {
      var obj1PropertyDescriptor = TypeDescriptor.GetProperties(obj1);
      var obj2PropertyDescriptor = TypeDescriptor.GetProperties(obj2);

      var values = new List<Object>();
      for (var i = 0; i < obj1PropertyDescriptor.Count; i++)
      {
        if (_objectMergerPolicy == null || !_objectMergerPolicy.Ignored.Contains(obj1PropertyDescriptor[i].GetValue(obj1)))
          values.Add(obj1PropertyDescriptor[i].GetValue(obj1));
      }

      for (var i = 0; i < obj2PropertyDescriptor.Count; i++)
      {
        if (_objectMergerPolicy == null || !_objectMergerPolicy.Ignored.Contains(obj2PropertyDescriptor[i].GetValue(obj2)))
          values.Add(obj2PropertyDescriptor[i].GetValue(obj2));
      }

      return values.ToArray();
    }


    /// <summary>
    /// Initialize static objects
    /// </summary>
    private static void InitializeAssembly()
    {
      // Check to see if we've already instantiated the static objects
      if (_assemblyBuilder == null)
      {
        // Create a new dynamic assembly
        var assembly = new AssemblyName {Name = "AnonymousTypeExentions"};

        // Get the current application domain
        var domain = Thread.GetDomain();

        // Get a module builder object
        _assemblyBuilder = domain.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run);
        _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyBuilder.GetName().Name, false);
      }
    }


    /// <summary>
    /// Create a new Type definition from the list
    /// of PropertyDescriptors
    /// </summary>
    private static Type CreateType(String name, PropertyDescriptor[] propertyDescriptor)
    {
      // Create TypeBuilder
      var typeBuilder = CreateTypeBuilder(name);

      // Get list of types for ctor definition
      var types = GetTypes(propertyDescriptor);

      // Create private fields for use w/in the ctor body and properties
      var fields = BuildFields(typeBuilder, propertyDescriptor);

      // Define/emit the ctor
      BuildCtor(typeBuilder, fields, types);

      // Define/emit the properties
      BuildProperties(typeBuilder, fields);

      // Return Type definition
      return typeBuilder.CreateType();
    }


    /// <summary>
    /// Create a type builder with the specified name
    /// </summary>
    private static TypeBuilder CreateTypeBuilder(string typeName)
    {
      var typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Public, typeof (object));

      return typeBuilder;
    }


    /// <summary>
    /// Define/emit the ctor and ctor body
    /// </summary>
    private static void BuildCtor(TypeBuilder typeBuilder, FieldBuilder[] fields, Type[] types)
    {
      // Define ctor()
      var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, types);

      // Build ctor()
      var ctorGen = ctor.GetILGenerator();

      // Create ctor that will assign to private fields
      for (var i = 0; i < fields.Length; i++)
      {
        // Load argument (parameter)
        ctorGen.Emit(OpCodes.Ldarg_0);
        ctorGen.Emit(OpCodes.Ldarg, (i + 1));

        // Store argument in field
        ctorGen.Emit(OpCodes.Stfld, fields[i]);
      }

      // Return from ctor()
      ctorGen.Emit(OpCodes.Ret);
    }


    /// <summary>
    /// Define fields based on the list of PropertyDescriptors
    /// </summary>
    private static FieldBuilder[] BuildFields(TypeBuilder typeBuilder, IList<PropertyDescriptor> propertyDescriptors)
    {
      var fields = new List<FieldBuilder>();

      // Build/define fields
      for (var i = 0; i < propertyDescriptors.Count; i++)
      {
        var pd = propertyDescriptors[i];

        // Define field as '_[Name]' with the object's Type
        var field = typeBuilder.DefineField(String.Format("_{0}", pd.Name), pd.PropertyType, FieldAttributes.Private);

        // Add to list of FieldBuilder objects
        fields.Add(field);
      }

      return fields.ToArray();
    }


    /// <summary>
    /// Build a list of Properties to match the list of private fields
    /// </summary>
    private static void BuildProperties(TypeBuilder typeBuilder, IList<FieldBuilder> fields)
    {
      // Build properties
      for (var i = 0; i < fields.Count; i++)
      {
        // Remove '_' from name for public property name
        var propertyName = fields[i].Name.Substring(1);

        // Define the property
        var property = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, fields[i].FieldType, null);

        //define 'Get' method only (anonymous types are read-only)
        var getMethod = typeBuilder.DefineMethod(String.Format("Get_{0}", propertyName), MethodAttributes.Public,
          fields[i].FieldType, Type.EmptyTypes);

        // Build 'Get' method
        var methGen = getMethod.GetILGenerator();

        // Method body
        methGen.Emit(OpCodes.Ldarg_0);
        // Load value of corresponding field
        methGen.Emit(OpCodes.Ldfld, fields[i]);
        // Return from 'Get' method
        methGen.Emit(OpCodes.Ret);

        // Assign method to property 'Get'
        property.SetGetMethod(getMethod);
      }
    }

    public static ObjectMergerPolicy Ignore(object value)
    {
      return new ObjectMergerPolicy(value);
    }
  }
}
