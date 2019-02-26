from decouple import config
from twitter.stream import TwitterStream, Timeout, HeartbeatTimeout, Hangup
from twitter.oauth import OAuth
import os, json

consumer_key = config('CONSUMER_KEY')
consumer_secret = config('CONSUMER_SECRET')
token = config('ACCESS_TOKEN')
token_secret =config('ACCESS_SECRET')

def main():
    cur_path = os.path.dirname(os.path.realpath(__file__))
    tweet_out = open(os.path.join(cur_path, "geo_tweets.json"), 'w+')

    ts = TwitterStream(
        auth=OAuth(token, token_secret, consumer_key, consumer_secret))
    iterator = ts.statuses.sample()
    i = 0

    for tweet in iterator:
        if tweet is Hangup:
            sample.write("Connection broken, bye!")
            break
        elif tweet is HeartbeatTimeout:
            sample.write("Heartbeat stopped, bye!")
            break
        elif tweet is Timeout:
            sample.write("Timed out, bye!")
            break
        elif tweet is None:
            continue
        
        try:
            if tweet['coordinates']:
                json.dump(tweet, tweet_out)
                tweet_out.write('\n')
                i = i + 1
                print(i)
        except:
            pass

        if i >= 20:
            break

    tweet_out.close()

main()