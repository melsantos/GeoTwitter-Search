import json, os
from elasticsearch import Elasticsearch

es = Elasticsearch()

class IndexTweets:

    def str_is_json(myjson):
        try:
            json_object = json.loads(myjson)
        except ValueError as e:
            return False
        return True

    def indexDirTweets(dir):
        directory = os.fsencode("geo_tweets")

        #iterate through all of the JSON files
        for file in os.listdir(directory):
             filename = os.fsdecode(file)
             with open("geo_tweets/" + filename, 'r') as file:
                 for line in file:
                     #make a JSON object from each line
                     obj = json.loads(line)
                     #index a new tweet with its id being that of its id from Twitter
                     es.index(index="geotwitter", doc_type="tweets", id=obj['id'], body=obj)

    def indexLiveTweet(jsonObj):
        #make a JSON object from the given string
        if(str_is_json(jsonObj)):
            obj = json.loads(jsonObj)
        #index a new tweet with its id being that of its tweet id from Twitter
        es.index(index="geotwitter", doc_type="tweets", id=obj['id'], body=obj)

    def searchIndex(field, term):
        results = es.search(index="geotwitter", body={'query': {'match': {field: term}}})
        print("Got " + str(results['hits']['total']) +" Hits")

#use this instance of the IndexTweets object to perform desired function(s)
idxTwts = IndexTweets
print(idxTwts.searchIndex("text", "own"))
