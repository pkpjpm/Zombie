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
  internal class Program
  {
    private static void Main(string[] args)
    {
      var menuItems = new string[][]
      {
                new string[] {"1", "Customer List (Read Only)"},
                new string[] {"2", "Customer Update (NOT Read Only)"},
                new string[] {"3", "Account List (Read Only)"},
                new string[] {"4", "Serial Number Report"},
                new string[] {"Q", "Quit"}
      };

      try
      {
        ConnectionMgr.InitDesktop("Zombie demonstration console application");

        //the StatusConsole class will direct all error and trace information to the console
        StatusMgr.AddListener(new StatusConsole(), true);

        Console.WriteLine("\r\nThis application demonstrates the capabilities of the Zombie Library\r\n");

        char c;

        do
        {
          Console.WriteLine("\r\n=====================================================\r\n");
          Console.WriteLine("  Please select a demonstration:");

          foreach (var item in menuItems)
          {
            Console.WriteLine("\r\n\t{0}:\t{1}", item[0], item[1]);
          }

          c = Console.ReadKey().KeyChar;

          switch (c)
          {
            case '1':
              CustomerList.Show();
              break;

            case '2':
              CustomerUpdate.Run();
              break;

            case '3':
              AccountList.Show();
              break;

            case '4':
              SerialNumberReport.Run();
              break;

            case 'Q':
            case 'q':
              break;

            default:
              Console.WriteLine("unrecognized key: {0}", c);
              break;
          }
        }
        while (c != 'q' && c != 'Q');
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      Console.WriteLine("Thank you for trying Zombie!");
    }
  }
}