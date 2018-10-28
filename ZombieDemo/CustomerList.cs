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
using Zombie;

namespace ZombieDemo
{
  internal class CustomerList
  {
    /// <summary>
    /// Output a simple list of all customers in the company file
    /// </summary>
    public static void Show()
    {
      using (var cn = ConnectionMgr.GetConnection())
      {
        Console.WriteLine("Customer list from the current company file");

        var batch = cn.NewBatch();

        var qryCust = batch.MsgSet.AppendCustomerQueryRq();

        batch.SetClosures(qryCust, b =>
        {
          var customers = new QBFCIterator<ICustomerRetList, ICustomerRet>(b);

          foreach (var customer in customers)
          {
            Console.WriteLine(Safe.Value(customer.FullName));
          }
        });

        batch.Run();
      }
    }
  }
}