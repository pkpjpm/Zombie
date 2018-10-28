/*
 *               ZOMBIE utility library for QBFC
 *
 *             created by Paul Keister (pk@pjpm.biz)
 *                copyright (c) 2003 - 2012 PJPM
 *
 *  Licensed under the Eclipse Public License 1.0 (EPL-1.0)
 *  full license available at http://opensource.org/licenses/EPL-1.0
 */

using System;
using Interop.QBFC13;

namespace Zombie
{
  /// <summary>
  /// The Safe class provide static methods to read items from a QBFC object.
  /// </summary>
  public static class Safe
  {
    /// <summary>
    /// Returns a valid non-exploding string from an IQBStringType object
    /// </summary>
    /// <param name="obj">The object to be translated</param>
    /// <returns>a translated string</returns>
    public static string Value(IQBStringType obj)
    {
      if (obj == null)
      {
        return string.Empty;
      }
      else
      {
        return obj.GetValue();
      }
    }

    public static bool Value(IQBBoolType bl)
    {
      if (bl == null)
      {
        return false;
      }
      else
      {
        return bl.GetValue();
      }
    }

    public static decimal Value(IQBAmountType amt)
    {
      if (amt == null)
      {
        return 0;
      }
      else
      {
        return (decimal)amt.GetValue();
      }
    }

    public static string Value(IQBIDType ID)
    {
      if (ID == null)
      {
        return string.Empty;
      }
      else
      {
        return ID.GetValue();
      }
    }

    public static int Value(IQBIntType i)
    {
      if (i == null)
      {
        return 0;
      }
      else
      {
        return i.GetValue();
      }
    }

    public static decimal Value(IQBPercentType pct)
    {
      if (pct == null)
      {
        return 0M;
      }
      else
      {
        return (decimal)pct.GetValue();
      }
    }

    /// <summary>
    /// The beginning of the COM Epoch is used to represent no date
    /// </summary>
    /// <returns>midnight on Dec 30, 1899</returns>
    public static DateTime GetNullDateTimeValue()
    {
      //this date represents no date
      return new DateTime(1899, 12, 30);
    }

    public static DateTime Value(IQBDateType dt)
    {
      if (dt == null)
      {
        return GetNullDateTimeValue();
      }
      else
      {
        return dt.GetValue();
      }
    }

    public static DateTime Value(IQBDateTimeType dt)
    {
      if (dt == null)
      {
        return GetNullDateTimeValue();
      }
      else
      {
        return dt.GetValue();
      }
    }

    public static decimal Value(IQBPriceType prc)
    {
      if (prc == null)
      {
        return 0M;
      }
      else
      {
        return (decimal)prc.GetValue();
      }
    }

    public static decimal Value(IQBQuanType quan)
    {
      if (quan == null)
      {
        return 0M;
      }
      else
      {
        return (decimal)quan.GetValue();
      }
    }

    public static decimal Value(IQBFloatType val)
    {
      if (val == null)
      {
        return 0M;
      }
      else
      {
        return (decimal)val.GetValue();
      }
    }

    /// <summary>
    /// Returns a name reference (Full Name) in a way that will not
    /// explode.  Returns a zero length string in the event of failure
    /// </summary>
    /// <param name="obj">the reference to read</param>
    /// <returns></returns>
    public static string FullName(IQBBaseRef obj)
    {
      if (obj == null)
      {
        return string.Empty;
      }
      if (obj.FullName == null)
      {
        return string.Empty;
      }
      return obj.FullName.GetValue();
    }

    /// <summary>
    /// Returns an ID reference (ListID) in a way that will not
    /// explode.  Returns a zero length string in the event of failure
    /// </summary>
    /// <param name="obj">The reference to read</param>
    /// <returns></returns>
    public static string ListID(IQBBaseRef obj)
    {
      if (obj == null)
      {
        return string.Empty;
      }
      if (obj.ListID == null)
      {
        return string.Empty;
      }
      return obj.ListID.GetValue();
    }

    public static string SafeString(object obj)
    {
      if (obj == null)
      {
        return string.Empty;
      }
      else
      {
        return obj.ToString();
      }
    }

    public static double DecimalAsCents(decimal d)
    {
      return (double)Math.Round(d, 2);
    }

    public static double Cents(float? f)
    {
      if (!f.HasValue) return 0;

      return Math.Round(f.Value, 2);
    }

    public static double DecimalAsMils(decimal d)
    {
      return (double)Math.Round(d, 4);
    }

    public static string CleanApostrophes(string s)
    {
      s = s.Replace('‘', '\'');
      return s.Replace('’', '\'');
    }

    /// <summary>
    /// SDK updates treat truncation as an error.  So it is important to trim
    /// values that are found to be too long
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string LimitedString(string val, int maxLength, string reference)
    {
      if (val == null) return string.Empty;

      if (val.Length > maxLength)
      {
        var entry = new StatusEntry()
        {
          TypeOfEntry = StatusEntry.EntryType.Warning,
          Summary = "Data truncated",
          Details = reference + " exceeds maximum length(" + maxLength
                + "), value will be truncated"
        };

        StatusMgr.LogEntry(entry);

        return val.Substring(0, maxLength);
      }

      return val;
    }

    /// <summary>
    /// Silent truncation function when data loss is acceptable
    /// </summary>
    /// <param name="val">Original string value</param>
    /// <param name="maxLength">maximum allowed lenght</param>
    /// <returns>a safe string to use in an SDK string operation</returns>
    public static string LimitedString(string val, int maxLength)
    {
      if (val == null) return string.Empty;

      if (val.Length > maxLength)
      {
        return val.Substring(0, maxLength);
      }

      return val;
    }
  }
}