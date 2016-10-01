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
     * Class: ExpTreeOperatorNode
     * Description: Inherits from ExpTreeNode with added functionality for a node containing an 
     *         operator.
    \********************************************************************************************/
    public class ExpTreeOperatorNode : ExpTreeNode
    {
        #region Class Members

        private ExpTreeNode leftChild;                                              // Next node on the left side
        private ExpTreeNode rightChild;                                             // Next node on the right side
        private char _operator = '\0';                                              // For operator nodes only

        #endregion

        #region Constructors

        public ExpTreeOperatorNode(string Operator)                                 // Creates a new operator node based on the given string
        {
            _operator = Convert.ToChar(Operator);                                   // Converts the string to a character
        }

        #endregion

        #region Properties

        public ExpTreeNode LeftChild
        {
            get { return leftChild; }
            set { leftChild = value; }
        }

        public ExpTreeNode RightChild
        {
            get { return rightChild; }
            set { rightChild = value; }
        }
        public char Operator
        {
            get { return _operator; }
        }

        #endregion

        public override double Evaluate()                                           // Override function evaluate, determines the nodes value and returns it
        {
            double value = 0.0;                                                     // Initialize node's value to 0.0

            switch (_operator)
            {
                case '+':
                    value = rightChild.Evaluate() + leftChild.Evaluate();           // Add right childs evaluate to the left childs evaluate
                    break;

                case '-':
                    value = rightChild.Evaluate() - leftChild.Evaluate();           // Subtract left childs evaluate from the right childs evaluate
                    break;

                case '*':
                    value = rightChild.Evaluate() * leftChild.Evaluate();           // Multiply right childs evaluate to the left childs evaluate
                    break;

                case '/':
                    value = rightChild.Evaluate() / leftChild.Evaluate();           // Divide right childs evaluate by the left childs evaluate
                    break;  
            }
            return value;
        }
    }
}
