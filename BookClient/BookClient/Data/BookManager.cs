using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookClient.Data
{
    public class BookManager
    {
        // This is the string for the URL of the webapp service I created in an Azure sandbox.
        // I got the first half of it by using an echo command after setting up the Azure web app in the Azure Shell
        // I will probably need to change this as the Sandbox is only active for a limited time and I will need to set up a new one to test this app again.
        const string Url = "https://bookserver32137.azurewebsites.net/api/books/";

        // Stores the token needed to sign in to the Azure web app
        private string authorizationKey;


        /// <summary>
        /// This method returns an instance of HttpClient that has an authorizationKey in its headers
        /// </summary>
        /// <returns></returns>
        private async Task<HttpClient> GetClient()
        {
            HttpClient client = new HttpClient();
            if (string.IsNullOrEmpty(authorizationKey)){
                authorizationKey = await client.GetStringAsync(Url + "login");
                authorizationKey = JsonConvert.DeserializeObject<string>(authorizationKey);
            }

            client.DefaultRequestHeaders.Add("Authorization", authorizationKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }

        /// <summary>
        /// Gets all the books from the web app 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Book>> GetAll()
        { 
            HttpClient client = await GetClient();
            string result = await client.GetStringAsync(Url);
            var books = JsonConvert.DeserializeObject<IEnumerable<Book>>(result);
            return books;
        }

        public async Task<Book> Add(string title, string author, string genre)
        {
            Book book = new Book();
            book.Title = title;
            book.Authors = new List<string>();
            book.Authors.Add(author);
            book.Genre = genre;
            book.ISBN = "";
            book.PublishDate = DateTime.Now;

            HttpClient client = await GetClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Url, content);

            return JsonConvert.DeserializeObject<Book>(await response.Content.ReadAsStringAsync());
        }

        public Task Update(Book book)
        {
            // TODO: use PUT to update a book
            throw new NotImplementedException();
        }

        public Task Delete(string isbn)
        {
            // TODO: use DELETE to delete a book
            throw new NotImplementedException();
        }
    }
}

