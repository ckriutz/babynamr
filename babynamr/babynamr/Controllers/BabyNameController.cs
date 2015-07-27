using System.Web.Http;

namespace babynamr.Controllers
{
    public class BabyNameController : ApiController
    {
        private readonly Helpers.BabyNameSearch _babyNameSearch = new Helpers.BabyNameSearch();
        public dynamic Get(string q = "")
        {
            // If blank search, assume they want to search everything
            if (string.IsNullOrWhiteSpace(q))
            {
                q = "*";
            }
            
            var results = _babyNameSearch.Search(q);

            return results;
        }

        public dynamic Get(string gender, string q = "" )
        {
            // If blank search, assume they want to search everything
            if (string.IsNullOrWhiteSpace(q))
            {
                q = "*";
            }

            var results = _babyNameSearch.Search(q, gender);

            return results;
        }
        
    }
}
