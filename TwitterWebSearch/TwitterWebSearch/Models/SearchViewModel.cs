using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterWebSearch.Models
{
    public class SearchViewModel
    {
        public string SearchText { get; set; } //search text entered in the form
        public int Distance { get; set; }
        public List<Tweet> Tweets { get; set; } //a list of tweets returned after entering the search form
    }

    //this class represents a single tweet
    public class Tweet
    {
        public string id_str { get; set; }
        public string text { get; set; }
        public User user { get; set; }
        public Geo geo { get; set; }
        public Entity entities { get; set; }
        public double score { get; set; }
    }

    public class Geo
    {
        public List<double> coordinates { get; set; } //long, lat
    }

    public class User
    {
        public string id_str { get; set; }
        public string name { get; set; }
        public int followers_count { get; set; }
    }

    public class Entity
    {
        public List<Url> urls { get; set; }
    }

    public class Url
    {
        public string url { get; set; }
    }

}