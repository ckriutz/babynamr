using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Configuration;

namespace babynamr.Helpers
{
    public class BabyNameSearch
    {
        private static readonly SearchIndexClient IndexClient;
        public static string ErrorMessage;

        static BabyNameSearch()
        {
            try
            {
                string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
                string apiKey = ConfigurationManager.AppSettings["SearchServiceApiKey"];

                // Create an HTTP reference to the catalog index
                var searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
                IndexClient = searchClient.Indexes.GetClient("babynames");
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        public DocumentSearchResponse Search(string searchText)
        {
            // Execute search based on query string
            try
            {
                var sp = new SearchParameters() { SearchMode = SearchMode.All };
                return IndexClient.Documents.Search(searchText, sp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message);
            }
            return null;
        }
    }
}