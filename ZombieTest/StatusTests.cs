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

namespace ZombieTest
{
    public class StatusTests
    {
        public StatusTests()
        {
            StatusMgr.AddListener(new Zombie.StatusConsole(), true);
        }

        [Fact]
        public void BadErrorFormatWithSingleArgDoesNotThrowException()
        {
            StatusMgr.FormatError("This {0} is {1} wrong", 0);
        }

        [Fact]
        public void BadErrorFormatWithDoubleArgDoesNotThrowException()
        {
            StatusMgr.FormatError("This {0} is {1} really {2} wrong", 0, 0);
        }

        [Fact]
        public void BadErrorFormatWithArrayArgDoesNotThrowException()
        {
            StatusMgr.FormatError("This {0} is {1} wrong {5} again", new object[] { 1, 2, 3 });
        }

        [Fact]
        public void BadWarningFormatWithSingleArgDoesNotThrowException()
        {
            StatusMgr.FormatWarning("This is wrong {1}", 0);
        }
    }
}
