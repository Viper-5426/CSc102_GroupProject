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

            txtDisplay.Text = remove_punctuation(sString);  //uses the efficient method to display the book without punctuation

        }

        private string remove_punctuation(string sString)
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
    }
}
