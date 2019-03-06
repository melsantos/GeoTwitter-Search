import json, os
import certifi #for CA certifications for elasticsearch
from elasticsearch import Elasticsearch

es = Elasticsearch("https://8e3c24de87514e9c86b35812c83002e0.us-west-1.aws.found.io:9243", http_auth=('elastic','thvjOYqdxl4INNEDvxOFXC3R'), use_ssl=True, verify_certs=True, ca_certs=certifi.where())

class IndexTweets:

    def str_is_json(self, myjson):
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

    def indexLiveTweet(self, jsonObj):
        #make a JSON object from the given string
        if(self.str_is_json(jsonObj)):
            obj = json.loads(jsonObj)
            #index a new tweet with its id being that of its tweet id from Twitter
            es.index(index="geotwitter", doc_type="tweets", id=obj['id'], body=obj)

    def searchIndexByID(id):
        results = es.search(index="geotwitter", body={'query': {'match': {'_id': id}}})
        return (results['hits']['hits'][0]['_source'])

    def searchIndexByTweet(terms):
        results = es.search(index="geotwitter", body={'query': {'match': {'text': terms}}})
        jsonList = []
        for i in results:
            jsonList.append(results['hits']['hits'])
        return jsonList

#use this instance of the IndexTweets object to perform desired function(s)
idxTwts = IndexTweets

#getting the text field of a tweet by ID
j = idxTwts.searchIndexByID("1100829985605107712")
print(j['text'])

#searching by a query and printing matching results
j = idxTwts.searchIndexByTweet("very own")
for idxI, i in enumerate(j):
    print("Score: " + str(i[idxI]['_score']) + "\nTweet: " + i[idxI]['_source']['text'] + "\n")
