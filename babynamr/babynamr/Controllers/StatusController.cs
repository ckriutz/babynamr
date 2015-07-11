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
            // If we can't even connect to the search service, this will throw an exception.
            try
            {
                var indexExists = _searchClient.Indexes.Exists("babynames");

                if (indexExists)
                {
                    var stats = _searchClient.Indexes.GetStatistics("babynames");
                    HttpResponseMessage response = Request.CreateResponse<Microsoft.Azure.Search.Models.IndexGetStatisticsResponse>(HttpStatusCode.OK, stats);
                    return response;
                }

                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Index does not exist."
                };
                return null;
            }
            catch (Hyak.Common.CloudException cloudException)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = cloudException.Error.Message
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = String.Format("Likely unable to connect to Azure Search service.")
                };
            }

            
        }
    }
}
