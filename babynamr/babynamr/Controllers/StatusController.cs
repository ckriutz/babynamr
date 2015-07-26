using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace babynamr.Controllers
{
    // What the status controller will do is return the status of the search index!
    // If there is no Index for "babynames" then one will be created.
    // If there are no babynames in the index, the items will be added!

    public class StatusController : ApiController
    {
        private static SearchServiceClient _searchClient;
        private const string IndexName = "babynames";

        StatusController()
        {
            string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
            string apiKey = ConfigurationManager.AppSettings["SearchServiceApiKey"];

            // Create an HTTP reference to the catalog index
            _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));

        }


        public HttpResponseMessage Get()
        {
            // If we can't even connect to the search service, this will throw an exception.
            try
            {
                var indexExists = _searchClient.Indexes.Exists(IndexName);


                if (!indexExists) // Well, there is no index. Let's create one with our handy-dandy method!
                {
                    var indexResponse = CreateIndex();
                    // TODO: If indexResponse is null or something because it failed we should back out.
                }

                // Now that we are sure we have an index...
                var stats = _searchClient.Indexes.GetStatistics(IndexName);

                if (stats.DocumentCount == 0)
                {
                    // There are no baby names. :( lets add some in!
                    AddSomeDocuments();
                    stats = _searchClient.Indexes.GetStatistics(IndexName);
                }

                HttpResponseMessage response = Request.CreateResponse<Microsoft.Azure.Search.Models.IndexGetStatisticsResponse>(HttpStatusCode.OK, stats);
                return response;

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

        private static IndexDefinitionResponse CreateIndex()
        {
            // Create the Azure Search index based on the included schema
            try
            {
                var definition = new Index()
                {
                    Name = IndexName,
                    Fields = new[]
                    {
                        new Field("id", DataType.String) { IsKey = true,  IsSearchable = false, IsFilterable = false, IsSortable = false, IsFacetable = false, IsRetrievable = true},
                        new Field("name", DataType.String) { IsKey = false, IsSearchable = true,  IsFilterable = true,  IsSortable = true,  IsFacetable = false, IsRetrievable = true},
                        new Field("orgin", DataType.String) { IsKey = false, IsSearchable = true,  IsFilterable = true,  IsSortable = true,  IsFacetable = true, IsRetrievable = true},
                        new Field("gender", DataType.String) { IsKey = false, IsSearchable = true,  IsFilterable = true,  IsSortable = true,  IsFacetable = true, IsRetrievable = true},
                        new Field("meaning", DataType.String) { IsKey = false, IsSearchable = true, IsFilterable = false,  IsSortable = false,  IsFacetable = false,  IsRetrievable = true}, }
                };
                var response = _searchClient.Indexes.Create(definition);
                return response;

            }
            catch
            {
                // So I mean this sucks. I need to figure out what to do with this error. When it happens.
                return null;
            }

        }

        private static void AddSomeDocuments()
        {
            // This will put a bunch of JSON documents (names!) into the search.

            // The way this is done is admittingly awful. We have a tad bit over 3k names to add to the search list. Doing them all at once fails (even with the 2015-02-28 API version)
            // ...so the best way is to chuck it out. In theroy, it would all be in one method or document and we can batch them in one method in chucks of 100. Doing something like that will be next.
            // Also, the whole new {} object and serializing them into JSON sucks. I made a JSON document with them all in there! (names.json)  For some reason though, just passing in THAT
            // fails. Ideally, we'd maintain that list and find a way to get that to work. until then, THIS works so we can go from there.

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("api-key", ConfigurationManager.AppSettings["SearchServiceApiKey"]);

            var responseAB = httpClient.PostAsync("https://" + ConfigurationManager.AppSettings["SearchServiceName"] + ".search.windows.net/indexes/" + IndexName + "/docs/index?api-version=2015-02-28", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(Helpers.NameMaker.GetNamesAtoB()), System.Text.Encoding.Unicode, "application/json")).Result;
            responseAB.EnsureSuccessStatusCode();
            
            var responseCF = httpClient.PostAsync("https://" + ConfigurationManager.AppSettings["SearchServiceName"] + ".search.windows.net/indexes/" + IndexName + "/docs/index?api-version=2015-02-28", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(Helpers.NameMaker.GetNamesCtoF()), System.Text.Encoding.Unicode, "application/json")).Result;
            responseCF.EnsureSuccessStatusCode();
            
            var responseGM = httpClient.PostAsync("https://" + ConfigurationManager.AppSettings["SearchServiceName"] + ".search.windows.net/indexes/" + IndexName + "/docs/index?api-version=2015-02-28", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(Helpers.NameMaker.GetNamesGtoM()), System.Text.Encoding.Unicode, "application/json")).Result;
            responseGM.EnsureSuccessStatusCode();

            var responseNQ = httpClient.PostAsync("https://" + ConfigurationManager.AppSettings["SearchServiceName"] + ".search.windows.net/indexes/" + IndexName + "/docs/index?api-version=2015-02-28", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(Helpers.NameMaker.GetNamesNtoQ()), System.Text.Encoding.Unicode, "application/json")).Result;
            responseNQ.EnsureSuccessStatusCode();
            
            var responseRS = httpClient.PostAsync("https://" + ConfigurationManager.AppSettings["SearchServiceName"] + ".search.windows.net/indexes/" + IndexName + "/docs/index?api-version=2015-02-28", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(Helpers.NameMaker.GetNamesRtoS()), System.Text.Encoding.Unicode, "application/json")).Result;
            responseRS.EnsureSuccessStatusCode();
            
            var responseTZ = httpClient.PostAsync("https://" + ConfigurationManager.AppSettings["SearchServiceName"] + ".search.windows.net/indexes/" + IndexName + "/docs/index?api-version=2015-02-28", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(Helpers.NameMaker.GetNamesTtoZ()), System.Text.Encoding.Unicode, "application/json")).Result;
            responseTZ.EnsureSuccessStatusCode();

            //var response = httpClient.PostAsync("https://" + ConfigurationManager.AppSettings["SearchServiceName"] + ".search.windows.net/indexes/" + IndexName + "/docs/index?api-version=2015-02-28", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(Helpers.NameMaker.GetNames()), System.Text.Encoding.Unicode, "application/json")).Result;
            //api-version=2015-02-28"
        }

        
    }
}
