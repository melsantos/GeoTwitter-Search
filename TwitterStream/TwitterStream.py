from decouple import config
from twitter.stream import TwitterStream, Timeout, HeartbeatTimeout, Hangup
from twitter.oauth import OAuth
from urllib import request
from bs4 import BeautifulSoup
from IndexTweets import IndexTweets
import os, json, datetime, re, threading, time

consumer_key = config('CONSUMER_KEY')
consumer_secret = config('CONSUMER_SECRET')
token = config('ACCESS_TOKEN')
token_secret =config('ACCESS_SECRET')

cur_path = os.path.dirname(os.path.realpath(__file__))

def connect():
    try:
        ts = TwitterStream(
            auth=OAuth(token, token_secret, consumer_key, consumer_secret))
        iterator = ts.statuses.sample()
        print("Connection successful")
        return iterator
    except:
        print("Connection unsuccessful")
        return None

def consume_tweets(iterator):
    it = IndexTweets()

    def get_title(tweet):
        try:
            url = re.match(r'.*(http?s://[^\s]+)', tweet['text']).group(1)
            content = request.urlopen(url).read()
            return BeautifulSoup(content, features="html.parser").title.string
        except:
            return None

    for tweet in iterator:
        #if (datetime.datetime.now() - start) >= datetime.timedelta(minutes=60):     
        #    print("It's been an hour, bye!")
        #    break
        if tweet is Hangup:
            print("Connection broken at %s, bye!" % datetime.datetime.now().strftime('%H:%M'))
            break
        elif tweet is HeartbeatTimeout:
            print("Heartbeat stopped at %s\n" % datetime.datetime.now().strftime('%H:%M'))
            break
        elif tweet is Timeout:
            print("Timed out at %s, bye!" % datetime.datetime.now().strftime('%H:%M'))
            break
        elif tweet is None:
            continue
        
        if tweet.get('coordinates') is not None:
            title = get_title(tweet)
            if title:
                tweet['title'] = title
            t = threading.Thread(target=it.indexLiveTweet, args=(json.dumps(tweet),))
            t.start()

    return

def main():
    start = datetime.datetime.now()
    retries = 3

    print("Started at %s" % start)

    iterator = connect()

    while retries > 0:
        print("Retries left: %d" % retries)
        while iterator:
            print("Iterator exists")
            consume_tweets(iterator)
            time.sleep(20)
            iterator = connect()
        retries = retries - 1

    print("Ended at %s" % datetime.datetime.now())

if __name__ == "__main__":
    main()
