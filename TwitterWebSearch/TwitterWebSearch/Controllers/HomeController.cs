using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwitterWebSearch.Models;
using System.Device.Location;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Nest;
using GeoCoordinate = System.Device.Location.GeoCoordinate;
using Elasticsearch.Net;

namespace TwitterWebSearch.Controllers
{
    public class HomeController : Controller
    {
        public static GeoCoordinate GetCoordinates(string address)
        {
            var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?address={0}&key=AIzaSyDccK65hnsCsuS5VmBDWaWo6BQoBwgcH78", Uri.EscapeDataString(address));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");
            var lat = locationElement.Element("lat").Value;
            var lng = locationElement.Element("lng").Value;
            return new GeoCoordinate(Convert.ToDouble(lat), Convert.ToDouble(lng));
        }

        public static List<Tweet> GetTweets(string query)
        {

            ConnectionSettings connectionSettings = new ConnectionSettings(
                new Uri("https://8e3c24de87514e9c86b35812c83002e0.us-west-1.aws.found.io:9243"))
                .DefaultIndex("geotwitter")
                .BasicAuthentication("rgray003", "elastic");
            ElasticClient elasticClient = new ElasticClient(connectionSettings);

            var response = elasticClient.Search<Tweet>(s => s
            .Type("tweets")
            .MatchAll()
            //.Query(q => q.Match(m => m.Query(query).Field(f => f.text)))
            );

            List<Tweet> tweets = response.Hits.Select(val => val.Source).ToList();

            //set the rank as the order in the list
            for (int i = 0; i < tweets.Count; i++)
            {
                tweets[i].rank = (i + 1);
            }

            return tweets;
        }

        [HttpGet]
        public ActionResult Search(string query, string distance)
        {
            SearchViewModel model = new SearchViewModel
            {
                SearchText = query == null ? "" : query,
                Distance = distance == null ? 0 : int.Parse(distance),
                Tweets = new List<Tweet>()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel model, string lat, string lon)
        {
            GeoCoordinate coord = new GeoCoordinate(Convert.ToDouble(lat), Convert.ToDouble(lon)); //my position
            string searchText = model.SearchText; //what the user searched
            int distance = model.Distance;

            List<Tweet> rankedTweets = GetTweets(searchText);

            //getdistanceto returns meters so doing user entered distance miles * 1609.344 (meters per mile) to convert meters
            //if distance not entered default at 100 miles
            model.Tweets = rankedTweets
                .Where(val => new GeoCoordinate(val.geo.coordinates[1], val.geo.coordinates[0]).GetDistanceTo(coord) < ((distance == 0 ? 100 : distance) * 1609.344))
                .ToList();

            return RedirectToAction("Search", "Home", new { query = searchText, distance = distance.ToString(), model.Tweets });
        }

        // TESTBENCH
        //[HttpPost]
        //public ActionResult Search(SearchViewModel model, string lat, string lon)
        //{
        //    GeoCoordinate coord = new GeoCoordinate(Convert.ToDouble(lat), Convert.ToDouble(lon)); //my position
        //    string searchText = model.SearchText; //what the user searched
        //    int distance = model.Distance;

        //    // Hardcode tweets into model.Tweets
        //    List<Tweet> rankedTweets = new List<Tweet>();

        //    // Tweets 1-3: Southern California
        //    Tweet t1 = new Tweet();
        //    t1.TweetID = "1102451034872377344";
        //    t1.TweetRank = 10;
        //    t1.Latitude = 33.834492; t1.Longitude = -117.915642;
        //    t1.tweetLink = "www.twitter.com"; t1.tweetText = "Sample tweet 1 - Anaheim, CA";

        //    Tweet t2 = new Tweet();
        //    t2.TweetID = "487075163129667584";
        //    t2.TweetRank = 9.5;
        //    t2.Latitude = 33.980602; t2.Longitude = -117.375496;
        //    t2.tweetLink = "www.twitter.com"; t2.tweetText = "Sample tweet 2 - Riverside, CA";

        //    Tweet t3 = new Tweet();
        //    t3.TweetID = "470788307278778369";
        //    t3.TweetRank = 8.25;
        //    t3.Latitude = 34.108318; t3.Longitude = -117.294151;
        //    t3.tweetLink = "www.twitter.com"; t3.tweetText = "Sample tweet 3 - San Bernardino, CA";

        //    // Tweets 4-5: Africa
        //    Tweet t4 = new Tweet();
        //    t4.TweetID = "470766788112683008";
        //    t4.TweetRank = 7.125;
        //    t4.Latitude = 0; t4.Longitude = 0;
        //    t4.tweetLink = "www.twitter.com"; t4.tweetText = "Sample tweet 4 - West Africa (Ocean)";

        //    Tweet t5 = new Tweet();
        //    t5.TweetID = "467436682245976065";
        //    t5.TweetRank = 6.0625;
        //    t5.Latitude = 7.946527; t5.Longitude = -1.023194;
        //    t5.tweetLink = "www.twitter.com"; t5.tweetText = "Sample tweet 5 - Ghana";

        //    // Tweets 6-8: Europe
        //    Tweet t6 = new Tweet();
        //    t6.TweetID = "1103456226388533248";
        //    t6.TweetRank = 5.12345;
        //    t6.Latitude = 51.507351; t6.Longitude = -0.127758;
        //    t6.tweetLink = "www.twitter.com"; t6.tweetText = "Sample tweet 6 - London";

        //    Tweet t7 = new Tweet();
        //    t7.TweetID = "1103327648909066240";
        //    t7.TweetRank = 4.123456;
        //    t7.Latitude = 40.463669; t7.Longitude = -3.749220;
        //    t7.tweetLink = "www.twitter.com"; t7.tweetText = "Sample tweet 7 - Spain";

        //    Tweet t8 = new Tweet();
        //    t8.TweetID = "1102747254207594496";
        //    t8.TweetRank = 3.1234567;
        //    t8.Latitude = 40.463669; t8.Longitude = -3.749220;
        //    t8.tweetLink = "www.twitter.com"; t8.tweetText = "Sample tweet 8 - Italy";

        //    // Tweets 9-11: Northern California
        //    Tweet t9 = new Tweet();
        //    t9.TweetID = "1101808856840179713";
        //    t9.TweetRank = 2.12345678;
        //    t9.Latitude = 37.804363; t9.Longitude = -122.271111;
        //    t9.tweetLink = "www.twitter.com"; t9.tweetText = "Sample tweet 9 - Oakland, CA";

        //    Tweet t10 = new Tweet();
        //    t10.TweetID = "1103445536378478592";
        //    t10.TweetRank = 1.003006009;
        //    t10.Latitude = 38.581573; t10.Longitude = -121.494400;
        //    t10.tweetLink = "www.twitter.com"; t10.tweetText = "Sample tweet 10 - Sacramento, CA";

        //    Tweet t11 = new Tweet();
        //    t11.TweetID = "1103477617481732096";
        //    t11.TweetRank = 0.5;
        //    t11.Latitude = 37.338207; t11.Longitude = -121.886330;
        //    t11.tweetLink = "www.twitter.com"; t11.tweetText = "Sample tweet 11 - San Jose, CA ";

        //    rankedTweets.Add(t1);
        //    rankedTweets.Add(t2);
        //    rankedTweets.Add(t3);
        //    rankedTweets.Add(t4);
        //    rankedTweets.Add(t5);
        //    rankedTweets.Add(t6);
        //    rankedTweets.Add(t7);
        //    rankedTweets.Add(t8);
        //    rankedTweets.Add(t9);
        //    rankedTweets.Add(t10);
        //    rankedTweets.Add(t11);

        //    // Note - To display all tweets, regardless of distance, use:
        //    model.Tweets = rankedTweets;

        //    // Note - To display tweets within a certain distance, use:
        //    //model.Tweets = rankedTweets
        //    //    .Where(val => new GeoCoordinate(val.Latitude, val.Longitude).GetDistanceTo(coord) < ((distance == 0 ? 100 : distance) * 1609.344))
        //    //    .ToList();
        //    return View(model);
        //}

    }
}