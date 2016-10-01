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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trie_Prefix_Auto_Complete
{
    class TrieNode
    {
        // Nodes must have a character value and a list of potentially 27 children, 
        //  one for each character of the alphabet plus one for null character

        #region Class Members

        private char value;                         // Character value of the node
        private TrieNode[] children;                // Array of children, A-Z populates indices 0-25 respectively, and null character gets index 26
        private bool isRoot;                        // Determines whether or not the node is the trie's root
        private int numChildren;                    // Counts how many children (not including the null character) the node has
        private string word;                        // Null nodes will keep track of the word leading to them, speeds up UI quite a bit

        #endregion

        #region Constructors

        public TrieNode()                           // Empty constructor, will signify creating a null character node
        {
            this.value = '\0';
            children = new TrieNode[27];
            isRoot = false;
        }

        public TrieNode(char character)             // Constructor that takes in a character value
        {
            this.value = character;
            children = new TrieNode[27];
            isRoot = false;
        }

        #endregion

        #region Properties

        public char Character                       // Returns node value as a character, read-only
        {
            get { return this.value; }
        }

        public string String
        {
            get { return this.value.ToString(); }   // Returns node value as a string, read-only
        }

        public TrieNode[] Children                  // Returns entire array of children, read-only
        {
            get { return this.children; }
        }

        public TrieNode[] PresentChildren           // Returns new array of only non-null children, read-only
        {
            get
            {
                List<TrieNode> presentChildren = new List<TrieNode>();

                foreach (TrieNode child in this.children)
                {
                    if (child != null)
                    {
                        presentChildren.Add(child);
                    }
                }

                return presentChildren.ToArray();
            }
        }

        public bool IsRoot                          // Properties controls the bool isRoot. 
        {
            get { return isRoot; }
            set { isRoot = value; }
        }

        public int NumChildren                      // Propertie controls the int numChildren, the number of the children the node has
        {
            get { return numChildren; }
            set { numChildren = value; }
        }

        public bool HasNull                         // Determines if the node has a null character node as a child, read only
        {
            get
            {
                if (children[26] == null)
                {
                    return false;
                }

                return true;
            }
        }

        public string Word                          // Controls the string word
        {
            get { return word; }
            set { word = value; }
        }


        #endregion

        #region Helper Functions

        public TrieNode PushChar(char character)    // Takes a character and creates a child out of it
        {
            int index = IndexFromChar(character);   // Get the index of where the child belongs

            if (children[index] == null)            // If there isn't already a child there,
            {
                TrieNode newNode = new TrieNode(character); // Create a new one with the given character

                this.children[index] = newNode;     // Set it as the current node's child

                return this.children[index];        // Return it
            }

            return this.children[index];
        }

        public bool HasChar(char character)         // Retermines if the node has a child of the given character or not
        {
            if (NodeFromChar(character) == null)
            {
                return false;
            }

            return true;
        }

       
        public int IndexFromChar(char character)    // Returns the array index based on the given character
        {
            character = Convert.ToChar(character.ToString().ToUpper());     // Ugly, but converts character to upper case, just in case I do something dumb while programming this

            if (character >= 'A' && character <= 'Z')
            {
                return (int)(character - 'A');
            }
            else                                                            // If not A-Z, must be null character, return 26 
            {
                return 26;
            }
        }

        public TrieNode NodeFromChar(char character)    // Returns the node 
        {
            int index = IndexFromChar(character);

            return children[index];
        }

        #endregion
    }
}
