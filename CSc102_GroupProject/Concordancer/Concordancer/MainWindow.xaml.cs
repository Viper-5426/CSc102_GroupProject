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
        string cleanedText = "";
        List<String> lstDepunctuated = new List<string>();  //cleanedText list
        string[] punctuatedText; //loadedText split into array and stripped of null and empty items
        Dictionary<string, int> collocateList = new Dictionary<string, int>();



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
            cleanedText = removePunctuation(loadedText.ToLower());  //uses the efficient method to display the book without punctuation

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
            punctuatedText = loadedText.Split();
            punctuatedText = stripEmptyAndNull(punctuatedText);
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

        }
        private Dictionary<string, int> frequencyCounter(string[] textArray)
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
                    wordCounts[word]++;
                }
                else
                {
                    wordCounts[word] = 1;
                }
            }

            return wordCounts;
        }

        private void btnMakeConcordance_Click(object sender, RoutedEventArgs e)
        {
            txtConcordanceLines.Text = "";
            int range = Convert.ToInt32(sliWindowRange.Value);
            string searchTerm = txtSearchTerm.Text;
            txtLength.Text = "" + lstDepunctuated.Count;
            txtLength.Text += "\n" + punctuatedText.Length;



            int[] indexes = findMatches(lstDepunctuated.ToArray(), searchTerm.ToLower());

            foreach (int index in indexes)
            {
                if ((index - range) > 0 && (index + range + 1) < punctuatedText.Length)
                {
                    for (int r = index - range; r < range + index + 1; r++)
                    {
                        if (r == index)
                        {
                            txtConcordanceLines.Text += punctuatedText[r].ToUpper() + " ";
                        }
                        else
                        {
                            txtConcordanceLines.Text += punctuatedText[r] + " ";

                        }
                        //txtConcordanceLines.Text += depunctuatedText[r] + " ";

                    }
                }
                else if ((index - range) < 0)
                {
                    for (int r = 0; r < range + index; r++)
                    {
                        if (r == index)
                        {
                            txtConcordanceLines.Text += punctuatedText[r].ToUpper() + " ";
                        }
                        else
                        {
                            txtConcordanceLines.Text += punctuatedText[r] + " ";
                        }
                        //txtConcordanceLines.Text += depunctuatedText[r] + " ";
                    }
                }
                else if ((index + range) > punctuatedText.Length)
                {
                    for (int r = index - range; r < punctuatedText.Length; r++)
                    {
                        if (r == index)
                        {
                            txtConcordanceLines.Text += punctuatedText[r].ToUpper() + " ";
                        }
                        else
                        {
                            txtConcordanceLines.Text += punctuatedText[r] + " ";
                        }
                        //txtConcordanceLines.Text += depunctuatedText[r] + " ";
                    }
                }
                txtConcordanceLines.Text += "\n";
                lblOccurrences.Content = String.Format("Found {0} occurrences of '{1}'", indexes.Length, searchTerm);
            }

        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {

            txtFreqList.Text = "Freq.\tWord\n\n";
            Dictionary<string, int> wordList = frequencyCounter(lstDepunctuated.ToArray());
            var myList = wordList.ToList();

            myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value)); //Link syntax
            foreach (KeyValuePair<string, int> k in myList)
            {
                string tabs = "\t";
                if (k.Key.Length < 8)
                {
                    tabs += "\t";
                }
                txtFreqList.Text += string.Format("{0} --\t{1}", k.Value, k.Key);
                txtFreqList.Text += "\n";
            }
        }

        private void btnSort_Click2(object sender, RoutedEventArgs e)
        {
            txtFreqList.Text = "Freq.\tWord\n\n";
            Dictionary<string, int> wordList = frequencyCounter(lstDepunctuated.ToArray());
            var myList = wordList.ToList();

            myList.Sort((pair1, pair2) => pair2.Key.CompareTo(pair1.Key)); //Link syntax
            foreach (KeyValuePair<string, int> k in myList)
            {
                string tabs = "\t";
                if (k.Key.Length < 8)
                {
                    tabs += "\t";
                }
                txtFreqList.Text += string.Format("{0} --\t{1}", k.Value, k.Key);
                txtFreqList.Text += "\n";
            }
        }

        private void btnListCollocates_Click(object sender, RoutedEventArgs e)
        {
            txtCollocates.Text = "";
            string searchTerm = txtSearchCollocates.Text;
            int[] indexes = findMatches(lstDepunctuated.ToArray(), searchTerm.ToLower());
            List<string> surroundingWds = new List<string>();
            int range = Convert.ToInt32(sliWindowRange.Value);

            foreach (int index in indexes)
            {
                if ((index - range) > 0 && (index + range + 1) < lstDepunctuated.Count)
                {
                    for (int r = index - range; r < range + index + 1; r++)
                    {
                        if (r != index)
                        {
                            surroundingWds.Add(lstDepunctuated[r]);
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
            string result = "Freq.\tCollocate\n\n";

            foreach (string k in collocateList.Keys)
            {
                result += String.Format("{0} --\t {1}\n", collocateList[k], k);
            }
            txtCollocates.Text = result;
            lblCountCollocates.Content = String.Format("Listing {0} collocates of '{1}'", surroundingWds.Count(), searchTerm);
        }

        private void btnSortFreq_Click(object sender, RoutedEventArgs e)
        {
            txtCollocates.Text = "Freq.\tCollocate\n\n";
            var myList = collocateList.ToList();

            myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value)); //Link syntax
            foreach (KeyValuePair<string, int> k in myList)
            {
                string tabs = "\t";
                if (k.Key.Length < 8)
                {
                    tabs += "\t";
                }
                txtCollocates.Text += string.Format("{0} --\t{1}", k.Value, k.Key);
                txtCollocates.Text += "\n";
            }
        }

        private void btnSortAlph_Click(object sender, RoutedEventArgs e)
        {
            txtCollocates.Text = "Freq.\tCollocate\n\n";
            var myList = collocateList.ToList();

            myList.Sort((pair1, pair2) => pair2.Key.CompareTo(pair1.Key)); //Link syntax
            foreach (KeyValuePair<string, int> k in myList)
            {
                string tabs = "\t";
                if (k.Key.Length < 8)
                {
                    tabs += "\t";
                }
                txtCollocates.Text += string.Format("{0} --\t{1}", k.Value, k.Key);
                txtCollocates.Text += "\n";
            }
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string[] splitted = txtFreqList.Text.Split('\n');

			string directory = @"D:\Concordancer Results\";
			string x = System.IO.Path.Combine(directory, "Freq_results.txt");

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			for (int i = 0; i < splitted.Length; i++)
			{
				File.WriteAllLines(x, splitted);
				
			}

		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			string[] CoNsplitted = txtConcordanceLines.Text.Split('\n');
			string directory = @"D:\Concordancer Results\";
			string x = System.IO.Path.Combine(directory, "Conc_results.txt");

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			for (int i = 0; i < CoNsplitted.Length; i++)
			{
				File.WriteAllLines(x, CoNsplitted);

			}
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			string[] CoLsplitted = txtCollocates.Text.Split('\n');
			string directory = @"D:\Concordancer Results\";
			string x = System.IO.Path.Combine(directory, "Collo_results.txt");

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			for (int i = 0; i < CoLsplitted.Length; i++)
			{
				File.WriteAllLines(x, CoLsplitted);

			}
		}

		private void txtFreqList_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{

			txtFreqList.Text = ("What is a Concordancer? : https://en.wikipedia.org/wiki/Concordancer \n \n" +
				"What is a Collocate? : https://en.wikipedia.org/wiki/Collocation \n \n" +
				"Copy the links into a browser for more information.");
		}
	}
}
