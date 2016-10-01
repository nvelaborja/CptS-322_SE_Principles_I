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
     * Class: ExpTreeNode
     * Description: This class represent a base class node for the expression tree class ExpTree 
    \********************************************************************************************/
    public abstract class ExpTreeNode
    {
        #region Class Members

        private int depth = 0;                                                              // Not currently used, but left in for expected development

        #endregion

        #region Properties

        public int Depth                                                                    // Property that gets and sets the nodes depth, also not being used as of assignment 8
        {
            get { return depth; }
            set { depth = value; }
        }

        #endregion

        #region Abstract Functions

        public abstract double Evaluate();                                                  // Will evaluate the Node's value

        #endregion

        #region Helper Functions
        public void IncrementDepth()                                                        // Increment the nodes depth by one
        {
            depth++;
        }
        #endregion

    }

}


