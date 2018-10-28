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
using Xunit;
using Zombie;
using Moq;
using Interop.QBFC13;

namespace ZombieTest
{
  public class ResponseTests
  {
    public ResponseTests()
    {
      StatusMgr.AddListener(new StatusConsole(), true);
    }

    [Fact]
    public void QueryForItemsWithOneMissingSucceeds()
    {
      var sessionMock = new Mock<IQBSessionManager>();

      var msgMock = new Mock<IMsgSetRequest>();

      var queryMock = new Mock<ICustomerQuery>();

      var requestListMock = new Mock<IRequestList>();

      var requestMock = new Mock<IRequest>();

      var responseSetMock = new Mock<IMsgSetResponse>();

      var responseListMock = new Mock<IResponseList>();

      var responseMock = new Mock<IResponse>();

      var customerListMock = new Mock<ICustomerRetList>();

      var customerMock = new Mock<ICustomerRet>();

      var customerNameMock = new Mock<IQBStringType>();

      sessionMock.Setup(x => x.CreateMsgSetRequest(It.IsAny<string>(), It.IsAny<short>(), It.IsAny<short>())).Returns(msgMock.Object);

      //for somre reason we have to explicitly setup these no-op methods on the mock
      //This only became necessary after the upgrade to QBSDK12
      sessionMock.Setup(x => x.EndSession());
      sessionMock.Setup(x => x.CloseConnection());

      msgMock.Setup(x => x.AppendCustomerQueryRq()).Returns(queryMock.Object);

      msgMock.Setup(x => x.RequestList).Returns(requestListMock.Object);

      requestListMock.Setup(x => x.Count).Returns(1);

      requestListMock.Setup(x => x.GetAt(0)).Returns(requestMock.Object);

      requestMock.Setup(x => x.Detail).Returns(queryMock.Object);

      requestMock.Setup(x => x.RequestID).Returns(0);

      sessionMock.Setup(x => x.DoRequests(msgMock.Object)).Returns(responseSetMock.Object);

      responseSetMock.Setup(x => x.ResponseList).Returns(responseListMock.Object);

      responseListMock.Setup(x => x.Count).Returns(1);

      responseListMock.Setup(x => x.GetAt(0)).Returns(responseMock.Object);

      responseMock.Setup(x => x.RequestID).Returns("0");

      responseMock.Setup(x => x.StatusCode).Returns(500); //500 means some object found, but others not

      responseMock.Setup(x => x.Detail).Returns(customerListMock.Object);

      customerListMock.Setup(x => x.Count).Returns(1);

      customerListMock.Setup(x => x.GetAt(0)).Returns(customerMock.Object);

      customerMock.Setup(x => x.Name).Returns(customerNameMock.Object);

      customerNameMock.Setup(x => x.GetValue()).Returns("Kilroy");

      using (var cn = ConnectionMgr.GetTestConnection(sessionMock.Object))
      {
        var batch = cn.NewBatch();

        var query = batch.MsgSet.AppendCustomerQueryRq();

        bool customerFound = false;

        batch.SetClosures(query, b =>
            {
              var customers = new QBFCIterator<ICustomerRetList, ICustomerRet>(b);

              foreach (var customer in customers)
              {
                Console.WriteLine(Safe.Value(customer.Name));
                customerFound = true;
              }
            }, null, true);

        Assert.Equal(true, batch.Run());
        Assert.True(customerFound);
      }
    }
  }
}