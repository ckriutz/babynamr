using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace babynamr.Controllers
{
    public class StatusController : ApiController
    {
        private static SearchServiceClient _searchClient;
        private static SearchIndexClient _indexClient;

        StatusController()
        {
            string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
            string apiKey = ConfigurationManager.AppSettings["SearchServiceApiKey"];

            // Create an HTTP reference to the catalog index
            _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            //_indexClient = _searchClient.Indexes.GetClient("babynames");
        }


        public HttpResponseMessage Get()
        {
            //TODO: Create some sort of type that will give me better information.
            
            // If we can't even connect to the search service, this will throw an exception.
            try
            {
                var indexExists = _searchClient.Indexes.Exists("babynames");

                if (indexExists)
                {
                    var stats = _searchClient.Indexes.GetStatistics("babynames");

                    return new HttpResponseMessage()
                    {
                        StatusCode = stats.StatusCode,
                        ReasonPhrase = stats.DocumentCount.ToString()
                    };
                }

                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Index does not exist."
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = String.Format("Likely unable to connect to Azure Search service. SearchServiceName: {0}, SearchServiceKey: {1}", ConfigurationManager.AppSettings["SearchServiceName"], ConfigurationManager.AppSettings["SearchServiceApiKey"])
                };
            }

            
        }
    }
}
