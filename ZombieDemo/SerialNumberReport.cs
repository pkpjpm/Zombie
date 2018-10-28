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
  internal class SerialNumberReport
  {
    public static void Run()
    {
      using (var cn = ConnectionMgr.GetConnection())
      {
        var batch = cn.NewBatch();

        var qry = batch.MsgSet.AppendGeneralSummaryReportQueryRq();

        qry.GeneralSummaryReportType.SetValue(Interop.QBFC13.ENGeneralSummaryReportType.gsrtSerialNumberInStockBySite);

        batch.SetClosures(qry, b =>
            {
              var report = b as IReportRet;

              for (int idx = 0; idx < Safe.Value(report.NumRows); idx++)
              {
                var row = report.ReportData.ORReportDataList.GetAt(idx);

                if (row.TextRow != null)
                {
                  Console.WriteLine(row.TextRow.value.GetValue());
                }

                if (row.DataRow != null)
                {
                  Console.WriteLine(row.DataRow.RowData.value.GetValue());

                  for (int idb = 0; idb < row.DataRow.ColDataList.Count; idb++)
                  {
                    Console.WriteLine(row.DataRow.ColDataList.GetAt(idb).value.GetValue());
                  }
                }
              }
            });

        batch.Run();
      }
    }
  }
}