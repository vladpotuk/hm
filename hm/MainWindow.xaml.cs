using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace hm
{
    public partial class MainWindow : Window
    {
        private HttpClient httpClient;
        private const string GutenbergUrl = "https://www.gutenberg.org";

        public MainWindow()
        {
            InitializeComponent();
            httpClient = new HttpClient();
            LoadTopBooks();
        }

        private async void LoadTopBooks()
        {
            try
            {
                string url = $"{GutenbergUrl}/browse/scores/top";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string html = await response.Content.ReadAsStringAsync();
                List<string> bookTitles = ParseTopBooks(html);

                
                foreach (string title in bookTitles)
                {
                    BooksListBox.Items.Add(title);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні книг: {ex.Message}");
            }
        }

        private List<string> ParseTopBooks(string html)
        {
            List<string> bookTitles = new List<string>();

            try
            {
                int startIndex = html.IndexOf("<ol class=\"bookindex\">");
                int endIndex = html.IndexOf("</ol>", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    string booksListHtml = html.Substring(startIndex, endIndex - startIndex);

                    
                    int index = 0;
                    while ((startIndex = booksListHtml.IndexOf("<a href=\"/ebooks/", index)) != -1)
                    {
                        startIndex = booksListHtml.IndexOf(">", startIndex) + 1;
                        endIndex = booksListHtml.IndexOf("</a>", startIndex);

                        if (endIndex != -1)
                        {
                            string bookTitle = booksListHtml.Substring(startIndex, endIndex - startIndex).Trim();
                            bookTitles.Add(bookTitle);
                            index = endIndex + "</a>".Length;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при парсингу списку книг: {ex.Message}");
            }

            return bookTitles;
        }

        private async void BookTitleSelected(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            string bookTitle = (string)listBox.SelectedItem;

            try
            {
              
                string url = $"{GutenbergUrl}/ebooks/search/?query={Uri.EscapeUriString(bookTitle)}";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string html = await response.Content.ReadAsStringAsync();
                string bookTextUrl = ParseBookTextUrl(html);

                if (!string.IsNullOrEmpty(bookTextUrl))
                {
                    
                    response = await httpClient.GetAsync(bookTextUrl);
                    response.EnsureSuccessStatusCode();

                    string bookText = await response.Content.ReadAsStringAsync();
                    
                    BookTextBlock.Text = bookText;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні тексту книги: {ex.Message}");
            }
        }

        private string ParseBookTextUrl(string html)
        {
            string bookTextUrl = "";

            try
            {
                int startIndex = html.IndexOf("<a href=\"/ebooks/");
                if (startIndex != -1)
                {
                    startIndex += "<a href=\"".Length;
                    int endIndex = html.IndexOf("\">", startIndex);
                    if (endIndex != -1)
                    {
                        bookTextUrl = $"{GutenbergUrl}{html.Substring(startIndex, endIndex - startIndex)}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при парсингу URL тексту книги: {ex.Message}");
            }

            return bookTextUrl;
        }
    }
}
