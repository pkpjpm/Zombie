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
using NUnit.Framework;
using Zombie;
using Moq;
using Interop.QBFC11;
using ZombieTest.Support;

namespace ZombieTest
{
    [TestFixture]
    class ResponseTests
    {
        public ResponseTests()
        {
            StatusMgr.AddListener(new StatusConsole(), true);
        }

        [Test]
        public void QueryForItemsWithOneMissingSucceeds()
        {
            var msgMock = new Mock<IMsgSetRequest>();

            var queryMock = new Mock<ICustomerQuery>();

            msgMock.Setup(x => x.AppendCustomerQueryRq()).Returns(queryMock.Object);

            var sessionFake = new SessionFake(msgMock.Object);

            using (var cn = ConnectionMgr.GetTestConnection(sessionFake))
            {
                var batch = cn.NewBatch();

                var query = batch.MsgSet.AppendCustomerAddRq();

                batch.SetClosures(query, b =>
                    {
                        var customers = new QBFCIterator<ICustomerRetList, ICustomerRet>(b);

                        foreach(var customer in customers)
                        {
                            Console.WriteLine(Safe.Value(customer.Name));
                        }
                    }, null, true);

                Assert.AreEqual(true, batch.Run());
            }
        }
    }
}
