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
     * Interface: UndoRedoCollection
     * Description: This interface represents a change in a cell property for undo/redo functionality
    \********************************************************************************************/
    public interface UndoRedoCollection
    {
        void Execute();                                                 // Execute undo / redo

        UndoRedoCollection CreateRedo();                                // Create redo based on current undo/redo

        UndoRedoCollection CreateUndo();                                // Create undo based on current undo/redo

        string UndoRedoType();                                          // Returns a string representing the type of change this undo/redo refers to
    }
}
