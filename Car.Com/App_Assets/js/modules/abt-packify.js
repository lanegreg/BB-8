/**
 *	Autobytel (c)2015
 *	Module:   Micro Object Packer / Unpacker
 *  Purpose:  To provide a way to shrink (pack) JSON objects through shortening the property names.
 *            This is accomplished by providing a pack-template.
 *
 *	@version: 0.0.1 
 *	@last: 01/01/2015
 *	@author: Greg Lane
 */


var packify = function() {
  var pname = '@pname'
    , $ = jQuery
    , isPlainObject = $.isPlainObject

  //#region - Private funcs

  var invertMapTemplate = function (template) {
    var templObj = {}

    Object.keys(template).forEach(function (key) {
      if (key === pname)
        return

      var invertKey = template[key]
        , invertVal = key

      if (isPlainObject(invertKey)) {
        var obj = invertKey
          , pvalue = obj[pname]

        invertVal = invertMapTemplate(invertKey)
        invertVal[pname] = key
        invertKey = pvalue
      }

      templObj[invertKey] = invertVal
    })

    return templObj
  }

  var mapProperties = function (targetObj, template, sourceObj) {
    Object.keys(sourceObj).forEach(function (key) {
      var srcPropValue = sourceObj[key]
        , packKeyName = template[key]

      if (isPlainObject(srcPropValue) && isPlainObject(packKeyName)) {
        var trgObj = {}
          , srcObj = srcPropValue
          , tmplObj = packKeyName

        packKeyName = tmplObj[pname]
        mapProperties(trgObj, tmplObj, srcObj)
        srcPropValue = trgObj
      }

      targetObj[packKeyName] = srcPropValue
    })
  }

  //#endregion


  return {
    pack: function(obj, templ) {
      var packed = {}

      mapProperties(packed, templ, obj)

      return packed
    },

    unpack: function (obj, templ) {
      var packed = {}
        , invTempl = invertMapTemplate(templ)

      mapProperties(packed, invTempl, obj)

      return packed
    }
  }
}()


module.exports = packify
