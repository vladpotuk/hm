using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

            List<string> imageUrls = await FetchImagesFromSelectedEngines(searchQuery);

            DisplayImages(imageUrls);
        }

        private async Task<List<string>> FetchImagesFromSelectedEngines(string searchQuery)
        {
            List<string> imageUrls = new List<string>();

            foreach (ComboBoxItem item in cmbSearchEngines.Items)
            {
                if (item.Content.ToString() == "Bing")
                {
                    string bingUrl = $"https://www.bing.com/images/search?q={Uri.EscapeDataString(searchQuery)}";
                    List<string> bingImages = await FetchImageUrlsAsync(bingUrl);
                    imageUrls.AddRange(bingImages);
                }
                else if (item.Content.ToString() == "Google")
                {
                    string googleUrl = $"https://www.google.com/search?tbm=isch&q={Uri.EscapeDataString(searchQuery)}";
                    List<string> googleImages = await FetchGoogleImageUrlsAsync(googleUrl);
                    imageUrls.AddRange(googleImages);
                }
                
            }

            return imageUrls;
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

        private async Task<List<string>> FetchGoogleImageUrlsAsync(string searchUrl)
        {
            List<string> imageUrls = new List<string>();

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

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
                        if (!string.IsNullOrEmpty(imageUrl) && !imageUrl.StartsWith("data:"))
                        {
                            imageUrls.Add(imageUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching Google image URLs: {ex.Message}");
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
