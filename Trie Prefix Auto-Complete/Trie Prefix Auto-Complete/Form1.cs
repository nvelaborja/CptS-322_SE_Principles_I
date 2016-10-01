/********************************************************************************************\
* Name: Nathan VelaBorja
* Date: November 19, 2015
* Class: CptS 322 - Fall 2015
* Assignment: 13 - Trie Prefix Auto-Complete
* Description: This program takes in real-time input from a text box and uses a trie to come up
*       with a list of autocomplete suggestions.
* NOTE TO GRADER: I noticed in Evan's instructions, his results were sorted alphabetically.
*       I decided I did not want to do this and instead prioritize word length and then by
*       by alphabet. Although he didn't specify that it was to be done either way, I have a
*       feeling I could be marked down for this. I just wanted to let you know that it was 
*       intentional. Thanks.
* EXTRA FEATURES: 
*   - Result words become capitalized if the input word was capitalized (doesn't interfere with
*       word equality checks).
*   - Added support for TAB and ENTER commands so it's somewhat like a real auto-complete module
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

namespace Trie_Prefix_Auto_Complete
{
    public partial class Form1 : Form
    {
        #region Class Members

        private Trie trie;                                              // Trie that will be used to create suggestions

        #endregion

        #region Constructors

        public Form1()  
        {
            InitializeComponent();

            trie = new Trie();                                          // Initialize the trie
            if (trie.Error) listBox.Items.Add("Error Loading Dictionary File"); // If it had an error loading, set the listBox text to an error message
        }

        #endregion

        #region Event Functions

        private void entryBox_TextChanged(object sender, EventArgs e)   // Event function calls when the entryBox text changes
        {
            if (!trie.Error)                                            // If the trie didn't have a load error,
            {
                listBox.Items.Clear();                                  // Clear the list box

                trie.GetSuggestions(entryBox.Text);                     // Get the suggestions given the text in the entryBox

                listBox.Items.AddRange(trie.Suggestions);               // Add the trie's newly created suggestions into the list box
            }
        }

        private void listBox_Enter(object sender, EventArgs e)          // Event function calls when the focus switches to the list box ( usually pressing tab from the entry box )
        {
            if (listBox.Items.Count > 1)                                // If the box has more than one entries,
            {
                listBox.SelectedItem = listBox.Items[1];                // Select the second item since the first one is what the user has already typed in
            }
            else if (listBox.Items.Count == 1)                          // Otherwise, if the only thing in the box is what the user has typed,
            {
                listBox.SelectedItem = listBox.Items[0];                // Select it
            }
        }

        private void listBox_KeyPress(object sender, KeyPressEventArgs e)   // Event Function calls when a key is pressed in the list box
        {
            if (e.KeyChar == (char)Keys.Enter)                              // If it was the enter key,
            {
                if (listBox.Items.Count > 1)                                // and if the box has more than one items
                {
                    entryBox.Text = listBox.SelectedItem.ToString();        // Copy the selected item into the entry box
                    entryBox.Select();                                      // Then put the focus back on the entry box
                    entryBox.SelectionStart = entryBox.Text.Length;         // and put the cursor back at the end of the string ( otherwise it will highlight the whole string )
                }
                else                                                        // If the box only has one or zero things in it
                {
                    entryBox.Select();                                      // Do the same as above but don't copy anything
                    entryBox.SelectionStart = entryBox.Text.Length;
                }
            }
        }

        #endregion
    }
}
