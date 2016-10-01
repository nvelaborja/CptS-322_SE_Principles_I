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
     * Class: UndoRedoColor
     * Description: This class inherits from the UndoRedoCollection interface and specifies a cell's
     *      change in color
    \********************************************************************************************/
    public class UndoRedoColor : UndoRedoCollection
    {
        private int color;                                          // Local colleciton of the given cell's color value
        List<SpreadsheetCell> cells;                                // List of cells that were changed prior to this object's instantiation

        public UndoRedoColor()                                      // Empty constructor
        {

        }

        public UndoRedoColor(ref List<SpreadsheetCell> Cells)       // Constructor given a list of cells
        {
            cells = Cells;                                          // Set our list to the ref of the given cells
            color = cells[0].BGColor;                               // Set our color to the color value of the first cell in the list (they should all be the same)
        }

        public void Execute()                                       // Set all cell's colors to the color of the cells at time of creation
        {
            foreach (SpreadsheetCell cell in cells)
            {
                cell.BGColor = color;
            }
        }

        public UndoRedoCollection CreateRedo()                      // Create a redo based on the current undo ( will only be called from undo instance )
        {
            UndoRedoColor redo = new UndoRedoColor();
            redo.cells = cells;
            redo.color = cells[0].BGColor;

            return redo;
        }

        public UndoRedoCollection CreateUndo()                      // Create an undo based on the current redo ( will only be called from redo instance )
        {
            UndoRedoColor undo = new UndoRedoColor();
            undo.cells = cells;
            undo.color = cells[0].BGColor;

            return undo;
        }

        public string UndoRedoType()                                // Returns a string representing this undo/redo as a color change
        {
            return "Color Change";
        }
    }
}
