using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace babynamr.Controllers
{
    public class BabyNameController : ApiController
    {
        private Helpers.BabyNameSearch _babyNameSearch = new Helpers.BabyNameSearch();
        //public List<Models.BabyName> Get(string q = "")
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
        
    }
}
