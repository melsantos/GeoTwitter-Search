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

        [HttpGet]
        public ActionResult Search()
        {
            SearchViewModel model = new SearchViewModel
            {
                SearchText = "",
                Tweets = new List<Tweet>()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel model, string lat, string lon)
        {
            GeoCoordinate coord = new GeoCoordinate(Convert.ToDouble(lat), Convert.ToDouble(lon)); //my position
            string searchText = model.SearchText; //what the user searched
            List<Tweet> rankedTweets = new List<Tweet>(); //get list from lucene using searchText above

            //getdistanceto returns meters so doing 100 miles * 1609.344 (meters per mile) to convert meters
            model.Tweets = rankedTweets
                .Where(val => new GeoCoordinate(val.Latitude, val.Longitude).GetDistanceTo(coord) < (100 * 1609.344))
                .ToList(); 

            return View(model);
        }

    }
}