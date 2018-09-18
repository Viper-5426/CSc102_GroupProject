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
            loadedText = sString;  //uses the efficient method to display the book without punctuation
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
            string[] totalWords = loadedText.Split();
            Dictionary<string, int> wordList = frequencyCounter(totalWords);
            foreach (string k in wordList.Keys)
            {
                txtFreqList.Text += string.Format("'{0}' has {1} occurrence(s) \n", k, wordList[k]);
            }

        }

        private Dictionary<string, int> frequencyCounter(string[] textArray)
        {
            //TODO: needs to sort all items in the array
            //then count occurrence of each unique item (maybe use dictionary and return it instead of an array of arrays?)
            //needs to associate a count with each new word it encounters
            Dictionary<string, int> wordCounts = new Dictionary<string, int>();
            foreach (string ss in textArray)
            {
                if (wordCounts.ContainsKey(ss))
                {
                    wordCounts[ss]++;
                }
                else
                {
                    wordCounts[ss] = 1;
                }
            }
            return wordCounts;
        }
    }
}
