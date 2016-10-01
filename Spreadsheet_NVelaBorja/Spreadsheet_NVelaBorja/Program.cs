/********************************************************************************************\
* Name: Nathan VelaBorja
* Date: October 29, 2015
* Class: CptS 322 - Fall 2015
* Assignment: 9 - Load and Save
* Description: This program mimics the basic functionality of an excel spreadsheet, using a 
 *      custom cell-class as the underlying data grid type.
\********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spreadsheet_NVelaBorja
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
       
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
