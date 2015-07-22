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
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("api-key", ConfigurationManager.AppSettings["SearchServiceApiKey"]);

            // I really don't like how this works but whatever. It works.
            // TODO: Make something better.
            var batch = new
            {
                value = new[] 
                { 
                    new { id = "1", name = "Casey", orgin = "Irish", gender = "Both", meaning = "Casey is a given name, derived either from the Irish Gaelic cathasaigh, meaning vigilant or watchful. or from a combination of the initials K.C. It is in use for both boys and girls in the United States, often with variant spellings. It was the 326th most popular name for boys born in the United States in 2007 and the 458th most popular name for girls. It ranked among the top 200 names for both sexes in the 1990s. Variants include Caci, Cacey, Kacey, Kaci, Kacie, Kasey, Kaycee, Kayci, Kaysi, Kaysey, Kaycie, and honestly any sort of possible combination you can imagine." },
                    new { id = "2", name = "Brandy", orgin = "English", gender = "Female", meaning = "Brandy is a feminine given name. It is possible that the name is derived from the Dutch language brandewijn, meaning 'brandy wine', or 'brandewine'; however, it is more likely a feminine form of Brandon. Variants include Brandi, Brandee, and Brandiy though the latter has never been found in the wild." },
                    new { id = "2", name = "Liam", orgin = "Irish", gender = "Male", meaning = "Liam is a short form of the Irish name 'Uilliam', itself a derivative of the Frankish, 'Willahelm'. The original name was a merging of the Old German elements, vila ('will' or 'resolution') and helma ('helmet'), and therefore, means 'helmet of will' and 'Guardian'. So basically one who guards the Helmet of Will, which honestly is pretty cool." },
                    new { id = "3", name = "Noah", orgin = "Hebrew", gender = "Male", meaning = "Noah is a name with Hebrew origins. The meaning of Noah is peace/rest. In the Bible, Noah was known for gathering animals 'two by two' and creating an ark of refuse for the great flood. Higher possibility of becoming a horder, or one who hords things." },
                    new { id = "4", name = "Mason", orgin = "French", gender = "Male", meaning = "From an English surname meaning 'stoneworker', from an Old French word of Germanic origin (akin to Old English macian 'to make'). This name represents people who like this hit thigns with other things." },
                    new { id = "5", name = "Jacob", orgin = "Hebrew", gender = "Male", meaning = "His original name Ya'akov is sometimes explained as having meant 'holder of the heel' or 'supplanter', because he was born holding his twin brother Esau's heel, and eventually supplanted Esau in obtaining their father Isaac's blessing. Other scholars speculate that the name is derived from a longer form such as Hebrew: יַעֲקֹבְאֵל (Ya'aqov'el) meaning 'may God protect'. In te television series 'Lost', Jacob has been the guardian of the island for close to two thousand years. He was first mentioned as the true leader of the Others. So keep these sort of things in mind." },
                    new { id = "6", name = "William", orgin = "German", gender = "Male", meaning = "William comes ultimately from the given name Wilhelm (cf. Old German Wilhelm > German Wilhelm and Old Norse Vilhjálmr). The Anglo-Saxon form should be *Wilhelm as well (although the Anglo-Saxon chronicle refers to William the Conqueror as Willelm). That is a compound of two distinct elements : wil = 'will or desire'; helm; Old English helm 'helmet, protection'; > English helm 'knight's large helmet'. In fact, William is from the Old Norman form Williame, because the English language should have retained helm. The development to -iam is the result of the diphthongation [iaʷ] + [m] in Old Norman-French, quite similar in Old Central French [eaʷ] + [m] from an early Gallo-Romance form WILLELMU. This development can be followed in the different versions of the name in the Wace's Roman de Rou. The spelling and phonetics Wi- [wi] is a characteristic trait of the Northern French dialects, but the pronunciation changed in Norman from [wi] to [vi] in the 12th century (cf. the Norman surnames Villon and Villamaux 'little William'), unlike the Central French and Southern Norman that turned the Germanic Wi- into Gui- [gwi] > [gi]. The Modern French spelling is Guillaume. The first well-known carrier of the name was Charlemagne's cousin William of Gellone, a.k.a. Guilhelm, William of Orange, Guillaume Fierabrace, or William Short-Nose (755–812). This William is immortalized in the Chanson de Guillaume and his esteem may account for the name's subsequent popularity among European nobility. Also, the shortahnd 'Will' is something people make when they want to distribute assests when they die." },
                    new { id = "7", name = "Ethan", orgin = "Hebrew", gender = "Male", meaning = "Ethan is a male given name of Hebrew origin (איתן) that means firm, strong and long-lived. It may also be spelled Eitan or Eytan though nobody actually does that." },
                    new { id = "8", name = "Michael", orgin = "Hebrew", gender = "Male", meaning = "Michael /ˈmaɪkəl/ is a male given name that comes from the Hebrew: מִיכָאֵל / מיכאל‎ (Mīkhāʼēl, pronounced [miχaˈʔel]), derived from the question מי כאל mī kāʼēl) meaning 'Who is like God?' (literally, Who is like El?). In English, it is sometimes shortened to Mike, Mikey, Mickey, or Mick. Michael will often think he is either god, or god like, or a gift from god. Historically is protected by Liam." },
                    new { id = "9", name = "James", orgin = "Hebrew", gender = "Male", meaning = "James came into the English language from the Old French variation James of the late Latin name Iacomus. This was a dialect variant of Iacobus, from the New Testament Greek Ἰάκωβος (Iákōbos), from Hebrew יעקב (Yaʻaqov) (Jacob). The development Iacobus > Iacomus is likely a result of nasalization of the o and assimilation to the following b (i.e., intermediate *Iacombus) followed by simplification of the cluster mb through loss of the b. Diminutives include: Jim, Jimmy, Jimmie, Jamie, Jimbo and others. You get the drift." },
                    new { id = "10", name = "Daniel", orgin = "Hebrew", gender = "Male", meaning = "Daniel is a masculine given name and a surname of Hebrew origin. It means, 'God is my judge', and derives from two early Biblical figures, primary among them Daniel from the Book of Daniel. It is a common given name for males, and is also used as a surname. It is also the basis for various derived given names and surnames. Lots of people are named this." },
                    new { id = "11", name = "Emma", orgin = "Latin", gender = "Female", meaning = "Emma is a name of derived of Latin orgin and is likely a shorthand version of Emily. The current thought is the name means 'Universal', though likely a cop-out." },
                    new { id = "12", name = "Olivia", orgin = "English", gender = "Female", meaning = "A popular feminine given name in the English language. It is a Latinate name derived, first coined by William Shakespeare for a character in the Twelfth Night. It is possible that Shakespeare may have intended this name as a feminine form of Oliver; another possibility is that he may have derived it from the Latin oliva,ae, which translates into English as 'olive' Olive is also a really popular name. It's oil is popular for cooking." },
                    new { id = "13", name = "Charlotte", orgin = "French", gender = "Female", meaning = "Charlotte is a female given name, a female form of the male name Charles. It is of French origin meaning 'free man'. The name back to the 17th century. There is a 100% chance that at one point in Charlotte's life she will spin a web of lies - thanks to the book which is her namesake." },
                    new { id = "14", name = "Sophia", orgin = "Greek", gender = "Female", meaning = "Sophia is a female name derived from σοφία, the Greek word for 'Wisdom'. The name was used to represent the personification of wisdom. Never let her know this." },
                    new { id = "15", name = "Isabella", orgin = "Latin", gender = "Female", meaning = "Isabella is a feminine given name, which is the Latinised form of Elizabeth." },
                    new { id = "16", name = "Brooke", orgin = "English", gender = "Female", meaning = "This name literally means 'Small Stream'." },
                    new { id = "17", name = "Ava", orgin = "German", gender = "Female", meaning = "The medieval name Ava is an abbreviation of a Germanic name containing the first element av-, meaning water from Proto-Persian word ab or av for water. Saint Ava was a 9th-century princess, daughter of Pepin II of Aquitaine. Ava was also the name of a medieval German woman poet. This name is the origin of the Norman French name of Aveline, which in turn gave rise to the English given name of Evelyn. Its recent popularity is ultimately due to a number of celebrity babies of the 1990s, who were ultimately named after American actress Ava Gardner (who had died in 1990). At least thats what Wikipedia thinks so take it with a grain of Salt." },
                    new { id = "18", name = "Mia", orgin = "Hebrew", gender = "Female", meaning = "The earliest known reference was to the short form of Miriam (מִרְיָם), an ancient female Hebrew name. Bearing in mind that many Levite names are Egyptian, it might be derived from an Egyptian word myr, which means 'beloved'. The name has subsequently come to be associated with the Italian word mia, meaning 'mine', and also recognized as a derivation from the Slavic word mila, meaning 'dear, darling'. I know this doesn't help much but were doing the best we can." },
                    new { id = "19", name = "Emily", orgin = "Latin", gender = "Female", meaning = "Emily is a feminine name derived from the Roman feminine name Aemilia. The Latin name Aemilia in turn may derive from the Latin word aemulus (or from the same root as aemulus), meaning 'rival', but this may be a folk etymology. Actually it's likely folk etymology. In any case this name is pretty awesome." },
                    new { id = "20", name = "Abagail", orgin = "Hebrew", gender = "Female", meaning = "Abigail (Hebrew: אֲבִיגַיִל / אֲבִיגָיִל, Modern Avigáyil Tiberian ʾĂḇîḡáyil / ʾĂḇîḡāyil ; 'my father's joy', spelled Abigal in 2 Samuel 17:25 in the American Standard Version but not in the King James Version) was the wife of Nabal; she became a wife of David after Nabal's death (1 Samuel 25). Abigail is David's second wife, after Saul's daughter, Michal, whom Saul later married to Palti, son of Laish when David went into hiding. Increased chance of 'daddy issues.'" },
                    new { id = "21", name = "Madison", orgin = "English", gender = "Both", meaning = "Basically just means 'son of Matthew.' Back in the day it was a popular Boys name, but now it's a popular girls name. <shrugs>" },
                    new { id = "22", name = "Dylan", orgin = "Welsh", gender = "Male", meaning = "Dylan is a Welsh male given name. It is derived from the word llanw, meaning 'tide' or 'flow' and the intensifying prefix dy-. Dylan ail Don was a character in Welsh mythology, but the popularity of Dylan as a given name in modern times arises from its use by poet Dylan Thomas. Its use as a surname stems from the adoption of the name by Bob Dylan. In Wales it was the most popular Welsh name given to babies in 2010. Anyone named Dylan is likely to either BE attractive, or think of oneself as." },
                    new { id = "23", name = "Luke", orgin = "Latin", gender = "Male", meaning = "The name Luke is derived from the Latin name Lucas or from the Greek Loukas, meaning 'man from Lucania' (a region of Italy). Although the name is attested in ancient inscriptions, the best known historical use of the name is in the New Testament. The Gospel of Luke was written around 70 to 90 AD (the exact years are unknown), and was from here that the name was first popularized. Luke, who is credited with the authorship of the Gospel of Luke, was a physician who lived around 30 to 130 AD. Luke is also credited with the Book of Acts in the Bible, and also is mentioned by the Apostle Paul in some of Paul's letters to first-century churches." },
                    new { id = "24", name = "Samantha", orgin = "England", gender = "Female", meaning = "Samantha is a feminine given name. It has been recorded in England in 1633 in Newton-Regis, Warwickshire, England. It was also recorded in the 18th century in New England, but its etymology is unknown. Speculation (without evidence) has suggested an origin from the masculine given name Samuel and anthos, the Greek word for 'flower'. A variant of this speculation is that it may have been a feminine form of Samuel with the addition of the already existing feminine name Anthea. Other variants of the name include Sam, Sammie, Sammy, but never Sammmy (three m's)" },
                    new { id = "25", name = "Allison", orgin = "German", gender = "Female", meaning = "Means 'son of noble one.' How noble is up to debate. Apperently used to be a male name back in the day but is more popular as a last name. All this is news to us." },
                    new { id = "26", name = "Claire", orgin = "Latin", gender = "Female", meaning = "Claire or Clair /ˈklɛər/ is a given name of Latin/Viking origin via French; the name could mean 'clear' or 'famous'. The word still means clear in French in its feminine form." },
                    new { id = "27", name = "Levi", orgin = "Hebrew", gender = "Male", meaning = "Levi/Levy (/ˈliːvaɪ/, Hebrew: לֵּוִי‎; Standard Levy Tiberian Lēwî; LITERALLY joining) was, according to the Book of Genesis, the third son of Jacob and Leah, and the founder of the Israelite Tribe of Levi (the Levites). Certain religious and political functions were reserved for the Levites, and in the documentary hypothesis, the early sources of the Torah—the Jahwist and Elohist—appear to treat the term Levi as just being a word meaning priest; some scholars therefore suspect that 'levi' was originally a general term for a priest, and had no connection to ancestry, and that it was only later, for example in the priestly source and Blessing of Moses, that the existence of a tribe named Levi became assumed, in order to explain the origin of the priestly caste. So I guess thats cool." },
                    new { id = "28", name = "Sadie", orgin = "English", gender = "Female", meaning = "Sadie is a name with English origins. The name Sadie is a variation of the name Sarah and both mean Princess. The most common nickname for Sadie is Sayde. If you name your child, 'Princess' is absolutely going to be true." },
                    new { id = "29", name = "Aaron", orgin = "", gender = "Male", meaning = "In the Hebrew Bible and the Quran, Aaron אַהֲרֹן (/ˈɛərən/) was the older brother of Moses (Exodus 6:16-20, 7:7; Qur'an 28:34) and a prophet of God. Unlike Moses, who grew up in the Egyptian royal court, Aaron and his elder sister Miriam remained with their kinsmen in the eastern border-land of Egypt (Goshen). When Moses first confronted the Egyptian king about the Israelites, Aaron served as his brother's spokesman ('prophet') to Pharaoh. (Exodus 7:1) Part of the Law (Torah) that Moses received from God at Sinai granted Aaron the priesthood for himself and his male descendants, and he became the first High Priest of the Israelites. Various dates for his life have been proposed, ranging from approximately 1600 to 1200 BC. Aaron died before the Israelites crossed the Jordan river and he was buried on Mount Hor (Numbers 33:39; Deuteronomy 10:6 says he died and was buried at Moserah). Aaron is also mentioned in the New Testament of the Bible." },
                    new { id = "30", name = "Sarah", orgin = "Hebrew", gender = "Female", meaning = "Sarah or Sara (/ˈsɛərə/;[1] Hebrew: שָׂרָה, Modern Sara, Tiberian Śārā ISO 259-3 Śarra; Latin: Sara; Arabic: سارا or سارة Sāra;) was the wife and half-sister of Abraham and the mother of Isaac as described in the Hebrew Bible and the Quran. Her name was originally Sarai. According to Genesis 17:15, God changed her name to Sarah as part of a covenant after Hagar bore Abraham his first son, Ishmael. The Hebrew name Sarah indicates a woman of high rank and is translated as 'princess' or 'noblewoman'." },
                    new { id = "31", name = "Peyton", orgin = "English", gender = "Both", meaning = "From Pacca's Town" },
                    new { id = "32", name = "Robert", orgin = "German", gender = "Male", meaning = "The name Robert is a Germanic given name, from Old High German Hrodebert 'bright with glory' (a compound of hruod 'fame, glory' and berht 'bright'). It is also in use as a surname. It's also Bob." },
                    new { id = "33", name = "Lucy", orgin = "French", gender = "Female", meaning = "Lucy is an English and French feminine given name derived from Latin masculine given name Lucius with the meaning as of light (born at dawn or daylight, maybe also shiny, or of light complexion). Alternative spellings are Luci, Luce, Lucie" },
                    new { id = "34", name = "Gavin", orgin = "Welsh", gender = "Male", meaning = "Gavin is a male given name. It is the late medieval form of the name Gawain, which in turn is believed to have originated from the Welsh name Gwalchgwn, meaning 'White Hawk.' Sir Gawain and the Green Knight is an epic poem connected with King Arthur's Round Table. Gavin also shares an origin with the Italian name Gavino, which dates back to ancient Latin. Saint Gavinus (San Gavino, Porto Torres, Sardinia) was an early Christian martyr, an ex Roman centurion, decapitated in 300 AD and whose head was thrown in the Mediterranean sea before being reunited with his body. So I mean there's that." },
                    new { id = "35", name = "Bridget", orgin = "Irish", gender = "Female", meaning = "Bridget or Brigid is a Celtic/Irish female name derived from the noun brígh, meaning 'power, strength, vigor, virtue'. An alternate meaning of the name is 'exalted one'. Its popularity, especially in Ireland, is largely related to the popularity of Saint Brigid of Kildare, who was so popular in Ireland she was known as 'Mary of the Gael'. This saint took on many of the characteristics of the early Celtic goddess Brigid, who was the goddess of agriculture and healing and possibly also of poetry and fire. One of her epithets was 'Brigid of the Holy Fire'. In German and Scandinavian countries, the popularity of the name spread due to Saint Bridget of Sweden." },
                    new { id = "36", name = "Brandon", orgin = "English", gender = "Male", meaning = "The name originates from the English surname Brandon. This surname can be derived from any of the numerous placenames in England so-named which are composed of two elements derived from the Old English language. The first element means 'broom', 'gorse'; and the second means 'hill'. There are several variant spellings of the given name Brandon; there is also probably, a feminine variant of the name. Brandon is considered to be a masculine name; however, in the United States during the 1980s, the name cracked the top 1,000th names recorded for female births; the name has since then fallen out of the top 1,000 female baby names which makes sense because what girl wants to be named Brandon, ever more so if it means 'broom hill'." },
                },
            };

            var response = httpClient.PostAsync("https://" + ConfigurationManager.AppSettings["SearchServiceName"] + ".search.windows.net/indexes/" + IndexName + "/docs/index?api-version=2014-10-20-Preview", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(batch), System.Text.Encoding.Unicode, "application/json")).Result;
            response.EnsureSuccessStatusCode();


        }
    }
}
