from decouple import config
from twitter.stream import TwitterStream, Timeout, HeartbeatTimeout, Hangup
from twitter.oauth import OAuth
from urllib import request
from bs4 import BeautifulSoup
import os, json, datetime, re

consumer_key = config('CONSUMER_KEY')
consumer_secret = config('CONSUMER_SECRET')
token = config('ACCESS_TOKEN')
token_secret =config('ACCESS_SECRET')

cur_path = os.path.dirname(os.path.realpath(__file__))

def main():
    start = datetime.datetime.now()
    tweet_folder = 'geo_tweets'
    tweet_out = open(os.path.join(cur_path, tweet_folder, "%s tweets.json" % start.strftime('%m-%d %H-%M')), 'w+')
    ts = TwitterStream(
        auth=OAuth(token, token_secret, consumer_key, consumer_secret))
    iterator = ts.statuses.sample()

    for tweet in iterator:
        if (datetime.datetime.now() - start) >= datetime.timedelta(minutes=60):     
            print("It's been an hour, bye!")
            break
        if tweet is Hangup:
            print("Connection broken at %s, bye!" % datetime.datetime.now().strftime('%H:%M'))
            break
        elif tweet is HeartbeatTimeout:
            print("Heartbeat stopped at %s, bye!" % datetime.datetime.now().strftime('%H:%M'))
            break
        elif tweet is Timeout:
            print("Timed out at %s, bye!" % datetime.datetime.now().strftime('%H:%M'))
            break
        elif tweet is None:
            continue
        
        try:
            if tweet['coordinates']:
                print('Coords founds!')
                try:
                    url = re.match(r'.*(http?s://.*).*', tweet['text']).group(1)
                    content = request.urlopen(url).read()
                    soup = BeautifulSoup(content).title.string
                    tweet['title'] = soup
                    print('URL found! Page Title: %s' % soup)
                except:
                    print("Could not open url")
                json.dump(tweet, tweet_out)
                tweet_out.write('\n')
        except:
            pass

    tweet_out.close()

main()
