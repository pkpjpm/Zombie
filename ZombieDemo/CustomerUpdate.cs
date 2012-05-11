using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zombie;
using Interop.QBFC11;

namespace ZombieDemo
{
    class CustomerUpdate
    {
        public static bool Run()
        {
            using (var cn = ConnectionMgr.GetConnection())
            {
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
                if (!queryBatch.Run()) return false;

                StatusMgr.Trace("Updating all customers");
                return updateBatch.Run();
            }
        }

    }
}
