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
using System.Collections.Generic;
using Interop.QBFC13;

namespace Zombie
{
  /// <summary>
  /// This generic class simplifies and standardizes iteration syntax
  /// for QBFC lists.  Using this class, one can use the foreach keyword
  /// to iterate across all items in a list.
  /// </summary>
  /// <typeparam name="L">The type of the list, for example IBillRetList</typeparam>
  /// <typeparam name="D">The type of the item, for example IBillRet</typeparam>
  public class QBFCIterator<L, D> : IEnumerable<D> where L : class, IQBBase
  {
    private L m_List;

    /// <summary>
    /// This constructor can be used for response list items or for sub-lists that are properties
    /// on other QBFC objects.
    /// </summary>
    /// <param name="lst">The sub-list</param>
    public QBFCIterator(IQBBase lst)
    {
      m_List = lst as L;

      if (m_List == null && lst != null)
      {
        throw new Exception("iterator type mismatch: check your template parameters");
      }
    }

    public bool IsEmpty
    {
      get
      {
        if (m_List == null)
        {
          return true;
        }
        else
        {
          return Count == 0;
        }
      }
    }

    /// <summary>
    /// An efficient alternative to the Count() function
    /// </summary>
    public int EntityCount
    {
      get { return Count; }
    }

    public D GetFirstItem()
    {
      if (IsEmpty)
      {
        throw new Exception("Cannot retrieve item from empty list");
      }
      else
      {
        return GetAt(0);
      }
    }

    #region Late-bound properties

    //
    // Since .NET requires that all methods invoked on a parameterized type
    // must compile based solely on interface constraints, we must use late
    // binding to access the count property and GetAt methods.  This may have
    // an impact on performance and could conceivably cause run time errors
    // with incorrect type parameters.
    //
    private int Count
    {
      get
      {
        if (m_List == null)
        {
          return 0;
        }
        else
        {
          Type t = m_List.GetType();

          return (int)t.InvokeMember("Count",
              System.Reflection.BindingFlags.GetProperty, null, m_List, null);
        }
      }
    }

    private D GetAt(int idx)
    {
      Type t = m_List.GetType();

      return (D)t.InvokeMember("GetAt",
          System.Reflection.BindingFlags.InvokeMethod, null, m_List, new Object[] { idx });
    }

    #endregion Late-bound properties

    #region IEnumerable<D> Members

    public IEnumerator<D> GetEnumerator()
    {
      if (m_List != null)
      {
        for (int idx = 0; idx < Count; idx++)
        {
          yield return GetAt(idx);
        }
      }
    }

    #endregion IEnumerable<D> Members

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      if (m_List != null)
      {
        for (int idx = 0; idx < Count; idx++)
        {
          yield return GetAt(idx);
        }
      }
    }

    #endregion IEnumerable Members
  }
}