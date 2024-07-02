using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SearchApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = txtSearchQuery.Text.Trim();
            if (string.IsNullOrEmpty(searchQuery))
            {
                MessageBox.Show("Please enter a search query.");
                return;
            }

           
            ComboBoxItem selectedSearchEngine = cmbSearchEngines.SelectedItem as ComboBoxItem;
            if (selectedSearchEngine == null)
            {
                MessageBox.Show("Please select a search engine.");
                return;
            }

            string searchUrl = selectedSearchEngine.Tag.ToString() + Uri.EscapeDataString(searchQuery);
            DisplaySearchResults(searchUrl);
        }

        private void DisplaySearchResults(string searchUrl)
        {
           
            lstSearchResults.Items.Clear();

            
            List<string> results = new List<string>
            {
                "Result 1",
                "Result 2",
                "Result 3"
            };

            
            foreach (var result in results)
            {
                lstSearchResults.Items.Add(result);
            }
        }
    }
}
