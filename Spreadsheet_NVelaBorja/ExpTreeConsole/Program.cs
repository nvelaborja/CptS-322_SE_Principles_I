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
using SpreadsheetEngine;

namespace CptS322
{
    /********************************************************************************************\
     * Class: Program
     * Description: This class is a console application that contains a simple menu that showcases
     *      the ExpTree functionality to the user
    \********************************************************************************************/
    class Program
    {
        static void Main(string[] args)
        {
            #region Variables

            ExpTree eTree = new ExpTree("");                                                                    // Create new expression tree, initially with no expression
            string userInput = "";                                                                              // User input from the menu interface
            string expression = "";                                                                             // Expression given from the user in the menu interface
            double expressionValue = 0.0;                                                                       // Expression value returned from eTree's evaluation function
            string variableName = "";                                                                           // Variable name used in variable creation in the menu interface
            double variableValue = 0.0;                                                                         // Variable value used in the variable creation in the menu interface
            bool quit = false;                                                                                  // Boolean used to exit the program if the user specifies intent in the menu interface
            short menuChoice = 0;                                                                               // Integer used to navigate menu interface

            #endregion

            #region Console Setup

            Console.SetWindowSize(100, 40);                                                                     // Set console window size to a relatively small window ( fits menu well )
            Console.Title = "VelaBorja's Expression Tree Console";                                              // Set console window title to program title
            Console.WriteLine("Welcome to VelaBorja's Expression Tree Console!" + Environment.NewLine);         // Print a welcome message at the top of the console

            #endregion

            #region Menu Loop

            while (!quit)
            {
                //Console.Clear();                                                                              // Makes menu much cleaner, but much easier to grade with it off!
                
                if(expression == "")                                                                            // If the expression is empty, set the expression text to "[empty]" so the user can clearly see nothing has been entered yet
                {
                    expression = "[empty]";
                }

                Console.WriteLine("Current Expression: " + expression + Environment.NewLine);                   // Print out current expression and menu options
                Console.WriteLine("Menu: " + Environment.NewLine);
                Console.WriteLine("\t1. Enter a new expression" + Environment.NewLine);
                Console.WriteLine("\t2. Set a variable value" + Environment.NewLine);
                Console.WriteLine("\t3. Evaluate Tree" + Environment.NewLine);
                Console.WriteLine("\t4. Quit" + Environment.NewLine);
                Console.Write(": ");
                userInput = Console.ReadLine();                                                                 // Get user selection for menu navigation

                if (Int16.TryParse(userInput, out menuChoice))
                {
                    switch (menuChoice)
                    {
                        case 1:                                                                                 // If user entered 1, prompt for an expression and set the tree's expression to given value
                            eTree.ClearVariables();
                            Console.Write(": Enter Expression: ");
                            expression = Console.ReadLine();
                            eTree.Expression = expression;
                            Console.Write(Environment.NewLine);
                            continue;
                        case 2:                                                                                 // If user entered 2, prompt for variable name and value, then add entry to the variable dictionary
                            Console.Write(": Enter Variable Name: ");
                            variableName = Console.ReadLine();
                            Console.Write(": Enter Variable Value: ");
                            variableValue = Convert.ToDouble(Console.ReadLine());
                            eTree.SetVar(variableName, variableValue);
                            Console.Write(Environment.NewLine);
                            continue;
                        case 3:                                                                                 // If user entered 3, evaluate tree and print results
                            expressionValue = eTree.Eval();
                            Console.WriteLine(": Expression Value: " + expressionValue + Environment.NewLine);
                            continue;
                        case 4:                                                                                 // If user entered 4, set quit to true ( will then exit loop and program will end )
                            quit = true;
                            break;
                        default:                                                                                // If user entered something we weren't looking for, continue to next loop iteration
                            continue;
                    }
                }
                

                Console.Write(Environment.NewLine);
            }

            #endregion
        }
    }
}
