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
using System.Linq;
using System.Text;
using Zombie;
using Interop.QBFC13;

namespace ZombieDemo
{
  internal class AccountList
  {
    public static void Show()
    {
      Console.WriteLine("Chart of Accounts from the current company file");

      using (var cn = ConnectionMgr.GetConnection())
      {
        var batch = cn.NewBatch();

        var qry = batch.MsgSet.AppendAccountQueryRq();

        //if return elements are not specified, the account query can trigger permission errors
        //because bank and credit card account numbers are included by default
        qry.IncludeRetElementList.Add("Name");
        qry.IncludeRetElementList.Add("AccountType");

        batch.SetClosures(qry, b =>
        {
          var accounts = new QBFCIterator<IAccountRetList, IAccountRet>(b);

          foreach (var account in accounts)
          {
            Console.WriteLine(" Account name:{0}, type {1}", Safe.Value(account.Name), account.AccountType.GetAsString());
          }
        });

        batch.Run();
      }
    }
  }
}