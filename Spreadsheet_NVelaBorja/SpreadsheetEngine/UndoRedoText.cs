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

namespace SpreadsheetEngine
{
    /********************************************************************************************\
     * Class: UndoRedoText
     * Description: This class inherits from the UndoRedoCollection interface and specifies a cell's
     *      change in text
    \********************************************************************************************/
    public class UndoRedoText : UndoRedoCollection  
    {
        private string text;                                        // local collection of the cell's text at the time this Undo/Redo was created
        SpreadsheetCell cell;                                       // Reference to cell that was changed

        public UndoRedoText()                                       // Empty constructor
        {

        }

        public UndoRedoText(ref SpreadsheetCell Cell)               // Constructor given a reference to a cell
        {
            cell = Cell;                                            // Set our local cell to the given cell 
            text = cell.Text;                                       // Set our local text to the cell's text ( won't change as the cell's text continues to)
        }

        public void Execute()                                       // Set's the cell's text back to the text we received
        {
            cell.Text = text;
        }

        public UndoRedoCollection CreateRedo()                      // Create redo based on this undo (only called from undos)
        {
            UndoRedoText redo = new UndoRedoText();

            redo.cell = cell;
            redo.text = cell.Text;

            return redo;
        }

        public UndoRedoCollection CreateUndo()                      // Create undo based on this redo (only called from redos)
        {
            UndoRedoText undo = new UndoRedoText();
            undo.cell = cell;
            undo.text = cell.Text;

            return undo;
        }

        public string UndoRedoType()                                 // Return string describing this undo/redo as a text change
        {
            return "Text Change";
        }
    }
}
