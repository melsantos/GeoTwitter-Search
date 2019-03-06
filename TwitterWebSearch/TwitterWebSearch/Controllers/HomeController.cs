using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwitterWebSearch.Models;
using System.Device.Location;
using System.Net;
using System.Xml.Linq;

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
            //var requestUri = string.Format("https://8e3c24de87514e9c86b35812c83002e0.us-west-1.aws.found.io:9243/geotwitter/_search?q=text:{0}", Uri.EscapeDataString(query));
           // var request = WebRequest.Create(requestUri);
            //request.ContentType = "application/json; charset=utf-8";
           // var response = request.GetResponse();
            //here we will convert our json response to a list of 
            //tweet objects. Currently getting unauthorized error on web request because there are no crednetials entered
            return new List<Tweet>();
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
                .Where(val => new GeoCoordinate(val.Latitude, val.Longitude).GetDistanceTo(coord) < ((distance == 0 ? 100 : distance) * 1609.344))
                .ToList();

            return RedirectToAction("Search", "Home", new { query = searchText, distance = distance.ToString() });
        }

    }
}