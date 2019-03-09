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

    /*
     "_source": {
                      "text": "Jo també sóc sòcia de @omnium i n’estic molt orgullosa!!! @ La Roca, Cataluna, Spain https://t.co/lK2x1cYBrH",
                      "id": 1100827888465690600,
                      "user": {
                        "id": 150952599,                       
                        "followers_count": 2003,
                      },
                      "geo": {
                        "coordinates": [41.58611426, 2.32474123]
                      },
                    },
         */

    //this class represents a single tweet
    public class Tweet
    {
        public string id_str { get; set; }
        public string text { get; set; }
        public User user { get; set; }
        public Geo geo { get; set; }
    }

    public class Geo
    {
        public List<double> coordinates { get; set; } //long, lat
    }

    public class User
    {
        public string id_str { get; set; }
        public int followers_count { get; set; }
    }

}