/********************************************************************************************\
* Name: Nathan VelaBorja
* Date: November 5, 2015
* Class: CptS 322 - Fall 2015
* Assignment: 10 - Circular Reference Check
* Description: This program mimics the basic functionality of an excel spreadsheet, using a 
 *      custom cell-class as the underlying data grid type.
\********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SpreadsheetEngine
{
    /********************************************************************************************\
     * Class: SpreadsheetCell
     * Description: This class inherits from the abstract Cell class and currently modifies it very
     *       little. As of now it only adds constructor functionality which is absolutely necessary
     *       in Spreadsheet creation. This will likely be expanded in future assignments
    \********************************************************************************************/
    public class SpreadsheetCell : Cell
    {
        #region Members

        private List<SpreadsheetCell> dependencies;                         // Represents the list of cells that are dependent on this cell, all dependent cells will be re-evaluated when this cell is changed
        
        #endregion

        #region Constructors

        public SpreadsheetCell(int Row, int Column)                         // Create new cell given a row and column index
        {
            this.RowIndex = Row;                                            // Copy row
            this.ColIndex = Column;                                         // Copy column
            this.Text = "";                                                 // Set text to empty string
            this.Value = this.Text;                                         // Initially set value to match text
            dependencies = new List<SpreadsheetCell>();                     // Create dependencies list
            BGColor = -1;                                                    // Set cell's background color to -1 (white)
        }

        #endregion

        #region Properties

        public List<SpreadsheetCell> Dependencies                           // Gets or sets the private list of dependencies
        {
            get { return dependencies; }
            set { dependencies = value; }
        }

        #endregion

        #region Helper Functions

        public void AddDependency(SpreadsheetCell dependency)               // Takes in a dependent cell and adds it to the list of dependencies
        {
            dependencies.Add(dependency);
        }

        public void ResetCell()                                             // Resets all cell values to default
        {
            Text = "";                                                      // Set text to empty string
            Value = "";                                                     // Set value to empty string
            BGColor = -1;                                                   // Set color to white
            dependencies.Clear();                                           // Clear dependencies list
        }

        #endregion
    }
}
