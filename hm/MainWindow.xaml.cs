using System;
using System.Windows;

namespace hm
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

            string searchUrl = $"https://www.bing.com/search?q={Uri.EscapeDataString(searchQuery)}";
            webBrowser.Navigate(new Uri(searchUrl));
        }
    }
}
