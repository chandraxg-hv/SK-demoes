using System;
using System.ComponentModel;
using Microsoft.SemanticKernel;
using SimpleFeedReader;

namespace ConsoleApp1
{
    public class NewsPlugin
    {
        [KernelFunction("gets_news")]
        [Description("Get the news feed from the New York Times for today's date")]
        [return : Description("A list of news items")]

        public async Task<List<FeedItem>> GetNewsFeed(Kernel kernel, string category)
        {
            var reader = new FeedReader();
            var feed = await reader.RetrieveFeedAsync ($"https://rss.nytimes.com/services/xml/rss/nyt/{category}.xml");
            
            //convert the feed into a list
            var feedList = new List<FeedItem>();
            foreach (var item in feed)
            {
                feedList.Add(item);
            }
            return feedList;

        }
    }
}
