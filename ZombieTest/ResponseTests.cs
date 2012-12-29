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
            using (var cn = ConnectionMgr.GetTestConnection())
            {
            }
        }
    }
}
