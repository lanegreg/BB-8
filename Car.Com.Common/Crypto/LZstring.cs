using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Car.Com.Common.Crypto
{
  public class LZstring
  {
    private class ContextCompress
    {
      public Dictionary<string, int> Dictionary { get; set; }
      public Dictionary<string, bool> DictionaryToCreate { get; set; }
      public string C { get; set; }
      public string Wc { get; set; }
      public string W { get; set; }
      public int EnlargeIn { get; set; }
      public int DictSize { get; set; }
      public int NumBits { get; set; }
      public ContextCompressData Data { get; set; }
    }


    private class ContextCompressData
    {
      public string Str { get; set; }
      public int Val { get; set; }
      public int Position { get; set; }
    }


    private class DecompressData
    {
      public string Str { get; set; }
      public int Val { get; set; }
      public int Position { get; set; }
      public int Index { get; set; }
    }


    private static ContextCompressData WriteBit(int value, ContextCompressData data)
    {
      data.Val = (data.Val << 1) | value;

      if (data.Position == 15)
      {
        data.Position = 0;
        data.Str += (char) data.Val;
        data.Val = 0;
      }
      else
        data.Position++;

      return data;
    }


    private static ContextCompressData WriteBits(int numbits, int value, ContextCompressData data)
    {
      for (var i = 0; i < numbits; i++)
      {
        data = WriteBit(value & 1, data);
        value = value >> 1;
      }

      return data;
    }


    private static ContextCompress ProduceW(ContextCompress context)
    {
      if (context.DictionaryToCreate.ContainsKey(context.W))
      {
        if (context.W[0] < 256)
        {
          context.Data = WriteBits(context.NumBits, 0, context.Data);
          context.Data = WriteBits(8, context.W[0], context.Data);
        }
        else
        {
          context.Data = WriteBits(context.NumBits, 1, context.Data);
          context.Data = WriteBits(16, context.W[0], context.Data);
        }

        context = DecrementEnlargeIn(context);
        context.DictionaryToCreate.Remove(context.W);
      }
      else
      {
        context.Data = WriteBits(context.NumBits, context.Dictionary[context.W], context.Data);
      }

      return context;
    }


    private static ContextCompress DecrementEnlargeIn(ContextCompress context)
    {
      context.EnlargeIn--;

      if (context.EnlargeIn != 0)
        return context;

      context.EnlargeIn = (int) Math.Pow(2, context.NumBits);
      context.NumBits++;

      return context;
    }


    public static string Compress(string uncompressed)
    {
      var context = new ContextCompress();
      var data = new ContextCompressData();

      context.Dictionary = new Dictionary<string, int>();
      context.DictionaryToCreate = new Dictionary<string, bool>();
      context.C = String.Empty;
      context.Wc = String.Empty;
      context.W = String.Empty;
      context.EnlargeIn = 2;
      context.DictSize = 3;
      context.NumBits = 2;
      data.Str = String.Empty;
      data.Val = 0;
      data.Position = 0;
      context.Data = data;

      try
      {
        for (var i = 0; i < uncompressed.Length; i++)
        {
          context.C = uncompressed[i].ToString();

          if (!context.Dictionary.ContainsKey(context.C))
          {
            context.Dictionary[context.C] = context.DictSize++;
            context.DictionaryToCreate[context.C] = true;
          }

          context.Wc = context.W + context.C;

          if (context.Dictionary.ContainsKey(context.Wc))
          {
            context.W = context.Wc;
          }
          else
          {
            context = ProduceW(context);
            context = DecrementEnlargeIn(context);
            context.Dictionary[context.Wc] = context.DictSize++;
            context.W = context.C;
          }
        }

        if (context.W != "")
          context = ProduceW(context);


        // Mark the end of the stream
        context.Data = WriteBits(context.NumBits, 2, context.Data);

        // Flush the last char
        while (true)
        {
          context.Data.Val = (context.Data.Val << 1);

          if (context.Data.Position == 15)
          {
            context.Data.Str += (char) context.Data.Val;
            break;
          }

          context.Data.Position++;
        }
      }
      catch (Exception ex)
      {
        return ex.Message;
      }

      return context.Data.Str;
    }


    private static int ReadBit(DecompressData data)
    {
      var res = data.Val & data.Position;

      data.Position >>= 1;

      if (data.Position == 0)
      {
        data.Position = 32768;

        // This 'if' check doesn't appear in the orginal lz-string javascript code.
        // Added as a check to make sure we don't exceed the length of data.str
        // The javascript charCodeAt will return a NaN if it exceeds the index but will not error out
        if (data.Index < data.Str.Length)
          data.Val = data.Str[data.Index++];
      }

      return res > 0 ? 1 : 0;
    }


    private static int ReadBits(int numBits, DecompressData data)
    {
      var res = 0;
      var maxpower = (int) Math.Pow(2, numBits);
      var power = 1;

      while (power != maxpower)
      {
        res |= ReadBit(data)*power;
        power <<= 1;
      }

      return res;
    }


    public static string Decompress(string compressed)
    {
      var data = new DecompressData();
      var dictionary = new List<string>();
      var enlargeIn = 4;
      var numBits = 3;
      dynamic c = "";
      var errorCount = 0;

      data.Str = compressed;
      data.Val = compressed[0];
      data.Position = 32768;
      data.Index = 1;

      try
      {
        for (var i = 0; i < 3; i++)
        {
          dictionary.Add(i.ToString());
        }

        var next = ReadBits(2, data);

        switch (next)
        {
          case 0:
            c = Convert.ToChar(ReadBits(8, data)).ToString();
            break;

          case 1:
            c = Convert.ToChar(ReadBits(16, data)).ToString();
            break;

          case 2:
            return "";
        }

        dictionary.Add(c);
        string result;
        dynamic w = result = c;

        while (true)
        {
          c = ReadBits(numBits, data);
          var cc = (int) (c);

          switch (cc)
          {
            case 0:
            {
              if (errorCount++ > 10000)
                throw new Exception("To many errors");

              c = Convert.ToChar(ReadBits(8, data)).ToString();
              dictionary.Add(c);
              c = dictionary.Count - 1;
              enlargeIn--;

              break;
            }

            case 1:
            {
              c = Convert.ToChar(ReadBits(16, data)).ToString();
              dictionary.Add(c);
              c = dictionary.Count - 1;
              enlargeIn--;

              break;
            }

            case 2:
              return result;
          }

          if (enlargeIn == 0)
          {
            enlargeIn = (int) Math.Pow(2, numBits);
            numBits++;
          }


          string entry;
          if (dictionary.ElementAtOrDefault((int) c) != null)
          {
            entry = dictionary[c];
          }
          else
          {
            if (c == dictionary.Count)
              entry = w + w[0];
            else
              return null;
          }

          result += entry;
          dictionary.Add(w + entry[0]);
          enlargeIn--;
          w = entry;

          if (enlargeIn == 0)
          {
            enlargeIn = (int) Math.Pow(2, numBits);
            numBits++;
          }
        }
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }


    public static string CompressToUtf16(string input)
    {
      var output = String.Empty;
      var status = 0;
      var current = 0;

      try
      {
        if (input == null)
          throw new Exception("Input is Null");

        input = Compress(input);
        if (input.Length == 0)
          return input;

        foreach (var t in input)
        {
          var c = (int) t;

          switch (status++)
          {
            case 0:
            {
              output += (char) ((c >> 1) + 32);
              current = (c & 1) << 14;
              break;
            }

            case 1:
            {
              output += (char) ((current + (c >> 2)) + 32);
              current = (c & 3) << 13;
              break;
            }

            case 2:
            {
              output += (char) ((current + (c >> 3)) + 32);
              current = (c & 7) << 12;
              break;
            }

            case 3:
            {
              output += (char) ((current + (c >> 4)) + 32);
              current = (c & 15) << 11;
              break;
            }

            case 4:
            {
              output += (char) ((current + (c >> 5)) + 32);
              current = (c & 31) << 10;
              break;
            }

            case 5:
            {
              output += (char) ((current + (c >> 6)) + 32);
              current = (c & 63) << 9;
              break;
            }

            case 6:
            {
              output += (char) ((current + (c >> 7)) + 32);
              current = (c & 127) << 8;
              break;
            }

            case 7:
            {
              output += (char) ((current + (c >> 8)) + 32);
              current = (c & 255) << 7;
              break;
            }

            case 8:
            {
              output += (char) ((current + (c >> 9)) + 32);
              current = (c & 511) << 6;
              break;
            }

            case 9:
            {
              output += (char) ((current + (c >> 10)) + 32);
              current = (c & 1023) << 5;
              break;
            }

            case 10:
            {
              output += (char) ((current + (c >> 11)) + 32);
              current = (c & 2047) << 4;
              break;
            }

            case 11:
            {
              output += (char) ((current + (c >> 12)) + 32);
              current = (c & 4095) << 3;
              break;
            }

            case 12:
            {
              output += (char) ((current + (c >> 13)) + 32);
              current = (c & 8191) << 2;
              break;
            }

            case 13:
            {
              output += (char) ((current + (c >> 14)) + 32);
              current = (c & 16383) << 1;
              break;
            }

            case 14:
            {
              output += (char) ((current + (c >> 15)) + 32);
              output += (char) ((c & 32767) + 32);
              status = 0;
              break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        return ex.Message;
      }

      return output + (char) (current + 32);
    }


    public static string DecompressFromUtf16(string input)
    {
      var output = String.Empty;
      var status = 0;
      var current = 0;
      var i = 0;

      try
      {
        if (input == null)
          throw new Exception("input is Null");

        while (i < input.Length)
        {
          var c = input[i] - 32;

          switch (status++)
          {
            case 0:
              current = c << 1;
              break;
            case 1:
              output += (char) (current | (c >> 14));
              current = (c & 16383) << 2;
              break;
            case 2:
              output += (char) (current | (c >> 13));
              current = (c & 8191) << 3;
              break;
            case 3:
              output += (char) (current | (c >> 12));
              current = (c & 4095) << 4;
              break;
            case 4:
              output += (char) (current | (c >> 11));
              current = (c & 2047) << 5;
              break;
            case 5:
              output += (char) (current | (c >> 10));
              current = (c & 1023) << 6;
              break;
            case 6:
              output += (char) (current | (c >> 9));
              current = (c & 511) << 7;
              break;
            case 7:
              output += (char) (current | (c >> 8));
              current = (c & 255) << 8;
              break;
            case 8:
              output += (char) (current | (c >> 7));
              current = (c & 127) << 9;
              break;
            case 9:
              output += (char) (current | (c >> 6));
              current = (c & 63) << 10;
              break;
            case 10:
              output += (char) (current | (c >> 5));
              current = (c & 31) << 11;
              break;
            case 11:
              output += (char) (current | (c >> 4));
              current = (c & 15) << 12;
              break;
            case 12:
              output += (char) (current | (c >> 3));
              current = (c & 7) << 13;
              break;
            case 13:
              output += (char) (current | (c >> 2));
              current = (c & 3) << 14;
              break;
            case 14:
              output += (char) (current | (c >> 1));
              current = (c & 1) << 15;
              break;
            case 15:
              output += (char) (current | c);
              status = 0;
              break;
          }

          i++;
        }
      }
      catch (Exception ex)
      {
        return ex.Message;
      }

      return Decompress(output);
    }


    public static string CompressToBase64(string input)
    {
      const string keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
      var output = String.Empty;

      // Using the data type 'double' for these so that the .Net double.NaN & double.IsNaN functions can be used
      // later in the function.  .Net doesn't have a similar function for regular integers.

      var enc2 = 0;
      var enc3 = 0;
      var enc4 = 0;
      var i = 0;

      try
      {
        if (input == null)
          throw new Exception("input is Null");

        input = Compress(input);

        while (i < input.Length*2)
        {
          double chr1;
          double chr2;
          double chr3;

          if (i%2 == 0)
          {
            chr1 = input[i/2] >> 8;
            chr2 = input[i/2] & 255;

            if (i/2 + 1 < input.Length)
              chr3 = input[i/2 + 1] >> 8;
            else
              chr3 = Double.NaN;
          }
          else
          {
            chr1 = input[(i - 1)/2] & 255;
            if ((i + 1)/2 < input.Length)
            {
              chr2 = input[(i + 1)/2] >> 8;
              chr3 = input[(i + 1)/2] & 255;
            }
            else
            {
              chr2 = chr3 = Double.NaN;
            }
          }

          i += 3;


          var enc1 = (int) (Math.Round(chr1)) >> 2;

          // The next three 'if' statements are there to make sure we are not trying to calculate a value that has been 
          // assigned to 'double.NaN' above.  The orginal Javascript functions didn't need these checks due to how
          // Javascript functions.
          // Also, due to the fact that some of the variables are of the data type 'double', we have to do some type 
          // conversion to get the 'enc' variables to be the correct value.
          if (!Double.IsNaN(chr2))
          {
            enc2 = (((int) (Math.Round(chr1)) & 3) << 4) | ((int) (Math.Round(chr2)) >> 4);
          }

          if (!Double.IsNaN(chr2) && !Double.IsNaN(chr3))
          {
            enc3 = (((int) (Math.Round(chr2)) & 15) << 2) | ((int) (Math.Round(chr3)) >> 6);
          }

          if (!Double.IsNaN(chr3))
          {
            enc4 = (int) (Math.Round(chr3)) & 63;
          }

          if (Double.IsNaN(chr2))
          {
            enc3 = enc4 = 64;
          }
          else if (Double.IsNaN(chr3))
          {
            enc4 = 64;
          }

          output = output + keyStr[enc1] + keyStr[enc2] + keyStr[enc3] + keyStr[enc4];
        }
      }
      catch (Exception ex)
      {
        return ex.Message;
      }

      return output;
    }


    public static string DecompressFromBase64(string input)
    {
      const string keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

      var outputStr = String.Empty;
      var output = 0;
      var ol = 0;
      var i = 0;

      try
      {
        if (input == null)
          throw new Exception("input is Null");

        var regex = new Regex(@"[^A-Za-z0-9-\+\/\=]");
        input = regex.Replace(input, "");

        while (i < input.Length)
        {
          var enc1 = keyStr.IndexOf(input[i++]);
          var enc2 = keyStr.IndexOf(input[i++]);
          var enc3 = keyStr.IndexOf(input[i++]);
          var enc4 = keyStr.IndexOf(input[i++]);

          var chr1 = (enc1 << 2) | (enc2 >> 4);
          var chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
          var chr3 = ((enc3 & 3) << 6) | enc4;

          if (ol%2 == 0)
          {
            output = chr1 << 8;

            if (enc3 != 64)
            {
              outputStr += (char) (output | chr2);
            }

            if (enc4 != 64)
            {
              output = chr3 << 8;
            }
          }
          else
          {
            outputStr = outputStr + (char) (output | chr1);

            if (enc3 != 64)
            {
              output = chr2 << 8;
            }
            if (enc4 != 64)
            {
              outputStr += (char) (output | chr3);
            }
          }
          ol += 3;
        }

        // Send the output out to the main decompress function
        outputStr = Decompress(outputStr);
      }
      catch (Exception ex)
      {
        return ex.Message;
      }

      return outputStr;
    }
  }
}