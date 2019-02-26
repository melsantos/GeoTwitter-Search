from decouple import config
from twitter.stream import TwitterStream, Timeout, HeartbeatTimeout, Hangup
from twitter.oauth import OAuth
import os, json, datetime

consumer_key = config('CONSUMER_KEY')
consumer_secret = config('CONSUMER_SECRET')
token = config('ACCESS_TOKEN')
token_secret =config('ACCESS_SECRET')

def main():
    start = datetime.datetime.now()

    cur_path = os.path.dirname(os.path.realpath(__file__))
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
            print("Connection broken, bye!")
            break
        elif tweet is HeartbeatTimeout:
            print("Heartbeat stopped, bye!")
            break
        elif tweet is Timeout:
            print("Timed out, bye!")
            break
        elif tweet is None:
            continue
        
        try:
            if tweet['coordinates']:
                json.dump(tweet, tweet_out)
                tweet_out.write('\n')
        except:
            pass

    tweet_out.close()

main()