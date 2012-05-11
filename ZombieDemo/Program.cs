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
using Interop.QBFC11;
using Zombie;

namespace ZombieDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConnectionMgr.InitDesktop("Zombie demonstration console application");

                StatusMgr.AddListener(new StatusConsole(), true);

                //AccountExample();

                InventoryUpdateTest();

                //InventoryQuery();

                //using (var cn = ConnectionMgr.GetConnection())
                //{
                //    var batch = cn.NewBatch();

                //    var qry = batch.MsgSet.AppendSalesOrderAddRq();

                //    var line = qry.ORSalesOrderLineAddList.Append();

                //    line.SalesOrderLineAdd.Desc.SetValue("Is it A & B or A &amp; B");

                //    Console.WriteLine(batch.MsgSet.ToXMLString());
                //}

                //using (var cn = ConnectionMgr.GetConnection())
                //{
                //    Console.WriteLine("Company file open");

                //    var batch = cn.NewBatch();

                //    var qryCust = batch.MsgSet.AppendCustomerQueryRq();

                //    batch.SetClosures(qryCust, b =>
                //    {
                //        var customers = new QBFCIterator<ICustomerRetList, ICustomerRet>(b);

                //        foreach (var customer in customers)
                //        {
                //            Console.WriteLine(Safe.Value(customer.FullName));
                //        }
                //    }); 

                //    batch.Run();                    
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press any key to exit");

            Console.ReadKey();
        }

        static void AccountExample()
        {
            using (var cn = ConnectionMgr.GetConnection())
            {
                var batch = cn.NewBatch();

                var qry = batch.MsgSet.AppendAccountQueryRq();

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

        static void InventoryQuery()
        {
            using (var cn = ConnectionMgr.GetConnection())
            {
                var batch = cn.NewBatch();

                var qry = batch.MsgSet.AppendInventoryAdjustmentQueryRq();

                var filter = qry.ORInventoryAdjustmentQuery.TxnFilterWithItemFilter.ORDateRangeFilter.TxnDateRangeFilter.ORTxnDateRangeFilter.TxnDateFilter;

                filter.FromTxnDate.SetValue(DateTime.Parse("5/1/2012"));
                filter.ToTxnDate.SetValue(DateTime.Parse("5/31/2012"));

                qry.IncludeLineItems.SetValue(true);

                batch.SetClosures(qry, b =>
                    {
                        var adjustments = new QBFCIterator<IInventoryAdjustmentRetList, IInventoryAdjustmentRet>(b);

                        foreach(var adjustment in adjustments)
                        {
                            Console.WriteLine(Safe.Value(adjustment.TxnDate));
                            
                            var lines = new QBFCIterator<IInventoryAdjustmentLineRetList, IInventoryAdjustmentLineRet>(adjustment.InventoryAdjustmentLineRetList);

                            foreach(var line in lines)
                            {
                                string lot = "none";

                                if(line.ORSerialLotNumberPreference != null)
                                {
                                    lot = Safe.Value(line.ORSerialLotNumberPreference.LotNumber);

                                    if (line.ORSerialLotNumberPreference.SerialNumberRet != null)
                                    {
                                        Console.WriteLine("serial number ret");

                                        if (line.ORSerialLotNumberPreference.SerialNumberRet.SerialNumberAddedOrRemoved != null)
                                        {
                                            Console.WriteLine(line.ORSerialLotNumberPreference.SerialNumberRet.SerialNumberAddedOrRemoved.GetValue());
                                        }
                                    }

                                }

                                Console.WriteLine("{0} lot:{1} quantity:{2} value:{3}", new object[]
                                    {
                                        Safe.FullName(line.ItemRef), lot, Safe.Value(line.QuantityDifference), Safe.Value(line.ValueDifference)
                                    });

                            }
                        }
                    });

                batch.Run();
            }
        }

        static void InventoryUpdateTest()
        { 
            using (var cn = ConnectionMgr.GetConnection())
            {
                var batch = cn.NewBatch();

                //var qry = batch.MsgSet.AppendInventoryAdjustmentAddRq();

                var qry = batch.MsgSet.AppendInventoryAdjustmentModRq();

                qry.AccountRef.FullName.SetValue("Material Costs");

                //var line = qry.InventoryAdjustmentLineAddList.Append();

                var line = qry.InventoryAdjustmentLineModList.Append();

                line.ItemRef.FullName.SetValue("Acrylic Bowl");

                //var lot = line.ORTypeAdjustment.LotNumberAdjustment;
                //var lot = line.

                //lot.LotNumber.SetValue("A666");
                //lot.NewCount.SetValue(12);

                //string badXml = batch.MsgSet.ToXMLString();

                //string goodXml = badXml.Replace("NewCount", "CountAdjustment");

                Console.WriteLine(batch.MsgSet.ToXMLString());

                //batch.Run();

                StatusMgr.LogStatus("Complete");
            }
        }
    }
}
