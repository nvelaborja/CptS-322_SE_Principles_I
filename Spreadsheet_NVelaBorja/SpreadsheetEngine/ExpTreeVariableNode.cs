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

namespace CptS322
{
    /********************************************************************************************\
     * Class: ExpTreeVariableNode
     * Description: Inherits from ExpTreeNode with added functionality for a node containing a
     *      variable value linked to the tree's variable dictionary
    \********************************************************************************************/
    public class ExpTreeVariableNode : ExpTreeNode
    {
        #region Class Members

        private string variableName = "";                                                           // For variable nodes only
        private double value = 0.0;                                                                 // For constant nodes only
        private Dictionary<string, double> variables;                                               // Local reference to the dictionary that holds all variable values (variables in this case being cells)

        #endregion

        #region Constructors

        public ExpTreeVariableNode(string VariableName, ref Dictionary<string,double> Variables)    // Create a new variable node based on the given name and reference to variable dictionary
        {
            variableName = VariableName;                                                            // Set this nodes name to the given name
            variables = Variables;                                                                  // Set our local copy of the variable dictionary to the one given
        }

        #endregion

        #region Properties

        public string VariableName                                                                  // Property returns variable name (read only)
        {
            get { return variableName; }
        }

        public double Value                                                                         // Property returns variable value (read only)
        {
            get { return value; }
        }

        #endregion

        #region Functions

        public override double Evaluate()                                                           // Evaluate function, returns the value associated with this variable's name in the variable dictionary
        {
            if (variables.ContainsKey(variableName))                                                // If the cell has a set variable, return it
            {
                return variables[VariableName];

            }
            else return 0.0;                                                                        // Otherwise just return 0 in the case of an empty cell
        }

        #endregion
    }
}
