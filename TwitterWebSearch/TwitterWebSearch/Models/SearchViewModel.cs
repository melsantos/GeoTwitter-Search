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
        public int TweetID { get; set; }
        public int TweetRank { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string tweetLink { get; set; }
        public string tweetText { get; set; }
    }
}