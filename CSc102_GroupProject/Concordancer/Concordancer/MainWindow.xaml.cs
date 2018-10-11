using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;

namespace Concordancer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string loadedText = "";
        //string cleanedText = "";
        List<String> lstDepunctuated = new List<string>();  //cleanedText list
        string[] punctuatedText; //loadedText split into array and stripped of null and empty items
        Dictionary<string, int> collocateList = new Dictionary<string, int>();  //dictionary that counts all the entries of collocates



        public MainWindow()
        {
            InitializeComponent();
            tabConcordanceLines.Visibility = Visibility.Hidden;  //only allows the user to use collocates and concordancer tabs once they have counted frequencies
            tabCollocates.Visibility = Visibility.Hidden;  //same over here
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TEXT Files | *.txt";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                txtLocation.Text = filename;
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e) //just loads the text into a manageable variable
        {
            loadedText = System.IO.File.ReadAllText(@txtLocation.Text); //reads all the text in the file
            //cleanedText = removePunctuation(loadedText.ToLower());  //uses the efficient method to display the book without punctuation or numbers

            MessageBox.Show("Your file has been loaded!", "Success");  //just so you know we were successful :)

        }

        private string removePunctuation(string sString)  //does all the cleaning
        {
            StringBuilder newString = new StringBuilder("");   //instantiates a stringBuilder object that is mutable, can be changed
            foreach (char c in sString)  //loops through each character in the book
            {
                if (!char.IsPunctuation(c))  //checks whether it is punctuation
                {
                    newString.Append(c);  //appends all the letters and spaces to the stringBuilder object 
                }
            }
            return newString.ToString();
        }
        private string[] stripEmptyAndNull(string[] text)  //we want to get rid of all the empty and null things, right? They aren't words...
        {
            List<string> result; //result will be stripped of all empty strings and null items
            result = text.ToList();
            result.RemoveAll(p => string.IsNullOrEmpty(p));  //performs the IsNullOrEmpty method on all items in the list
            return result.ToArray();
        }
        private int[] findMatches(string[] stringList, string searchTerm)  //returns the indexes of where the search term is found within the original array
        {
            List<int> result = new List<int>();
            int i = 0;
            while (i < stringList.Length)  //loops through each item by index
            {
                if (searchTerm.CompareTo(stringList[i]) == 0)
                {
                    result.Add(i);  //adds the index if it matches with the searchterm
                }
                i++;
            }
            return result.ToArray();
        }

        private void btnFrequency_Click(object sender, RoutedEventArgs e)
        {
            //takes the loadedText and send it to frequency counter
            //what the frequency counter returns must be shown in the txtFreqList in an orderly fashion
            //also shows other information about the text as a whole in txtLength
            if (loadedText == "")
            {
                MessageBox.Show("Please load a text first!", "Error: No text loaded."); //some small error checking
            }
            else
            {
                punctuatedText = loadedText.Split();
                punctuatedText = stripEmptyAndNull(punctuatedText);
                string textLocation = txtLocation.Text;
                int index = 0, count = 0;

                int i = 0;
                foreach (string s in punctuatedText)
                {
                    lstDepunctuated.Add(removePunctuation(s.ToLower()));
                    i++;
                }
                Dictionary<string, int> wordList = frequencyCounter(lstDepunctuated.ToArray());
                var myList = wordList.ToList();

                myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value)); //Link syntax

                string result = "Freq.\tWord\n\n";

                foreach (string k in wordList.Keys)
                {
                    result += String.Format("{0} --\t {1}\n", wordList[k], k);
                }
                txtFreqList.Text = result;

                foreach (char c in textLocation)
                {
                    count++;
                    if (c == Convert.ToChar(@"\"))
                    {
                        index = count;
                    }
                }
                textLocation = "";
                for (int t = index; t < txtLocation.Text.Length; t++)
                {
                    textLocation += txtLocation.Text[t];
                }

                txtLength.Text = "Loaded text: " + "\n" + textLocation;
                txtLength.Text += "\n\n" + "Wordcount: " + "\n\n" + lstDepunctuated.Count;
                txtLength.Text += "\n\n" + "Total Unique Words:" + "\n\n" + wordList.Count();

                tabConcordanceLines.Visibility = Visibility.Visible;
                tabCollocates.Visibility = Visibility.Visible;
            }
        }  //does all the handling of when the count frequency button is clicked
        private Dictionary<string, int> frequencyCounter(string[] textArray)  //counts the frequency of all words in given array using a dictionary
        {
            //TODO: needs to sort all items in the array
            //then count occurrence of each unique item (maybe use dictionary and return it instead of an array of arrays?)
            //needs to associate a count with each new word it encounters
            Dictionary<string, int> wordCounts = new Dictionary<string, int>();
            List<string> textList; //totalWords stripped of all empty strings and null items
            textList = textArray.ToList();

            foreach (string word in textList)
            {
                if (wordCounts.ContainsKey(word))
                {
                    wordCounts[word]++;  //if the word has already been added to the dictionary, it needs to increase in count
                }
                else
                {
                    wordCounts[word] = 1;  //if the word is new, make its count 1
                }
            }

            return wordCounts;
        }

        private void btnMakeConcordance_Click(object sender, RoutedEventArgs e)  //does all the things related to the concordance button
        {
            //shows a selected number of words to the left and right of the search term
            txtConcordanceLines.Text = "";
            int range = Convert.ToInt32(sliWindowRange.Value);  //reads in selected number from slider
            string searchTerm = txtSearchTerm.Text;
            string result = "";


            int[] indexes = findMatches(lstDepunctuated.ToArray(), searchTerm.ToLower());  //returns indexes of search term

            foreach (int index in indexes)  // for each occurance of search term
            {
                if ((index - range) > 0 && (index + range + 1) < punctuatedText.Length)   //if it's not at the edges of the text
                {
                    for (int r = index - range; r < range + index + 1; r++)  //for selected range to the left and to the right - print
                    {
                        if (r == index)
                        {
                            result += punctuatedText[r].ToUpper() + " "; //uppercase the search term
                        }
                        else
                        {
                            result += punctuatedText[r] + " ";

                        }

                    }
                }
                else if ((index - range) < 0)  //if its at the very beginning of the text
                {
                    for (int r = 0; r < range + index; r++)
                    {
                        if (r == index)
                        {
                            result += punctuatedText[r].ToUpper() + " ";
                        }
                        else
                        {
                            result += punctuatedText[r] + " ";
                        }
                    }
                }
                else if ((index + range) > punctuatedText.Length)  //if its at the very end of the text
                {
                    for (int r = index - range; r < punctuatedText.Length; r++)
                    {
                        if (r == index)
                        {
                            result += punctuatedText[r].ToUpper() + " ";
                        }
                        else
                        {
                            result += punctuatedText[r] + " ";
                        }
                    }
                }
                result += "\n";
                lblOccurrences.Content = String.Format("Found {0} occurrences of '{1}'", indexes.Length, searchTerm); //label showing No. of occurances of search term
            }
            txtConcordanceLines.Text = result;

        }

        private void btnSortCountFreq_Click(object sender, RoutedEventArgs e)
        {

            string Text = "Freq.\tWord\n\n";
            Dictionary<string, int> wordList = frequencyCounter(lstDepunctuated.ToArray());
            var myList = wordList.ToList();

            myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value)); //Linq syntax
            foreach (KeyValuePair<string, int> k in myList)
            {
                string tabs = "\t";
                if (k.Key.Length < 8)
                {
                    tabs += "\t";
                }
                Text += string.Format("{0} --\t{1}", k.Value, k.Key);
                Text += "\n";
            }
            txtFreqList.Text = Text;
        }

        private void btnSortCountAlph_Click(object sender, RoutedEventArgs e)
        {
            string Text = "Freq.\tWord\n\n";
            Dictionary<string, int> wordList = frequencyCounter(lstDepunctuated.ToArray());
            var myList = wordList.ToList();

            myList.Sort((pair1, pair2) => pair2.Key.CompareTo(pair1.Key)); //LINQ, allows us to perform a function on each pair in this list
            foreach (KeyValuePair<string, int> k in myList)
            {
                string tabs = "\t";
                if (k.Key.Length < 8)
                {
                    tabs += "\t"; //neatly prints out all the things
                }
                Text += string.Format("{0} --\t{1}", k.Value, k.Key);
                Text += "\n";
            }
            txtFreqList.Text = Text;
        }

        private void btnListCollocates_Click(object sender, RoutedEventArgs e)
        {
            //combines aspects of the frequency count and the concordancer in order to 
            //count the words within a given range around each occurrence of the search term
            txtCollocates.Text = "";
            string searchTerm = txtSearchCollocates.Text;
            int[] indexes = findMatches(lstDepunctuated.ToArray(), searchTerm.ToLower());
            List<string> surroundingWds = new List<string>();
            int range = Convert.ToInt32(sliWindowRangeColl.Value);
 
            foreach (int index in indexes)
            {
                if ((index - range) > 0 && (index + range + 1) < lstDepunctuated.Count) //if not at the edges of the text
                {
                    for (int r = index - range; r < range + index + 1; r++)
                    {
                        if (r != index)
                        {
                            surroundingWds.Add(lstDepunctuated[r]);  //adds words to the surroundingWds list
                        }

                    }
                }
                else if ((index - range) < 0)
                {
                    for (int r = 0; r < range + index; r++)
                    {
                        if (r == index)
                        {
                            surroundingWds.Add(lstDepunctuated[r]);
                        }
                    }
                }
                else if ((index + range) > punctuatedText.Length)
                {
                    for (int r = index - range; r < punctuatedText.Length; r++)
                    {
                        if (r == index)
                        {
                            surroundingWds.Add(lstDepunctuated[r]);
                        }
                    }
                }

            }
            collocateList = frequencyCounter(surroundingWds.ToArray());  //global variable
            string result = "Freq.\tCollocate\n\n";  //for printing

            foreach (string k in collocateList.Keys)
            {
                result += String.Format("{0} --\t {1}\n", collocateList[k], k);
            }
            txtCollocates.Text = result;
            lblCountCollocates.Content = String.Format("Listing {0} collocates of '{1}'", surroundingWds.Count(), searchTerm);
        }

        private void btnSortFreq_Click(object sender, RoutedEventArgs e)  //sorts by frequency
        {
            string Text = "Freq.\tCollocate\n\n";
            var myList = collocateList.ToList();

            myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value)); //LINQ syntax
            foreach (KeyValuePair<string, int> k in myList)
            {
                string tabs = "\t";
                if (k.Key.Length < 8)
                {
                    tabs += "\t";
                }
                Text += string.Format("{0} --\t{1}", k.Value, k.Key);
                Text += "\n";
            }
            txtCollocates.Text = Text;
        }

        private void btnSortAlph_Click(object sender, RoutedEventArgs e)
        {
            string Text = "Freq.\tCollocate\n\n";
            var myList = collocateList.ToList();

            myList.Sort((pair1, pair2) => pair2.Key.CompareTo(pair1.Key)); //Link syntax
            foreach (KeyValuePair<string, int> k in myList)
            {
                string tabs = "\t";
                if (k.Key.Length < 8)
                {
                    tabs += "\t";
                }
                Text += string.Format("{0} --\t{1}", k.Value, k.Key);
                Text += "\n";
            }
            txtCollocates.Text = Text;
        }

        public void saveThingsToDirectory(string name, string[] splitted)
        {
            string directory = @"D:\Concordancer Results\";
            string x = System.IO.Path.Combine(directory, name);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            for (int i = 0; i < splitted.Length; i++)
            {
                File.WriteAllLines(x, splitted);

            }
        }  //selects or creates a directory and writes a file inside

        private void btnSaveFrequency_Click(object sender, RoutedEventArgs e)
        {
            string[] splitted = txtFreqList.Text.Split('\n');   //splits the text on all newline characters
            saveThingsToDirectory("Freq_Results.txt", splitted);


        }

        private void btnSaveConcordance_Click(object sender, RoutedEventArgs e)
		{
			string[] CoNsplitted = txtConcordanceLines.Text.Split('\n');   //splits the text on all newline characters
            saveThingsToDirectory("Concord_Results.txt", CoNsplitted);
		}

		private void btnSaveCollocates_Click(object sender, RoutedEventArgs e)
		{
			string[] CoLsplitted = txtCollocates.Text.Split('\n');  //splits the text on all newline characters
            saveThingsToDirectory("Coll_Results.txt", CoLsplitted);
		}
        

		private void btnHelp_Click(object sender, RoutedEventArgs e)
		{
            string helpText = 
                @""+"What is a Concordancer?\n"+"A concordancer is a computer program that automatically constructs a concordance. \n"+"The output of a concordancer may " +
                "serve as input to a translation memory system for computer-assisted translation, or as an early step in machine translation.\nhttps://en.wikipedia.org/wiki/Concordancer \n\n" +
                "What is a concordance? \n" +
                "A concordance is an alphabetical list of the principal words used in a book or " +
                "body of work, listing every instance of each word with its immediate context.\n " +
                "https://en.wikipedia.org/wiki/Concordance_(publishing) \n\n" +
                "What is a Collocate?\n" +
                "In linguistics, a collocate of a particular word is another word which often occurs with that word.\n"
                + "https://en.wikipedia.org/wiki/Collocation \n\n" +
                "Copy the links into a browser for more information.";
            MessageBox.Show(helpText, "Here is some advice");
        }

        private void sliWindowRange_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (Convert.ToInt32(sliWindowRange.Value) == -1)
                {
                    txtRange.Text = "0";
                }
                else
                {
                    txtRange.Text = Convert.ToString(Convert.ToInt32(sliWindowRange.Value));
                }

            }
            catch(Exception E)
            {

            }
        }

        private void sliWindowRangeColl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)  //displays the change of slider in a label
        {
            try
            {
                if (Convert.ToInt32(sliWindowRangeColl.Value) == 0)
                {
                    txtCollocateRange.Text = "" + Convert.ToInt32(sliWindowRangeColl.Value);
                }
                else
                {
                    txtCollocateRange.Text = Convert.ToString(Convert.ToInt32(sliWindowRangeColl.Value));
                }

            }
            catch (Exception E)
            {

            }
        }
    }
}
