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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using SpreadsheetEngine;
using CptS322;

namespace Spreadsheet_NVelaBorja
{
    /********************************************************************************************\
     * Class: Form1
     * Description: This class is the main driver for the form and handles all events involving the 
     *      UI.
    \********************************************************************************************/

    #region Known Bugs

    /********************************************************************************************\
     * When color dialog opens, even if the user presses cancel the color change still occurs - Solved
     * When changing color of an entire region, then doing it again over an intersecting region, 
     *      undo/redo do not work properly
     * 
    \********************************************************************************************/

    #endregion

    public partial class Form1 : Form
    {
        #region Members

        private int maxColumns = 26;                                                        // A - Z
        private int maxRows = 50;                                                           // Starting at 1, not 0
        private Spreadsheet spreadSheet;                                                    // Private spreadsheet, acts as the underlying data structure for the datagridview
        private string errorMessage = "";                                                   // Error message the cell will display if it fails any of the expression tests

        #endregion

        #region Constructors

        public Form1()                                                                      // Default constructor made by WinForms
        {
            InitializeComponent();
        }

        #endregion

        #region Event Functions

        private void Form1_Load(object sender, EventArgs e)                                     // Calls on form load, basically an initialization functions
        {
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();                  // Centers Column Headers
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid.ColumnHeadersDefaultCellStyle = cellStyle;

            for (int i = 0; i < maxColumns; i++)                                            // Create Spreadsheet Columns A-Z
            {
                char columnName = (char)(i + 65);
                dataGrid.Columns.Add(columnName.ToString(), columnName.ToString());
                dataGrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;      // Disable sorting for each cell, causes problems and is irritating
            }

            dataGrid.Rows.Add(maxRows);                                                     // Create Spreadsheet Rows 1-50
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                row.HeaderCell.Value = (row.Index + 1).ToString();                          // Name each row after their creation
            }
            dataGrid.RowHeadersWidth = 50;                                                  // Resize row header columns so double digits are visible, may need to expand if we extend to triple digits or more.

            spreadSheet = new Spreadsheet(maxRows, maxColumns);                             // Instantiate spreadSheet based on the defined maximum size
            spreadSheet.PropertyChanged += SpreadsheetPropertyChanged;                      // Subscribe this form's spreadsheet to the SpreadSheetPropertyChanged event
        }

        private void SpreadsheetPropertyChanged(object sender, PropertyChangedEventArgs e)      // Gets called when any properties in the spreadsheet are changed so the UI can reflect changes
        {
            string[] args = new string[2];
            args = e.PropertyName.Split('*');                                               // Split the property name into two pieces seperated by the asterisk, first part is the type of property change, second is the location of the change

            string[] coordinate = new string[2];                                            // Find the coordinates based on the given coordinate string
            coordinate = args[0].Split(',');
            int row = Convert.ToInt32(coordinate[0]);                                       // Convert the strings to integers
            int col = Convert.ToInt32(coordinate[1]);

            if (args[1] == "Text")                                                          // If text was changed
            {
                dataGrid.Rows[row].Cells[col].Value = (sender as Spreadsheet).cellGrid[row, col].Value; // Set the UI cell to match its corresponding SpreadSheetCell
            }
            else if (args[1] == "Color")                                                    // If color was changed
            {
                Color cellColor = new Color();                                              // Create a new color based on the cell's argb integer value
                SpreadsheetCell currentCell = (sender as Spreadsheet).GetCell(row, col);
                cellColor = Color.FromArgb((sender as Spreadsheet).GetCell(row, col).BGColor);
                dataGrid.Rows[row].Cells[col].Style.BackColor = cellColor;                  // Set the UI cell color to match its corresponding SpreadSheetCell color
            }
        }

        private void dataGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)   // Calls when a cell in the UI begins an edit (before its value is actually changed though)
        {
            SpreadsheetCell currentCell = spreadSheet.GetCell(e.RowIndex, e.ColumnIndex);       // Store local copy of the sender cell
            UndoRedoText undoText = new UndoRedoText(ref currentCell);                          // Before updating the UI, add current text state to the undo stack

            spreadSheet.AddUndo(undoText);                                                      // Add undo for the state of the current cell before modification

            dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = spreadSheet.GetCell(e.RowIndex, e.ColumnIndex).Text; // Set the UI cell's value to match the corresponding SpreadSheetCell
        }

        private void dataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)           // Calls when a cell in the UI has finished being modified
        {
            if (dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)                   // If the UI cell's value is set to an instance of an object (if it isn't blank)
            {
                if (PassesTests(dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), e.RowIndex + 1, e.ColumnIndex))            // Must offset row by one to account for 0 indexing, column doesn't need it but I'm not sure why..
                {
                    spreadSheet.GetCell(e.RowIndex, e.ColumnIndex).Text = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();  // Set the SpreadSheetCell's text property to the value of the UI cell
                }
                else                                                                            // If the expression failed any of our tests ...
                {
                    spreadSheet.GetCell(e.RowIndex, e.ColumnIndex).Text = errorMessage;         // ... Have the cell display the error message created in the tests
                }
            }
            else
            {
                spreadSheet.GetCell(e.RowIndex, e.ColumnIndex).Text = "";                       // If the UI cell is null, set the SpreadSheetCell text to an empty string
            }
        }

        private void selectColorToolStripMenuItem_Click(object sender, EventArgs e)             // Calls when the color dialog is opened
        {
            colorDialog1.Color = Color.White;                                                   // Set default color to white, not for any particular reason
            colorDialog1.ShowDialog();                                                          // Show dialog

            if (colorDialog1.Color != null)
            {
                List<SpreadsheetCell> selectedCells = new List<SpreadsheetCell>();                  // Construct a list that will contain all selected cells in the UI

                foreach (DataGridViewCell UICell in dataGrid.SelectedCells)                         // For each selected cell ...
                {
                    selectedCells.Add(spreadSheet.GetCell(UICell.RowIndex, UICell.ColumnIndex));    // ... Add it to the selectedCells list
                }

                UndoRedoColor undoColor = new UndoRedoColor(ref selectedCells);                     // Create new undoColor based on the selected cells before modification
                spreadSheet.AddUndo(undoColor);                                                     // Add it to the undo list

                foreach (DataGridViewCell cell in dataGrid.SelectedCells)                           // For each cell in the selected cells ...
                {
                    spreadSheet.GetCell(cell.RowIndex, cell.ColumnIndex).BGColor = colorDialog1.Color.ToArgb(); // ... Set the cell's color value to the color selected in the color dialog
                }
            }
        }

        private void UndoStrip_Click(object sender, EventArgs e)                                // Calls when the Undo strip is clicked in the Edit menu
        {
            spreadSheet.Undo();                                                                 // Forwards to the SpreadSheet's undo function
        }

        private void RedoStrip_Click(object sender, EventArgs e)                                // Calls when the Redo strip is clicked in the Edit menu
        {
            spreadSheet.Redo();                                                                 // Forwards to the SpreadSheet's redo function
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)          // Calls when the edit menu is clicked, completes before the menu strip is actually shown
        {
            if (spreadSheet.UndoSize > 0)                                                       // If we have anything on our undo stack ...
            {
                UndoStrip.Enabled = true;                                                       // ... Make the undo button clickable
                UndoStrip.Text = "Undo " + spreadSheet.TopUndoText;                             // ... Set the text to Undo + whatever kind of change the undo refers to
            }
            else                                                                                // If our undo stack is empty
            {
                UndoStrip.Enabled = false;                                                      // Make the undo button unclickable
                UndoStrip.Text = "Undo";                                                        // Set the text to just "Undo"
            }

            if (spreadSheet.RedoSize > 0)                                                       // If we have anything on our redo stack ...
            {
                RedoStrip.Enabled = true;                                                       // ... Make the redo button clickable
                RedoStrip.Text = "Redo " + spreadSheet.TopRedoText;                             // ... Set the text to Redo + whatever kind of change the redo refers to
            }
            else                                                                                // If our redo stack is empty
            {
                RedoStrip.Enabled = false;                                                      // Make the redo button unclickable
                RedoStrip.Text = "Redo";                                                        // Set the text to just "Redo"
            }
        }

        private void saveStrip_Click(object sender, EventArgs e)                                // Calls when the Save strip is clicked in the File menu
        {
            saveDialog.ShowDialog();                                                            // Display save file dialog

            if (saveDialog.FileName != "")                                                      // If they actually specified a file
            {
                spreadSheet.Save(saveDialog.FileName);                                          // Pass the file path to the spreadsheet's save function
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)                    // Calls when the Open strip is clicked in the File menu
        {
            openDialog.ShowDialog();                                                            // Display the open file dialog

            if (openDialog.FileName != "")                                                      // If the user actually selected a file
            {
                clearAllStrip_Click(sender, e);                                                 // Forward to the Clear All Event, will clear the entire spreadsheet
                spreadSheet.Load(openDialog.FileName);                                          // Pass the selected file's path to the spreadsheet's load function
            }
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)          // Calls when the File menu is clicked, completes before the file menu is actually shown
        {
            saveStrip.Enabled = false;                                                          // Initally make the save strip unclickable

            foreach (SpreadsheetCell cell in spreadSheet.cellGrid)                              // Loop through each cell in the spreadsheet
            {
                if (!cell.IsDefault)                                                            // If any of them have non-default values ...
                {
                    saveStrip.Enabled = true;                                                   // ... enable saving
                    break;
                }
            }
        }

        private void clearAllStrip_Click(object sender, EventArgs e)                            // Calls when the Clear All Strip is clicked in the Edit Menu
        {
            for (int i = 0; i < maxRows; i++)                                                   // Loop through all cells in the spreadsheet
            {
                for (int j = 0; j < maxColumns; j++)
                {
                    dataGrid.Rows[i].Cells[j].Value = "";                                       // Clear the UI Cell
                    spreadSheet.GetCell(i, j).ResetCell();                                      // Clear the logic engine cell
                }
            }
            spreadSheet.ClearUndoRedos();                                                       // Clear the undo and redo stacks
            spreadSheet.ClearVariables();                                                       // Clear the variables (spreadsheet cell values)
        }

        #endregion

        #region Helper Functions

        private bool PassesTests(string expression, int row, int column)                        // Runs through all of the different expression test. Expression will only be evaluated if it passes all tests
        {
            bool passes = true;                                                                 // Initialize bool to true

            passes = OK_Format(expression);                                                     // Check expressions format
            if (!passes) return passes;                                                         // If it failed, back out now, no need to check anything else

            passes = OK_SelfReference(expression, row, column);                                 // Next, check for self reference
            if (!passes) return passes;                                                         // If it failed, back out now, no need to check anything else

            passes = OK_CirclularReference(expression, row, column);                            // Finally, check for circular references

            return passes;                                                                      // Return true or false (really just the result of the circular reference test)
        }

        private bool OK_Format(string expression)                                               // Check to see if the expression satisfies functionality requirements
        {
            char[] delimiters = new char[4] { '+', '-', '/', '*' };

            if (expression[0] == '=')                                                           // If the expression starts with an '=', means the user must have tried to enter a computable expression
            {
                expression = expression.Substring(1);                                           // Remove the '='
                expression = RemoveWhiteSpace(expression);
                expression = expression.ToUpper();
                List<string> expressionPieces = expression.Split(delimiters).ToList();

                foreach (string piece in expressionPieces)
                {
                    try                                                                             // Try converting the expression to a double, if it works we're all good! 
                    {
                        Convert.ToDouble(piece);
                    }
                    catch                                                                           // If the conversion failed, check to see if it is a reference to a valid cell
                    {
                        char columnChar = piece[0];                                                 // If the piece is a valid, the first character will be A-Z and the rest will be 0-50, 
                        string rowNumber = piece.Substring(1);

                        if (columnChar - 'A' < 0 || columnChar - 'A' > 25)                          // If the column character isnt between A and Z, ERROR and back out
                        {
                            errorMessage = "! Format Error";
                            return false;
                        }

                        try                                                                         // Try to convert what should be a row number into an integer, if it fails then we have an error
                        {
                            int row = Convert.ToInt32(rowNumber);

                            if (row < 1 || row > maxRows)                                           // If it succeeded the conversion, we must then check to see if the given row was within our bounds
                            {
                                errorMessage = "! Format Error";
                                return false;
                            }
                        }
                        catch                                                                       // If the conversion failed, the cell name is not in proper format and the expression is not good
                        {
                            errorMessage = "! Format Error";
                            return false;
                        }
                    }
                }
            }
            else                                                                                    // If there is no '=', the expression must simply be a value, so if we can convert it to a double it's fine!
            {
                try
                {
                    Convert.ToInt32(expression);                                                    // Convert to double, if it succeeds we change nothing and isGood will return as true
                }
                catch
                {
                    errorMessage = "! Format Error";
                    return false;                                                                 // If it fails, we set isGood to false and return it right after
                }
            }

            return true;
        }

        private bool OK_SelfReference(string expression, int row, int column)                   // Check to see if the expression has no self references. If called, we have already verified that the expression is in proper format
        {
            char[] delimiters = new char[4] { '+', '-', '/', '*' };

            if (expression[0] == '=')                                                           // If the expression starts with an '=', means the user must have tried to enter a computable expression
            {
                expression = expression.Substring(1);                                           // Remove the '='
                expression = RemoveWhiteSpace(expression);                                      // Remove white space
                expression = expression.ToUpper();                                              // Convert all characters to upper case
                List<string> expressionPieces = expression.Split(delimiters).ToList();          // Split into chunks by expected operators

                foreach(string piece in expressionPieces)
                {
                    char columnChar = piece[0];                                                 // First check to see if the piece describes another cell. Grab the first character of the piece

                    if (columnChar < 'A' || columnChar > 'Z')                                   // If it's not A-Z, continue to next loop iteration
                    {
                        continue;
                    }
                    else                                                                        // If it is A-Z, go on to check the row number
                    {
                        int rowNumber = Convert.ToInt32(piece.Substring(1));                    // If it succeeds, check to see if it matches our current cell
                        int colNumber = columnChar - 'A';
                        if (CellMatch(rowNumber, colNumber, row, column))                       // If the row and column numbers match the row and column of our current cell, it must be a self reference!
                        {
                            errorMessage = "! Self Reference";
                            return false;
                        }
                    }
                }
            }
            return true;                                                                        // If there is no equals, it can't possibly reference itself!
        }

        private bool OK_CirclularReference(string expression, int row, int column)              // When called, we have already checked to make sure the expression is in proper format
        {
            bool isOkay = true;                                                                 // Initalize the return value to true
            
            if (expression != "")
            {
                if (expression[0] == '!')                                                       // If the first character is a '!' then it must be an error message, we will ignore this and continue
                {
                    return true;
                }

                List<string> cellReferences = new List<string>();                               // Create a list that will contain all references listed in the given expression
                cellReferences = FindCellReferences(expression);                                // Populate it using the FindCellReferences function

                foreach (string reference in cellReferences)                                    // For each reference ...
                {
                    int colNumber = reference[0] - 'A';                                         // Get the column number for the reference
                    int rowNumber = Convert.ToInt32(reference.Substring(1));                    // Get the row number for the reference

                    if (CellMatch(rowNumber, colNumber, row, column))                           // See if it matches our origin cell
                    {
                        errorMessage = "! Circular Reference";                                  // If so, set the error message to circular reference and return false (fail)
                        return false;
                    }

                    isOkay = OK_CirclularReference(spreadSheet.CellFromName(reference).Text, row, column);  // Otherwise, recursive call on the reference cell's expression
                    if (!isOkay) break;                                                         // If anything in within this recursive call failed, break out of the loop so the result isn't overridden 
                }
            }

            return isOkay;
        }

        private List<string> FindCellReferences(string expression)                          // Finds all referenced cells in A00 format and returns them in a string list
        {
            List<string> references = new List<string>();
            char[] delimiters = new char[4] { '+', '-', '/', '*' };

            expression = expression.Substring(1);                                           // Remove the '='
            expression = RemoveWhiteSpace(expression);                                      // Remove white space
            expression = expression.ToUpper();                                              // Convert to Upper
            List<string> expressionPieces = expression.Split(delimiters).ToList();          // Split up the expression

            foreach (string piece in expressionPieces)
            {
                char columnChar = piece[0];                                                 // First check to see if the piece describes another cell. Grab the first character of the piece

                if (columnChar < 'A' || columnChar > 'Z')                                   // If it's not A-Z, continue to next loop iteration
                {
                    continue;
                }
                else                                                                        // If it is A-Z, go on to check the row number
                {
                    references.Add(piece);                                                  // At this point, the piece must be a valid cell reference, so add it to the reference list                    
                }
            }

            return references;
        }

        private bool CellMatch(int row1, int column1, int row2, int column2)                // Simply determines whether or not the two given rows and columns match
        {
            if (row1 == row2 && column1 == column2)                            
            {
                return true;
            }

            return false;
        }

        public string RemoveWhiteSpace(string expression)                                   // Removes all white space from a given string including spaces, new lines, and tabs
        {
            int i = 0;

            while (i < expression.Length)
            {
                if (expression[i] == ' ' || expression[i] == '\n' || expression[i] == '\t')
                {
                    expression = expression.Remove(i, 1);
                }
                i++;
            }
            return expression;
        }

        #endregion
    }
}
