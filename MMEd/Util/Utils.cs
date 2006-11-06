using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MMEd.Util
{
  abstract class Utils
  {

    /// <summary>
    ///  Converts a 16 bit PS colour into a 32 bit ARGB Color
    ///  structure
    /// 
    ///  Assumes that the top bit is a transparency flag, but
    ///  I don't yet have good evidence for this.
    /// </summary>
    public static int PS16bitColorToARGB(short xiVal)
    {
      int alpha = ((xiVal & 0x8000) != 0)
       ? 0 //transparent
       : unchecked((int)0xff000000); //solid

      //  {0}{bbbbb}gg} {ggg{rrrrr}  
      int r = ((xiVal << 3) & 0xf8);
      int g = ((xiVal >> 2) & 0xf8);
      int b = ((xiVal >> 7) & 0xf8);

      return ((r << 16) | (g << 8) | b) | alpha;
    }

    public static Color PSRGBColorToColor(int xiVal)
    {
      //ABGR => XRGB
      return Color.FromArgb((xiVal & unchecked((int)0xff000000)) | ((xiVal >> 16) & 0xff) | (xiVal & 0xff00) | ((xiVal << 16) & 0xff0000));
    }

    public static int ColorToPSRGBColor(Color xiVal)
    {
      //XRGB => ABGR
      return (int)((xiVal.A << 24) | (xiVal.B << 16) | (xiVal.G << 8) | (xiVal.R));
    }

    /// <summary>
    ///  Converts a 32 bit ARGB Color into a 16 bit PS colour
    ///  structure
    /// </summary>
    public static short ARGBColorToPS16bit(int xiVal)
    {
      int alpha;

      if ((xiVal & 0xff000000) == 0xff000000)
      {
        alpha = 0;
      }
      else if ((xiVal & 0xff000000) == 0x0)
      {
        alpha = 0x8000;
      }
      else
      {
        throw new ArgumentException("Can't do partially opaque colors");
      }

      int r = ((xiVal >> 19) & 0x1f);
      int g = ((xiVal >> 11) & 0x1f);
      int b = ((xiVal >> 3) & 0x1f);

      //  {0}{bbbbb}gg} {ggg{rrrrr}  
      return (short)(r | (g << 5) | (b << 10) | alpha);
    }

    /// <summary>
    ///  I can't believe I have to write this.
    ///  There _must_ be a library function to do this...
    /// 
    ///  Adds slashes to a string to make it a valid C# string
    ///  literal.
    /// </summary>
    public static string EscapeString(string s)
    {
      int lFirstBadChar = -1;
      for (int i = 0; i < s.Length; i++)
      {
        char c = s[i];
        if ((c == '\\' || c < '!' || c > '~') && c != ' ')
        {
          lFirstBadChar = i;
          break;
        }
      }
      if (lFirstBadChar == -1) return s;
      StringBuilder acc = new StringBuilder(s.Length + 1);
      if (lFirstBadChar > 0)
      {
        acc.Append(s, 0, lFirstBadChar);
      }
      for (int i = lFirstBadChar; i < s.Length; i++)
      {
        char c = s[i];
        if ((c == '\\' || c < '!' || c > '~') && c != ' ')
        {
          switch (c)
          {
            case '\\': acc.Append("\\\\"); break;
            case '\r': acc.Append("\\r"); break;
            case '\n': acc.Append("\\n"); break;
            case '\t': acc.Append("\\t"); break;
            case '\0': acc.Append("\\0"); break;
            default:
              acc.AppendFormat("\\u{0:x4}", (int)c);
              break;
          }
        }
        else
        {
          acc.Append(c);
        }
      }
      return acc.ToString();
    }

    //counts how many objects in the given collection are of the
    //given type. Inefficient, but sometimes convenient;
    public static int CountInstances(System.Collections.IEnumerable xiCollection, Type xiType)
    {
      int acc = 0;
      foreach (object o in xiCollection)
      {
        if (xiType.IsInstanceOfType(o))
        {
          acc++;
        }
      }
      return acc;
    }

    public static bool ArrayCompare(Array xiArray1, Array xiArray2)
    {
      if (xiArray1.Length != xiArray2.Length)
      {
        return false;
      }
      for (int i = 0; i < xiArray1.Length; i++)
      {
        object lObj1 = xiArray1.GetValue(i);
        object lObj2 = xiArray2.GetValue(i);
        if (lObj1 is Array && lObj2 is Array)
        {
          if (!ArrayCompare((Array)lObj1, (Array)lObj2))
          {
            return false;
          }
        }
        else if (!lObj1.Equals(lObj2))
        {
          return false;
        }
      }
      return true;
    }

    public static string CamelCaseToSentence(string xiVal)
    {
      StringBuilder lAcc = new StringBuilder();
      bool lFirstLetter = true;
      foreach (char c in xiVal.ToCharArray())
      {
        if (char.IsUpper(c))
        {
          if (lFirstLetter)
          {
            lAcc.Append(c);
          }
          else
          {
            lAcc.Append(' ');
            lAcc.Append(char.ToLower(c));
          }
        }
        else
        {
          lAcc.Append(c);
        }
        lFirstLetter = false;
      }
      return lAcc.ToString();
    }
  }
}
