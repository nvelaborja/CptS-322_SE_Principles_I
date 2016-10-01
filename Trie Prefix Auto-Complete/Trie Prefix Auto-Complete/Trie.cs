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
using System.IO;

namespace Trie_Prefix_Auto_Complete
{
    class Trie
    {
        #region Class Members

        private TrieNode rootNode;                                      // Root node for the entire trie
        private TrieNode currentNode;                                   // Moving node used for navigation purposes
        private string[] dictionary;                                    // Massive dictionary loaded from dictionary file used to populate try
        private bool error;                                             // Boolean value that signifies if dictionary file loading was successful
        private List<string> suggestions;                               // List of suggestions for the given word in the input box, will constantly change
        private string currentWord;                                     // Current word in the input box, will constantly change

        #endregion

        #region Constructors

        public Trie()                                                   // Constructor for the Trie, no need for parameters
        {
            rootNode = new TrieNode();                                  // Create new node for the root,
            rootNode.IsRoot = true;                                     // Set it's IsRoot parameter to true. Didn't end up doing anything with this, but it could be used in future expansion
            error = false;                                              // Initialize error to be false
            suggestions = new List<string>();                           // Initialize suggestions list
            LoadDictionary();                                           // Call the load dicionary function, will populate trie if succeeds
            currentWord = "";                                           // Initalize currentWord to the empty string
            
        }

        #endregion

        #region Properties

        public bool Error                                              
        {
            get { return error; }
        }

        public string[] Suggestions
        {
            get { return suggestions.ToArray(); }
        }

        #endregion

        #region Main Logic Functions

        private void LoadDictionary()                                   // Loads dictionary text file. If it does this successfully, it forwards to the PopulateTrie() function
        {
            string filePath = @"wordsEn.txt";                           // Text file is located in the main project directory, so this filePath should always work

            if (File.Exists(filePath))                                  // Check to make sure the file exists. If so,
            {
                dictionary = File.ReadAllLines(filePath);               // Populate the dictionary string array from the files lines
                PopulateTrie();                                         // Populate the Trie
                return;
            }
            else
            {
                error = true;                                           // If the file load failed, set error to true. This will prevent the form from doing anything if true.
                return;
            }
        }

        
        private void PopulateTrie()                                     // Forms the Trie based on the dictionary array. Must be called after file is loaded and dictionary full
        {
            foreach (string word in dictionary)                         // For every word in the dictionary array,
            {
                currentNode = rootNode;                                 // Start our current node at the root
                string currentWord = "";                                // Change our current word back to empty string

                foreach(char character in word)                         // For every character in the given word,
                {
                    currentWord += character;                           // Add it to our current word
                    currentNode = currentNode.PushChar(character);      // Push it into our current node. This will create a new node if it doesn't exist as the current node's child
                }

                currentNode.PushChar('\0');                             // After the word is pushed, push a null character to signify it is a finished word
                currentNode.Word = currentWord;                         // Set the current node's word to the current word, since at this point the currentNode will be a null character node
            }
        }

        public void GetSuggestions(string word)                         // The first part of the function to get the suggestions for the given word. Originally broken in two functions for threading, but now I don't feel like doing the threading...
        {
            suggestions.Clear();                                        // First clear the current suggestions so we don't get multiples

            if (word == "") return;                                     // If the word is empty, return immediately or we'll crash

            currentNode = rootNode;                                     // Set our current node to the root
            currentWord = word;                                         // Set our current word to the given word
            suggestions.Add(currentWord);                               // Suggestion list always starts with the currently typed word

            foreach (char character in currentWord)                     // Navigate to try node at the end of the given string
            {
                if (currentNode != null)
                {
                    currentNode = currentNode.NodeFromChar(character);
                }
                else break;
            }
            
            if (currentNode != null)                                    // At this point the currentNode has trickled down to wherever it is in the Trie, make sure it isn't null, otherwise there are no suggestions!
            {
                GetSubSuggestions();                                    // If not, continue to the second part of this functionality
            }
        }

        private void GetSubSuggestions()                                // The second part of the function to get the suggestions for the given word, built to be able to be done by a thread
        {
            Queue<TrieNode> nodeQueue = new Queue<TrieNode>();          // Create a queue of nodes
            nodeQueue.Enqueue(currentNode);                             // Start by giving it our current node

            while (nodeQueue.Count > 0)                                 // Until we've gone through all nodes,
            {
                currentNode = nodeQueue.Dequeue();                      // Dequeue and set our current node to it (redundant for the first iteration)

                if (currentNode.Word != null && currentNode.Word != currentWord.ToLower())  // If the current node has a word ( meaning its the end of valid word ) and it doesn't match our given word, add it to the suggestion list!
                {
                    if (IsCapitalized(currentWord))                     // Just for looks, capitalze the word if the given word was also capitalized
                    {
                        suggestions.Add(Capitalize(currentNode.Word));
                    }
                    else suggestions.Add(currentNode.Word);
                }

                foreach (TrieNode node in currentNode.PresentChildren)  // Finally, add all of the current node's non-null children to the queue
                {
                    nodeQueue.Enqueue(node);
                }
            }
        }

        #endregion

        #region Helper Functions

        public void ClearSuggestions()
        {
            this.suggestions.Clear();
        }

        private bool IsCapitalized(string word)                         // Determines if the given word is capitalized. Assumes it's alphabetical
        {
            if (word[0] <= 'Z')
            {
                return true;
            }

            return false;
        }

        private string Capitalize(string word)                          // Capitalizes the given word if it's not already
        {
            if (word[0] > 'Z')
            {
                string capitalized = word[0].ToString().ToUpper() + word.Substring(1);
                return capitalized;
            }
            return word;
        }

        #endregion
    }
}
