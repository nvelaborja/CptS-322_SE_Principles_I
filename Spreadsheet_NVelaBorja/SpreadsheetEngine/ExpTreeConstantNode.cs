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
     * Class: ExpTreeConstantNode
     * Description: Inherits from ExpTreeNode with added functionality for a node containing a 
     *         constant value. 
    \********************************************************************************************/
    public class ExpTreeConstantNode : ExpTreeNode
    {
        #region Class Members

        private double value = 0.0;                                                 // For constant nodes only

        #endregion

        #region Constructors

        public ExpTreeConstantNode(string Value)                                    // Create a new constant node based on the given value
        {
            value = Convert.ToDouble(Value);                                        // Simply convert the string value to a double value and store it locally
        }

        #endregion

        #region Properties

        public double Value                                                         // Gets and sets the node's value
        {
            get { return value; }
        }

        #endregion

        #region Functions

        public override double Evaluate()                                           // Returns the value of the node
        {
            return value;                                                           // No arithmetic necessary here, just return the value
        }

        #endregion
    }
}
