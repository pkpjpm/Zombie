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
  /// The QBFCScalar class is based on the QBFCIterator class. This class should be used
  /// in cases where 0 or 1 items is expected in an SDK query result.
  /// </summary>
  /// <typeparam name="L">The type of the list, for example IBillRetList</typeparam>
  /// <typeparam name="D">The type of the item, for example IBillRet</typeparam>
  public class QBFCScalar<L, D> where L : class, IQBBase
  {
    private L m_List;

    public QBFCScalar(IQBBase lst)
    {
      m_List = lst as L;

      if (m_List == null && lst != null)
      {
        throw new Exception("scalar type mismatch");
      }

      if (m_List != null)
      {
        Type t = m_List.GetType();

        int count = (int)t.InvokeMember("Count",
            System.Reflection.BindingFlags.GetProperty, null, m_List, null);

        if (count > 1)
        {
          throw new Exception("More than one entity found for scalar query");
        }
      }
    }

    public bool ItemFound
    {
      get
      {
        return m_List != null;
      }
    }

    public D Item
    {
      get
      {
        Type t = m_List.GetType();

        return (D)t.InvokeMember("GetAt",
            System.Reflection.BindingFlags.InvokeMethod, null, m_List, new Object[] { 0 });
      }
    }
  }
}