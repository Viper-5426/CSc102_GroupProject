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

namespace Concordancer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string loadedText = "";
        string cleanedText = "";
        string[] depunctuatedText;  //cleanedText split into an array and stripped of any null and empty items
        string[] punctuatedText; //loadedText split into array and stripped of null and empty items


        public MainWindow()
        {
            InitializeComponent();
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

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            string sString = System.IO.File.ReadAllText(@txtLocation.Text);
            loadedText = sString; 
            MessageBox.Show("Your file has been loaded!", "Success");

        }

        private string removePunctuation(string sString)
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
        private string[] stripEmptyAndNull(string[] text)
        {
            List<string> result; //totalWords stripped of all empty strings and null items
            result = text.ToList();
            result.RemoveAll(p => string.IsNullOrEmpty(p));
            return result.ToArray();
        }
        private int[] findMatches(string[] depunctuatedText, string searchTerm)
        {
            List<int> result = new List<int>();
            int i = 0;
            while (i < depunctuatedText.Length)
            {
                if (searchTerm.CompareTo(depunctuatedText[i]) == 0)
                {
                    result.Add(i);
                }
                i++;
            }
            return result.ToArray();
        }

        private void btnFrequency_Click(object sender, RoutedEventArgs e)
        {
            //TODO: must take the loadedText and send it to frequency counter
            //what the frequency counter returns must be shown in the txtFreqList in an orderly fashion
            cleanedText = removePunctuation(loadedText.ToLower());  //uses the efficient method to display the book without punctuation
            depunctuatedText = cleanedText.Split();
            depunctuatedText = stripEmptyAndNull(depunctuatedText);
            Dictionary<string, int> wordList = frequencyCounter(depunctuatedText);
            var myList = wordList.ToList();

            myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            foreach (KeyValuePair<string, int> k in myList)
            {
                string tabs = "\t";
                if (k.Key.Length < 8)
                {
                    tabs += "\t";
                }
                txtFreqList.Text += string.Format("{0} {1} has {2} occurances", k.Key, tabs, k.Value);
                txtFreqList.Text += "\n";
            }

        }
        private Dictionary<string, int> frequencyCounter(string[] textArray)
        {
            //TODO: needs to sort all items in the array
            //then count occurrence of each unique item (maybe use dictionary and return it instead of an array of arrays?)
            //needs to associate a count with each new word it encounters
            Dictionary<string, int> wordCounts = new Dictionary<string, int>();
            List<string> textList; //totalWords stripped of all empty strings and null items
            textList = textArray.ToList();
            while (textList.Count > 0)
            {
                int i = 0;
                string first = textList[0];
                int count = 0;
                while (i < textList.Count)
                {
                    if (first == textList[i])
                    {
                        count++;
                        textList.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
                wordCounts[first] = count;
            }

            return wordCounts;
        }

        private void btnMakeConcordance_Click(object sender, RoutedEventArgs e)
        {
            txtConcordanceLines.Text = "";
            int range = Convert.ToInt32(sliWindowRange.Value);
            string searchTerm = txtSearchTerm.Text;
            punctuatedText = loadedText.Split();
            punctuatedText = stripEmptyAndNull(punctuatedText);
            txtLength.Text = "" + depunctuatedText.Length;
            txtLength.Text += "\n" + punctuatedText.Length;
            int[] indexes = findMatches(depunctuatedText, searchTerm);
            foreach (int index in indexes)
            {
                if ((index - range) > 0 && (index + range+1) < punctuatedText.Length)
                {
                    for (int r = index - range; r < range + index+1; r++)
                    {
                        //txtConcordanceLines.Text += punctuatedText[r] + " ";
                        txtConcordanceLines.Text += depunctuatedText[r] + " ";

                    }
                }
                else if ((index - range) < 0)
                {
                    for (int r = 0; r < range + index; r++)
                    {
                        //txtConcordanceLines.Text += punctuatedText[r] + " ";
                        txtConcordanceLines.Text += depunctuatedText[r] + " ";
                    }
                }
                else if ((index + range) > punctuatedText.Length)
                {
                    for (int r = index - range; r < punctuatedText.Length; r++)
                    {
                        //txtConcordanceLines.Text += punctuatedText[r] + " ";
                        txtConcordanceLines.Text += depunctuatedText[r] + " ";
                    }
                }
                txtConcordanceLines.Text += "\n";
            }
            
            //int i = 0;

            ////searches through the unsorted, depunctuated text to find words matching to input
            ////then it prints the range of words to the left and right of the input
            //while (i < depunctuatedText.Length)
            //{
            //    if (searchTerm.CompareTo(depunctuatedText[i]) == 0)
            //    {
            //        try
            //        {
            //            for (int n = i-range; n < i + range; n++)
            //            {
            //                if (searchTerm.CompareTo(depunctuatedText[n]) == 0)
            //                {
            //                    txtConcordanceLines.Text += punctuatedText[n].ToUpper() + " ";
            //                }
            //                else
            //                {
            //                    txtConcordanceLines.Text += punctuatedText[n] + " ";
            //                }
                            

            //            }
            //            txtConcordanceLines.Text += "\n";
            //        }
            //        catch (Exception E)
            //        {

            //        }
            //    }
            //    i++;
            //}
        }
    }
}
