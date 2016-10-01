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
using System.Drawing;
using System.Xml;
using System.IO;
using CptS322;

namespace SpreadsheetEngine
{
    /********************************************************************************************\
     * Class: Spreadsheet
     * Description: This class contains the 2-Dimensional array of cells that are linked to the 
     *      dataGridView in the win form. It is responsible for cell handling, and notifying 
     *      Form1 when cell values change for UI updates
    \********************************************************************************************/
    public class Spreadsheet : INotifyPropertyChanged
    {
        #region Class Members

        public SpreadsheetCell[,] cellGrid;                                                         // 2-D array of cells representing the cell grid corresponding the UI dataGridView
        private ExpTree eTree;                                                                      // Expression Tree that will be used for all expression handling within this spreadsheet
        private int rowCount, columnCount;                                                          // Keep local record of number of rows and columns just incase we need them later
        public event PropertyChangedEventHandler PropertyChanged = delegate { };                    // Propery Changed handler to perform executions when spreadsheet cells trigger changes
        private Stack<UndoRedoCollection> undoStack;                                                // Stack of undos for undo funcitonality
        private Stack<UndoRedoCollection> redoStack;                                                // Stack of redos for redo functionality
        private XmlTextWriter writer;

        #endregion

        #region Constructors

        public Spreadsheet(int numRows, int numColumns)
        {
            undoStack = new Stack<UndoRedoCollection>();                                            // Initialize undo stack
            redoStack = new Stack<UndoRedoCollection>();                                            // Initialize redo stack
            cellGrid = new SpreadsheetCell[numRows, numColumns];                                    // Create 2D array of Cells based on the given number of rows and columns
            rowCount = numRows;                                                                     // Store numRows locally within rowCount
            columnCount = numColumns;                                                               // Store numColumns locally columnCount
            eTree = new ExpTree("");                                                                // Create new expression tree, give it only an empty string at first so it won't produce a value

            for (int i = 0; i < numRows; i++)                                                       // Iterate through 2D array of cells and give them their indeces
            {
                for (int j = 0; j < numColumns; j++)
                {
                    cellGrid[i, j] = new SpreadsheetCell(i, j);
                    cellGrid[i, j].PropertyChanged += CellPropertyChanged;                          // Add subscription for every cell's propertyChanged event
                    cellGrid[i, j].BGColor = -1;                                                    // Initialize all cell colors to white, for some reason this didn't work in the cell constructor alone, cells are set to black without this
                }
            }

            ClearUndoRedos();                                                                       // Creating all these cells actually populates the undo stack with a blank cell for each and a white color for each, clear them before the spreadsheet appears so they don't activate the undo menu strip
        }

        #endregion

        #region Properties

        public string TopUndoText                                                                   // Property that returns a string depending on the topmost item in the undo stack. Will be used to specify what type of undo will be made in the menu strip
        {
            get
            {
                UndoRedoCollection topUndo = undoStack.Pop();                                       // Pop the undo stack to get the topmost undo
                string text = topUndo.UndoRedoType();                                               // Set text equal to the type of the undo. As of 10.23.2015, Text will return "Text Change" and Color will return "Color Change"
                AddUndo(topUndo);                                                                   // Put the undo back onto the stack, we don't actually want to manipulate the stack in this process
                return text;                                                                        // Return the text
            }
        }

        public string TopRedoText                                                                   // Property that returns a string depending on the topmost item in the redo stack. Will be used to specify what type of redo will be made in the menu strip
        {
            get
            {
                UndoRedoCollection topRedo = redoStack.Pop();                                       // Pop the redo stack to get the topmost redo
                string text = topRedo.UndoRedoType();                                               // Set text equal to the type of the redo. As of 10.23.2015, Text will return "Text Change" and Color will return "Color Change"
                AddRedo(topRedo);                                                                   // Put the redo back onto the stack, we don't actually want to manipulate the stack in this process
                return text;                                                                        // Return the text
            }
        }

        public int UndoSize                                                                         // Gets the size of the undo stack, will be used to determine in the menu strip "Undo" feature should be enabled
        {
            get { return undoStack.Count; }
        }

        public int RedoSize                                                                         // Gets the size of the redo stack, will be used to determine in the menu strip "Redo" feature should be enabled
        {
            get { return redoStack.Count; }
        }

        public int ColumnCount                                                                      // Gets the amount of columns in the cellGrid
        {
            get { return columnCount; }
        }

        public int RowCount                                                                         // Gets the amount of rows in the cellGrid
        {
            get { return rowCount; }
        }

        #endregion

        #region Property Changed Event Functions

        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)                 // Gets called when some property within a Spreadsheet cell in the cellGrid gets changed
        {
            SpreadsheetCell currentCell = (sender as SpreadsheetCell);                              // Create a local member for the cell being changed, makes following code much cleaner
            if (e.PropertyName == "Text")                                                           // If Text is the property being changed
            {
                if (currentCell.Text.ToString().Length > 0)                                         // Only handle if the given text is not an empty string
                {
                    if (currentCell.Text[0] != '=')                                                 // If the text doesn't start with an '=', and therefore not an expression
                    {
                        currentCell.Value = (currentCell.Text);                                     // Set the value of the cell to the cell's text

                        if (currentCell.Text[0] != '!')                                                        // Only set the cell's dictionary value if the cell's text doesn't start with a '!' indicating an error message
                        {
                            SetVariable(currentCell);                                                   // Update the dictionary value for the current cell (variable being the current cell)
                        }
                        
                    }
                    else                                                                            // If it is an expression
                    {
                        string expression = currentCell.Text;                                       // Create local member for the cell's text, makes following code much cleaner
                        expression = expression.Substring(1, expression.Length - 1);                // Cut off the '=' at the beggining of the expression
                        expression = this.ConverToUpperCase(expression);                            // Convert all characters to upper case, so that we don't have to worry about case sensitivity

                        eTree.Expression = expression;                                              // Set the Epression Tree's expression to the given expression, will form expression tree for the expression at this time
                        currentCell.Value = eTree.Eval().ToString();                                // Evaluate the expression tree with the given expression and give the value to the current cell

                        SetVariable(currentCell);                                                   // Update the dictionary value for the current cell (variable being the current cell)
                        SetDependencies(currentCell, expression);                                   // Set the dependencies for any cell referenced within this cell's expression. After this, if any of those cells are changed, this cell will re-evaluate
                    }

                }
                else { (sender as SpreadsheetCell).Value = (""); }                                  // Set sender's value to empty string

                CheckDependencies((sender as SpreadsheetCell), e);                                  // Now that we have changed the value of this cell, check dependencies to see if any other cells need to be re-evaluated

                if (PropertyChanged != null)                                                        // Call PropertyChanged() method only if the event is valid
                {
                    PropertyChanged(this, new PropertyChangedEventArgs((sender as SpreadsheetCell).RowIndex.ToString() + "," + (sender as SpreadsheetCell).ColIndex.ToString() + "*Text"));
                }
            }
            else if (e.PropertyName == "Color")                                                     // If Color is the property being changed
            {
                if (PropertyChanged != null)                                                        // No expression evaluation necessary here, just pass it on to the PropertyChanged() method if the event is valid
                {
                    PropertyChanged(this, new PropertyChangedEventArgs((sender as SpreadsheetCell).RowIndex.ToString() + "," + (sender as SpreadsheetCell).ColIndex.ToString() + "*Color"));
                }
            }
        }
       
        private void SetDependencies(SpreadsheetCell cell, string expression)                       // Sets the dependencies for the given cell based on the given expression
        {
            char[] operators = new char[6] { '+', '-', '*', '/', '(', ')'};                         // List of meta characters to be expected from the expression, used for splitting the expression into pieces
            
            List<string> expressionPieces = new List<string>();                                     // Initialize the string List to be populated with said pieces

            expression = eTree.RemoveWhiteSpace(expression);                                        // Remove all white space from the given expression

            expressionPieces = expression.Split(operators).ToList();                                // Split the given expression based on the above metacharacters

            List<string> newPieces = new List<string>();                                            // Create another string List. This will be used to modify the original, taking out any empty pieces found while seperating parentheses
            foreach (string piece in expressionPieces)                                     
            {
                if (piece != "")                                                                    // If any piece is not an empty string ...
                {
                    newPieces.Add(piece);                                                           // ... Add it to the new list
                }
            }

            expressionPieces = newPieces;                                                           // Update original list to match new one

            foreach (string piece in expressionPieces)
            {
                if (piece[0] >= 'A' && piece[0] <= 'Z')                                             // If an upper case letter is dicovered ( all letter converted to upper case at this point ) ...
                {
                    if (!DependencyExists(CellFromName(piece), cell))                               // ... If no dependency already exists from the cell ... 
                    {
                        CellFromName(piece).AddDependency(cell);                                    // ... Add dependency
                    }
                }
            }
        }

        public void Undo()                                                                          // Undos the latest change in the spreadsheet                                            
        {
            if (undoStack.Count > 0)                                                                // If the undo stack has anything on it, should be impossible to fail but good to have just in case
            {
                UndoRedoCollection undo = undoStack.Pop();                                          // Pop the undo stack, save it locally
    
                UndoRedoCollection redo = undo.CreateRedo();                                        // Create a redo based on the undo that was just popped

                AddRedo(redo);                                                                      // Add this redo to the redo stack

                undo.Execute();                                                                     // Execute the undo
            }
        }

        public void Redo()                                                                          // Redos the latest change in the spreadsheet
        {
            if (redoStack.Count > 0)                                                                // If the redo stack has anything on it, again, should be impossible to fail condition
            {
                UndoRedoCollection redo = redoStack.Pop();                                          // Pop the redo stack, save it locally

                UndoRedoCollection undo = redo.CreateUndo();                                        // Create an undo based on the redo that was just popped

                AddUndo(undo);                                                                      // Add this undo to the undo stack

                redo.Execute();                                                                     // Execute the redo
            }   
        }

        public void Demo(int maxRows, int maxCols)                                                  // Spreadsheet demo from an earlier assignment. Consider deleting.
        {
            Random rng = new Random();

            // Set text of 50 random cells to "IT WORKED!! :D"
            for (int i = 0; i < 50; i++)
            {
                int row = rng.Next(maxRows);
                int col = rng.Next(maxCols);

                cellGrid[row, col].Text = "IT WORKED!! :D";
            }

            // Set the text in every cell in column B to "This is cell B#" 
            for (int i = 0; i < maxRows; i++)
            {
                cellGrid[i, (Convert.ToInt32('B') - 'A')].Text = ("This is cell B" + (i + 1).ToString());
            }

            // Set the text in every cell in column A to "=B#"
            for (int i = 0; i < maxRows; i++)
            {
                cellGrid[i, (Convert.ToInt32('A') - 'A')].Text = ("=B" + (i + 1).ToString());
            }
        }

        #endregion

        #region Load & Save

        public void Load(string fileName)                                                           // Loads an XML file and populates the spreadsheet with the XML information
        {
            string xmlContent = File.ReadAllText(fileName, Encoding.UTF8);                          // Put all of the XML file content into a single string (necessary to create the XmlReader below, couldn't find another way to do this)
            string row = null, column = null, text = null, color = null, latestNode = null;         // Create a local account of the current row index, column index, color, text, and the most recently seen node type

            XmlReader reader = XmlReader.Create(new StringReader(xmlContent));                      // Create an XmlReader given the XML file string

            reader.ReadToFollowing("Spreadsheet");                                                  // Skip any XML intro stuff and get to the beginning of the stuff we care about
            
            while (reader.Read())                                                                   // Basically, while there's anything left in the XML file
            {
                if (reader.NodeType == XmlNodeType.Element)                                         // If we come across an element node
                {
                    if (reader.Name == "Row")                                                       // If it's a row node
                    {
                        latestNode = "Row";                                                         // Set latest node to "Row"
                    }
                    else if (reader.Name == "Column")                                               // If it's a column node
                    {
                        latestNode = "Column";                                                      // Set latest node to "Column"
                    }
                    else if (reader.Name == "Text")                                                 // If it's a text node
                    {
                        latestNode = "Text";                                                        // Set latest node to "Text"
                    }   
                    else if (reader.Name == "Color")                                                // If it's a color node
                    {
                        latestNode = "Color";                                                       // Set latest node to "Color"
                    } 
                }

                else if (reader.NodeType == XmlNodeType.Text)                                       // Otherwise, if the node is a text node, do something depending on the last element node we crossed
                {
                    if (latestNode == "Row")                                                        
                    {
                        row = reader.Value;                                                         // Set row to the next value
                    }
                    else if (latestNode == "Column")
                    {
                        column = reader.Value;                                                      // Set column to the next value
                    }
                    else if (latestNode == "Text")
                    {
                        text = reader.Value;                                                        // Set text to the next value

                        if (text != null)                                                           // If text was actually set to something
                        {
                            cellGrid[Convert.ToInt32(row), Convert.ToInt32(column)].Text = text;    // Get the cell from the latest row and column information and set it's text to the found text
                        }
                    }
                    else if (latestNode == "Color")                                                 // Otherwise, if the node is a color node
                    {
                        color = reader.Value;                                                       // Set color to the next value

                        if (color != null)                                                          // If color was actually set to something
                        {
                            cellGrid[Convert.ToInt32(row), Convert.ToInt32(column)].BGColor = Convert.ToInt32(color);   // Get the cell from the latest row and column information and set it's color to the found color
                        }
                    } 
                }
            }
        }

        public void Save(string fileName)                                                           // Saves all current spreadsheet information to a user-specified XML file ( may be a new one )
        {
            // Wrote the following code after following the tutorial on http://www.lessthanweb.com/blog/writing-xml-in-c-net-with-xmltextwriter 
            // Output Format: <Spreadsheet> -> <Cell> -> <Row> -> <Column> -> *<Text> -> *<Color>      *- indicating optional

            writer = new XmlTextWriter(fileName, Encoding.UTF8);                                    // Create new XmlTextWriter based on the user-selected file with UTF8 encoding
            writer.Formatting = Formatting.Indented;                                                // Set the format to indenting (not sure what happens without this, but indentation seems standard)
            writer.WriteStartDocument();                                                            // Start document 
            writer.WriteStartElement("Spreadsheet");                                                // Start outter-most element "Spreadsheet"

            foreach (SpreadsheetCell cell in cellGrid)                                              // Loop through each cell in the spreadsheet
            {
                if (!cell.IsDefault)                                                                // If cell has any non-default values
                {
                    writer.WriteStartElement("Cell");                                               // Start cell element
                    writer.WriteElementString("Row", cell.RowIndex.ToString());                     // Write the row element value as the cell's row index
                    writer.WriteElementString("Column", cell.ColIndex.ToString());                  // Write the column element value as the cell's column index 

                    if (cell.Text != "")                                                            // If cell's text is non-default
                    {
                        writer.WriteElementString("Text", cell.Text);                               // Write the text element value as the cell's text 
                    }

                    if (cell.BGColor != -1)                                                         // If the cell's color is non-default
                    {
                        writer.WriteElementString("Color", cell.BGColor.ToString());                // Write the color element value as the cell's color
                    }

                    writer.WriteEndElement();                                                       // End the cell element
                }
            }

            writer.WriteEndElement();                                                               // End the spreadsheet element
            writer.WriteEndDocument();                                                              // End the document
            writer.Flush();                                                                         // Not positive what this does, I think it just clears the stream or something, but I read that's it's good practice to include
            writer.Close();                                                                         // Close the writer
        }   

        #endregion

        #region Helper Functions

        public SpreadsheetCell CellFromName(string cellName)                                       // Returns the cell from the cellGrid based on the given name ( ex: A2, R22, D19 )
        {
            string columnChar = cellName.Substring(0, 1);                                           // Gets the substring of the cellName that corresponds to the column, assuming our columns don't exceed single characters. Made it a string instead of a character to make use of the substring function
            string rowNumber = cellName.Substring(1, cellName.Length - 1);                          // Gets the substring of the rest of the cell name that corresponds to the row number

            return cellGrid[Convert.ToInt16(rowNumber) - 1, Convert.ToInt16(Convert.ToChar(columnChar) - 'A')];     // Converts the strings into index values and returns the cell located at those index values
        }

        private bool DependencyExists(SpreadsheetCell from, SpreadsheetCell to)                     // Decides if the cell "from" has any dependcies to "to". Might want to rename these at some point as they're confusing
        {
            foreach (SpreadsheetCell dependency in from.Dependencies)                               // Loop through each dependency in "from"'s dependcies list
            {
                if (to == dependency)                                                               // If any of the dependencies match "to: ...
                {
                    return true;                                                                    // ... Return true
                }
            }
            return false;                                                                           // ... if not, return false
        }

        private void CheckDependencies(SpreadsheetCell cell, PropertyChangedEventArgs e)            // When a cell is changed, go through all cells that depend on it and re-evaluate them
        {
            if (cell.Dependencies.Count > 0)                                                        // If there are one or more cells depending on this cell ...
            {
                for (int i = 0; i < cell.Dependencies.Count; i++)                                   // ... Loop through the dependencies and ...
                {
                    CellPropertyChanged(cell.Dependencies[i], e);                                   // ... Call their property changed event ( causes re-evaluation without actually modifying any properties of the cell )
                }
            }
        }
        public SpreadsheetCell GetCell(int row, int column)                                         // Returns the cell at the specified row and column   
        {
            if (cellGrid[row, column] != null)                                                      // If the cell at the given location is set to an instance of an object
            {
                return cellGrid[row, column];                                                       // Return it!
            }

            return null;                                                                            // Otherwise return null
        }

        public void AddUndo(UndoRedoCollection undo)                                                // Adds a given undo to the undo stack
        {
            undoStack.Push(undo);                                                                   // Push given undo on to the undo stack
        }

        public void AddRedo(UndoRedoCollection redo)                                                // Adds a given redo to the redo stack
        {
            redoStack.Push(redo);                                                                   // Push given redo on to the redo stack
        }

       public void ClearUndoRedos()                                                                 // Clears all undos and redos in the two stacks. Should really be necessary, but could be very helpful in future additions
        {
            undoStack.Clear();                                                                      // Clear undo stack
            redoStack.Clear();                                                                      // Clear redo stack
        }

        public void ClearVariables()                                                                // Clears all dictionary entries in the variable dictionary located in the expression tree
        {
            eTree.ClearVariables();                                                                 // Clear the dictionary
        }

        private void SetVariable(SpreadsheetCell cell)                                              // Create a dictionary entry in the expression tree for the given cell
        {
            string cellName = (Convert.ToChar(cell.ColIndex + Convert.ToInt16('A'))).ToString() + (cell.RowIndex + 1).ToString();   // Get the cell name in form "X00" by manipulating the cell's column and row numbers
            double cellValue = Convert.ToDouble(cell.Value);                                        // Copy the cell's value to a local variable. Not necessary, but makes code slightly cleaner
            eTree.SetVar(cellName, cellValue);                                                      // Create variable inside the expression tree variable dictionary using the cell name and value
        }

        private string ConverToUpperCase(string expression)                                         // Simply converts all letter characters in the given expression to upper case
        {
            for (int i = 0; i < expression.Length - 1; i++)                                         // For each character in the expression
            {
                if (expression[i] >= 'a' && expression[i] <= 'z')                                   // If the character is lowercase, convert to upper case and replace the character in the string
                {
                    char newCharacter = Convert.ToChar('A' + (expression[i] - 'a'));                // New character shifts the current character value to upper case
                    expression = expression.Replace(expression[i], newCharacter);                   // Replace the character in the expression with the new character
                }
            }
            return expression;                                                                      // Return the new (possibly unmodified) expression
        }


        #endregion
    }
}
