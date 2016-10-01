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
     * Class: Cell
     * Description: This is the abstract cell class used by the Spreadsheet class through SpreadsheetCell
     *      and through it, Form1. It contains most of Cell's current logic and uses INotifyProperyChanged
     *      to let subscribers know of its property changes (currently only cell.Value).
    \********************************************************************************************/
    public abstract class Cell : INotifyPropertyChanged
    {
        #region Class Members

        private int rowIndex;                                                       // Index of the cell's row
        private int columnIndex;                                                    // Index of the cell's column
        private string text;                                                        // Text value of the cell
        private string _value;                                                      // Value of the cell, either matches the text or holds the result of an expression evaluation
        private int bgColor;                                                        // Integer values representing the background color of this cell 
        public event PropertyChangedEventHandler PropertyChanged = delegate { };    // Honestly not sure what this does, has to do with the event handling stuff
        private bool isDefault;                                                     // Shows whether or not the cell has a non-default value

        #endregion

        #region Constructors

        public Cell()                                                               // Base constructor, simply sets all variables to an instance of an object so we won't get errors
        {
            rowIndex = 0;
            columnIndex = 0;
            text = "";
            _value = text;
            bgColor = -1;
            isDefault = true;
        }

        #endregion

        #region Properties

        public int RowIndex                                                         // RowIndex is read-only, no setter provided. Can only be set through constructor
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }
        public int ColIndex
        {
            get { return columnIndex; }
            set { columnIndex = value; }
        }                                                                           // Property. Get and set the cell's column index

        public string Text                                                          // Only set if given text is different than current text
        {
            get { return text; }
            set 
            {  
                if (value != text || value != "") 
                { 
                    text = value;                                                   // Update text if new value is actually different
                    Value = text;                                                   // Set Value equal to the text intially, the spreadsheet will handle the Value if the text starts with a '='

                    NotifyPropertyChanged("Text");                                  // Notify that Property was changed
                }
                if (value != "")
                {
                    isDefault = false;
                }
                else isDefault = true;
            }                  
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }                                                                           // Property. Get and Set the cell's value

        public int BGColor
        {
            get { return bgColor; }
            set
            {
                if (bgColor != value)
                {
                    bgColor = value;
                    NotifyPropertyChanged("Color");
                }
                if (value != -1)
                {
                    isDefault = false;
                }
                else isDefault = true;
            }
        }                                                                           // Propery. Get and Set the cell's Background color value

        public bool IsDefault
        {
            get { return isDefault; }
        }

        #endregion

        #region Helper Functions

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
