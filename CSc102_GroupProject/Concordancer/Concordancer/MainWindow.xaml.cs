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

        private void btnFrequency_Click(object sender, RoutedEventArgs e)
        {
            //TODO: must take the loadedText and send it to frequency counter
            //what the frequency counter returns must be shown in the txtFreqList in an orderly fashion
            cleanedText = removePunctuation(loadedText.ToLower());  //uses the efficient method to display the book without punctuation
            string[] totalWords = cleanedText.Split();
            Dictionary<string, int> wordList = frequencyCounter(totalWords);
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
            
            List<string> textList = textArray.ToList();
            textList.RemoveAll(p => string.IsNullOrEmpty(p));
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
            int range = Convert.ToInt32(sliWindowRange.Value);
            string searchTerm = txtSearchTerm.Text;
            string[] punctuatedText = loadedText.Split();
            int i = 0;
            while (punctuatedText.Length > 0)
            {
                if (searchTerm == punctuatedText[i])
                {
                    try
                    {
                        for (int n = i-range; n < i + range; i++)
                        {
                            txtConcordanceLines.Text += punctuatedText[n] + " ";

                        }
                        txtConcordanceLines.Text += "\n";
                    }
                    catch (Exception E)
                    {

                    }
                }
            }
        }
    }
}
