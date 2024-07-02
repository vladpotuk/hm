using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;

namespace ImageSearchApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = txtSearchQuery.Text.Trim();
            if (string.IsNullOrEmpty(searchQuery))
            {
                MessageBox.Show("Please enter a search query.");
                return;
            }

            string searchUrl = $"https://www.bing.com/images/search?q={Uri.EscapeDataString(searchQuery)}";

            List<string> imageUrls = await FetchImageUrlsAsync(searchUrl);

            DisplayImages(imageUrls);
        }

        private async Task<List<string>> FetchImageUrlsAsync(string searchUrl)
        {
            List<string> imageUrls = new List<string>();

            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(searchUrl);
                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var imageNodes = htmlDocument.DocumentNode.SelectNodes("//img[@src]");
                if (imageNodes != null)
                {
                    foreach (var node in imageNodes)
                    {
                        string imageUrl = node.Attributes["src"].Value;
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrls.Add(imageUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching image URLs: {ex.Message}");
            }

            return imageUrls;
        }

        private void DisplayImages(List<string> imageUrls)
        {
            lstImageResults.Items.Clear();

            foreach (var imageUrl in imageUrls)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(imageUrl);
                image.EndInit();
                lstImageResults.Items.Add(image);
            }
        }
    }
}
