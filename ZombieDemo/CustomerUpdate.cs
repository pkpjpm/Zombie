using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zombie;
using Interop.QBFC13;

namespace ZombieDemo
{
  internal class CustomerUpdate
  {
    public static void Run()
    {
      using (var cn = ConnectionMgr.GetConnection())
      {
        Console.WriteLine("WARNING: this demonstration will change the name of every customer in your company file. Are you sure you want to do this? (y/n)");

        switch (Console.ReadKey().KeyChar)
        {
          case 'y':
          case 'Y':
            break;

          default:
            return;
        }

        var queryBatch = cn.NewBatch();

        var updateBatch = cn.NewBatch();

        var qry = queryBatch.MsgSet.AppendCustomerQueryRq();

        //specify returned values to limit the size of the XML
        qry.IncludeRetElementList.Add("ListID");
        qry.IncludeRetElementList.Add("EditSequence");
        qry.IncludeRetElementList.Add("CompanyName");

        queryBatch.SetClosures(qry, b =>
            {
              var customers = new QBFCIterator<ICustomerRetList, ICustomerRet>(b);

              foreach (var customer in customers)
              {
                var qryMod = updateBatch.MsgSet.AppendCustomerModRq();

                qryMod.ListID.SetValue(Safe.Value(customer.ListID));
                qryMod.EditSequence.SetValue(Safe.Value(customer.EditSequence));

                string tweakedName = Safe.Value(customer.CompanyName) + " tweaked by Zombie";

                qryMod.CompanyName.SetValue(Safe.LimitedString(tweakedName, 41));
              }
            });

        StatusMgr.Trace("Running query for all edit sequences");
        if (!queryBatch.Run()) return;

        StatusMgr.Trace("Updating all customers");
        updateBatch.Run();
      }
    }
  }
}