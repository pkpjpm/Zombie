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
  /// A simple collection that reads custom field names.
  /// </summary>
  public class CustomFieldGroup
  {
    private QBFCIterator<IDataExtRetList, IDataExtRet> _fields;
    private bool _requireFieldMatch;

    /// <summary>
    /// Wraps a custom field result
    /// </summary>
    /// <param name="dataExtensions">The field list to wrap</param>
    /// <param name="requireFieldMatch">if true, name mismatch will throw an exception</param>
    public CustomFieldGroup(IDataExtRetList dataExtensions, bool requireFieldMatch)
    {
      _fields = new QBFCIterator<IDataExtRetList, IDataExtRet>(dataExtensions);
      _requireFieldMatch = requireFieldMatch;
    }

    /// <summary>
    /// This indexer pulls custom fields by name.
    /// </summary>
    /// <param name="fieldName">The case insensitive name of the custom field</param>
    /// <returns>The field value if found or string.Empty if the field is not found
    /// and an exact match is not required</returns>
    public string this[string fieldName]
    {
      get
      {
        string targetName = fieldName.ToLower();

        foreach (var field in _fields)
        {
          if (Safe.Value(field.DataExtName).ToLower() == targetName)
          {
            return Safe.Value(field.DataExtValue);
          }
        }

        if (_requireFieldMatch)
        {
          throw new Exception(
              string.Format("A QuickBooks custom field with the name {0} was not found", fieldName));
        }

        return string.Empty;
      }
    }
  }
}