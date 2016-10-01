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
     * Class: ExpTree
     * Description: This class represent an expression tree built of ExpTreeNodes. It builds 
     *      a tree based on expressions and can evaluate any generated tree.
    \********************************************************************************************/
    public class ExpTree
    {
        #region Class Members

        private char[] operators = new char[4] { '+', '-', '*', '/' };                              // Array of operators that this tree can accept
        private Dictionary<string, double> variables = new Dictionary<string,double>();             // Dictionary used for variable storage
        private ExpTreeNode rootNode, currentNode;                                                  // Root node is the start of the tree, current node used for tree formation and navigation 
        private double treeValue = 0.0;                                                             // Global value for tree, needed for evaluation recursion

        #endregion

        #region Constructors
            
        public ExpTree(string expression)                                                           // Create new tree based on given expression
        {
            expression = RemoveWhiteSpace(expression);                                              // Remove any white space from the expression
            TreeFromExpression(expression);                                                         // Forwards to exterior function so that it may be called elsewhere ( within expression property currently )
        }

        #endregion

        #region Properties

        public string Expression
        {
            set                                                                                     // On expression set, create new tree ( we are assuming that any given expression tree may be re-written )
            {
                string expression = RemoveWhiteSpace(value);
                rootNode = TreeFromExpression(expression);
            }                                              
        }

        #endregion

        #region Input/Output Functions

        public ExpTreeNode TreeFromExpression(string expression)
        {
            Stack<ExpTreeNode> operatorStack = new Stack<ExpTreeNode>();                            // Stack representing found operators in the expression, used in building tree
            Stack<ExpTreeNode> valueStack = new Stack<ExpTreeNode>();                               // Stack representing found values (including variables) in the the expression, used in building tree
            List<string> expressionPieces = new List<string>();                                     // List of objects found in expression, excluding operators
            List<string> operatorPieces = new List<string>();                                       // List stores all operators found in expression in order
            Queue<ExpTreeNode> subTrees = new Queue<ExpTreeNode>();                                 // List stores the roots of all subtrees found between parentheses
            ExpTreeNode currentRoot;                                                                // Node acts as a navigation tool and holds our current location within the tree
            int leftIndex = 0, rightIndex = 0, parentheses = 0;                                     // Index value count our position within the expression and parentheses count how many parentheses we have discovered
            bool parenthesisFound = false;                                                          // Boolean to keep track of whether or not we have seen any parentheses in the expression
            string newExpression = "";                                                              // New expression string to save modifications made to the given expression

            if (expression.Length == 0)                                                             // If the expression is empty, do nothing so we don't break!
            {
                return null;
            }

            expression = TrimExpression(expression);                                                // Trim parenthesis off the expression before we split

            if (CountHighOperators(expression) > 0 && CountAllOperators(expression) > 1)            // Add parentheses to the expression for precedence if we have more than one operator
            {   
                for (int i = 0; i < expression.Length; i++)                                         // For each character in the expression ...
                {
                    if (expression[i] == '*' || expression[i] == '/')                               // If the character is a high presidence operator
                    {
                        int left = FindLeftEntity(expression, i);                                   // Set left index to the outide of the nearest calcuable entity on the left side of the operator
                        int right = FindRightEntity(expression, i);                                 // Set right index to the outside of the nearest calcuable entity on the right side of the operator
                        newExpression = expression.Insert(right + 1, ")");                          // Add parentheses around the left and right of the entities around the operator
                        newExpression = newExpression.Insert(left, "(");                                //
                    }
                }
                expression = newExpression;                                                         // Give the new expression back to the original
            }

            #region Recursive Step

            while (moreParenthesis(expression))                                                     // Before we split, find parenthesis and do recursive call to build parentheses trees first
            {
                parentheses = 0;                                                                    // Initialize found parenthesis to 0
                for (int i = 0; i < expression.Length; i++)                                         // For each character in the expression
                {
                    if (expression[i] == '(')                                                       // If the character is an open parenthesis
                    {
                        leftIndex = i + 1;                                                          // Set left index to the inside of the parethesis
                        parenthesisFound = true;                                                    // Set parenthesisFound to true
                        break;
                    }
                }

                if (parenthesisFound)                                                               // If there was a valid parenthesis in the expression
                {
                    for (int i = leftIndex; i < expression.Length; i++)                             // For each character in the expression starting at the left index
                    {
                        if (expression[i] == ')')                                                   // If the character is an end parenthesis
                        {
                            if (parentheses == 0)                                                   // If all parentheses are closed already
                            {
                                rightIndex = i;                                                     // Set right index to this position
                                break;
                            }
                            else
                            {
                                parentheses--;                                                      // Otherwise decrement the amount of parenthesis we have 
                            }
                        }
                        else if (expression[i] == '(')                                              // If the character is an open parenthesis
                        {
                            parentheses++;                                                          // Increment our parenthesis counter
                        }
                    }

                    string subExpression = expression.Substring(leftIndex, rightIndex - leftIndex); // recursive call, build tree from expression within outter-most parentheses
                    ExpTreeNode subTree = TreeFromExpression(subExpression);                        // Create sub tree from new sub expression
                    subTrees.Enqueue(subTree);                                                      // Add sub tree's root to our sub tree queue
                    expression = expression.Remove(leftIndex - 1, subExpression.Length + 2);        // Remove sub-expression from expression, replace with '$', will be replaced with subTree dequeue later
                    expression = expression.Insert(leftIndex - 1, "$");
                }
            }

            #endregion

            operatorPieces = PopulateOperatorPieces(expression);                                    // First, populate the operator pieces list
 
            expressionPieces = expression.Split(operators).ToList();                                // Create Nodes from expression, assuming variable or constant must come first

            for (int i = 0; i < expressionPieces.Count; i++)                                        // For each object in the expression ( excluding operators )
            {
                ExpTreeNode newValueNode = CreateNode(expressionPieces[i]);                         // Create new value node based on the piece of the expression (could be variable or constant node)
                valueStack.Push(newValueNode);                                                      // Push the new node onto the value stack
                if (i != expressionPieces.Count - 1)                                                // If we're not at the end of the expression
                {
                    ExpTreeNode newOperatorNode = CreateNode(operatorPieces[i]);                    // Add a operator node to the operator stack based on our position in the expression
                    operatorStack.Push(newOperatorNode);
                }
            }

                                                                                                    // Now that we have Stacks built, start forming the tree
            if (operatorStack.Count > 0)                                                            // If we have any operators
            {
                currentRoot = operatorStack.Pop();                                                  // Set current root to the first operator on our stack
                currentNode = currentRoot;                                                          // Set current node to the current root
            }                                                                                       // Current node should never be anything other than a Operator node, except for after tree is constructed, at which point loop will end
            else
            {
                ExpTreeNode nextValueNode = valueStack.Pop();                                       // Get our next value node by popping the value stack
                if (isVariableNode(nextValueNode))                                                  // If its a variable node
                {
                    if (isSubTree(nextValueNode))
                    {
                        currentRoot = subTrees.Dequeue();                                           // If we find '$' variable, make right child the next subtree, set next operator node to new root, make its left our old root.
                    }
                    else
                    {
                        currentRoot = nextValueNode;                                                // Set left side of operator to the front of value stack ( value may still be variable names at this point )
                    }
                }
                else
                {
                    currentRoot = nextValueNode;                                                    // Set left side of operator to the front of value stack ( value may still be variable names at this point )
                }
            }

            while (valueStack.Count > 0)                                                            // While we still have something on the value stack ( tree should be formed by the end of this loop )
            {
                ExpTreeNode nextValueNode = valueStack.Pop();
                if (isVariableNode(nextValueNode))
                {
                    if (isSubTree(nextValueNode))
                    {
                        (currentNode as ExpTreeOperatorNode).LeftChild = subTrees.Dequeue();        // If we find '$' operator, make right child the next subtree, set next operator node to new root, make its left our old root.
                    }
                    else
                    {
                        (currentNode as ExpTreeOperatorNode).LeftChild = nextValueNode;             // Set left side of operator to the front of value stack ( value may still be variable names at this point )
                    }
                }
                else
                {
                    (currentNode as ExpTreeOperatorNode).LeftChild = nextValueNode;                 // Set left side of operator to the front of value stack ( value may still be variable names at this point )
                }

                if (operatorStack.Count > 0)                                                        // If there are still any operator nodes left
                {
                    ExpTreeNode nextOpNode = operatorStack.Pop();
                    (currentNode as ExpTreeOperatorNode).RightChild = nextOpNode;                   // Set right child to next operator node
                }
                else                                                                                // Operator queue is empty, means we're at the end of the tree
                {
                    nextValueNode = valueStack.Pop();
                    if (isVariableNode(nextValueNode))
                    {
                        if (isSubTree(nextValueNode))
                        {
                            (currentNode as ExpTreeOperatorNode).RightChild = subTrees.Dequeue();   // If we find '$' operator, make right child the next subtree, set next operator node to new root, make its left our old root.
                        }
                        else
                        {
                            (currentNode as ExpTreeOperatorNode).RightChild = nextValueNode;        // Set left side of operator to the front of value stack ( value may still be variable names at this point )
                        }
                    }
                    else
                    {
                        (currentNode as ExpTreeOperatorNode).RightChild = nextValueNode;            // Set left side of operator to the front of value stack ( value may still be variable names at this point )
                    }
                }
                
                currentNode = (currentNode as ExpTreeOperatorNode).RightChild;
            }
            return currentRoot;
        }

        public void SetVar(string varName, double varValue)                                         // Creates a new dictionary entry for a variable, or overrides an existing entry
        {
            if (variables.ContainsKey(varName))
            {
                variables[varName] = varValue;
            }
            else
            {
                variables.Add(varName, varValue);
            }
        }

        public double GetVarValue(string varName)
        {
            if (variables.ContainsKey(varName))
            {
                return variables[varName];
            }
            else return 0.0;
        }

        public double Eval()
        {
            treeValue = 0.0;                                                                        // Set the treeValue back to zero before we calculate value ( could already have value from an earlier evaluation )

            treeValue = EvaluateOperators(rootNode);                                                // Get the value from the recursive evaluation function

            return treeValue;
        }

        private double EvaluateOperators(ExpTreeNode rootNode)                                      // Recursive evaluation function
        {
            double value = 0.0;                                                                     // Local value, contains the value of the operation involving only two adjacent nodes                                                              

            if (isOperatorNode(rootNode))
            {
                switch ((rootNode as ExpTreeOperatorNode).Operator)
                {
                    case '+':
                        value = (rootNode as ExpTreeOperatorNode).RightChild.Evaluate() + (rootNode as ExpTreeOperatorNode).LeftChild.Evaluate();
                        break;

                    case '-':
                        value = (rootNode as ExpTreeOperatorNode).RightChild.Evaluate() - (rootNode as ExpTreeOperatorNode).LeftChild.Evaluate();
                        break;

                    case '*':
                        value = (rootNode as ExpTreeOperatorNode).RightChild.Evaluate() * (rootNode as ExpTreeOperatorNode).LeftChild.Evaluate();
                        break;

                    case '/':
                        value = (rootNode as ExpTreeOperatorNode).RightChild.Evaluate() / (rootNode as ExpTreeOperatorNode).LeftChild.Evaluate();
                        break;
                }
            }
            else
            {
                value = rootNode.Evaluate();
            }

            return value;                                                                           // return the final value, will be called after all recursive steps have returned
        }

        #endregion

        #region Helper Functions

        public ExpTreeNode CreateNode(string expression)
        {
            double result = 0.0;

            if (double.TryParse(expression, out result))
            {
                return new ExpTreeConstantNode(expression);
            }
            else
            {
                if (expression == "+" || expression == "-" || expression == "*" || expression == "/")
                {
                    return new ExpTreeOperatorNode(expression);
                }
                else
                {
                    return new ExpTreeVariableNode(expression, ref variables);
                }
            }
        }

        private int CountHighOperators(string expression)
        {
            int count = 0;
            for (int i = 0; i < expression.Length; i++)
            {
                if (isHighOperator(expression[i]))
                {
                    count++;
                }
            }
            return count;
        }

        private int CountAllOperators(string expression)
        {
            int count = 0;
            for (int i = 0; i < expression.Length; i++)
            {
                if (isOperator(expression[i]))
                {
                    count++;
                }
            }
            return count;
        }

        private int FindLeftEntity(string expression, int index)
        {
            int leftIndex = 0, parentheses = 0;
            bool foundNumerical = false;

            for (int i = index - 1; i > -1; i--)
            {
                if (expression[i] == ')')
                {
                    parentheses++;
                }
                else if (expression[i] == '(')
                {
                    parentheses--;
                }
                if (!isMetaChar(expression[i]) || i == 0)
                {
                    if (parentheses == 0)
                    {
                        foundNumerical = true;
                        leftIndex = i;
                        break;
                    }
                }
            }

            if (foundNumerical)
            {
                if (leftIndex == 0)
                {
                    return leftIndex;
                }
                else
                {
                    for (int i = leftIndex - 1; i > -1; i--)
                    {
                        if (isNumerical(expression[i]))
                        {
                            leftIndex = i;
                        }
                        else
                            break;
                    }
                    return leftIndex;
                }
            }
            else return 0;
        }

        private int FindRightEntity(string expression, int index)
        {
            int rightIndex = 0, parentheses = 0;
            bool foundNumerical = false;

            for (int i = index + 1; i < expression.Length; i++)
            {
                if (expression[i] == '(')
                {
                    parentheses++;
                }
                else if (expression[i] == ')')
                {
                    parentheses--;
                }
                if (!isMetaChar(expression[i]) || i == expression.Length - 1)
                {
                    if (parentheses == 0)
                    {
                        foundNumerical = true;
                        rightIndex = i;
                        break;
                    }
                }
            }

            if (foundNumerical)
            {
                if (rightIndex == expression.Length - 1)
                {
                    return rightIndex;
                }
                else
                {
                    for (int i = rightIndex + 1; i < expression.Length - 1; i++)
                    {
                        if (isNumerical(expression[i]))
                        {
                            rightIndex = i;
                        }
                        else
                            break;
                    }
                    return rightIndex;
                }
            }
            else return 0;
        }

        private bool isNumerical(char character)
        {
            if (character >= '0' && character <= '9')
            {
                return true;
            }
            return false;
        }

        private bool isMetaChar(char character)
        {
            if (character == '(' || character == ')' || character == '+' || character == '-' || character == '*' || character == '/')
            {
                return true;
            }
            return false;
        }

        private bool isOperator(char character)
        {
            if (character == '+' || character == '-' || character == '*' || character == '/')
                return true;
            return false;
        }

        private bool isHighOperator(char character)
        {
            if (character == '*' || character == '/')
                return true;
            return false;
        }

        public void ClearVariables()
        {
            variables.Clear();
        }

        public string TrimExpression(string expression)
        {
            bool _continue = false;
            int parentheses = 0;

            if (expression[0] == '(')
            {
                
                _continue = true;
                parentheses = 1;
            }
            if (_continue)
            {
                for (int i = 1; i < expression.Length; i++)
                {
                    if (expression[i] == '(')
                    {
                        parentheses++;
                    }
                    else if (expression[i] == ')')
                    {
                        parentheses--;
                    }
                    if (parentheses == 0)
                    {
                        if (i == expression.Length - 1 && expression[i] == ')')
                        {
                            expression = expression.Substring(1);
                            expression = expression.Substring(0, expression.Length - 1);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return expression;
        }

        public List<string> PopulateOperatorPieces(string expression)
        {
            List<string> operatorPieces = new List<string>();

            for (int i = 0; i < expression.Length; i++)                                                         // For each character in the expression string
            {
                for (int j = 0; j < operators.Length; j++)                                                      // If that character matches something from our operators array, add it to the operator pieces list
                {
                    if (expression[i] == operators[j])
                    {
                        operatorPieces.Add(operators[j].ToString());
                        break;
                    }
                }
            }
            return operatorPieces;
        }

        public bool isOperatorNode(ExpTreeNode node)
        {
            if (node.GetType() == typeof(ExpTreeOperatorNode))
            {
                return true;
            }
            else return false;
        }

        public bool isConstantNode(ExpTreeNode node)
        {
            if (node.GetType() == typeof(ExpTreeConstantNode))
            {
                return true;
            }
            else return false;
        }

        public bool isVariableNode(ExpTreeNode node)
        {
            if (node.GetType() == typeof(ExpTreeVariableNode))
            {
                return true;
            }
            else return false;
        }

        public bool isSubTree(ExpTreeNode node)
        {
            string name = (node as ExpTreeVariableNode).VariableName;
            if (name == "$")
            { return true; }
            return false;
        }

        public string RemoveWhiteSpace(string expression)
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

        public bool moreParenthesis(string expression)
        {
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '(' || expression[i] == ')')
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}

