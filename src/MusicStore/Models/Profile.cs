using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace MusicStore.Models
{
    public class App
    {
        public Guid AppId { get; set; }
        public string Name { get; set; }
        public List<IDataSource> Sources { get; set; }
        public App()
        {
            AppId = Guid.NewGuid();
            Sources = new List<IDataSource>();
        }
    }

    public interface IDataSource
    {
        string AccessToken { get; set; }
        string Url { get; set; }
        Profile Profile { get; set; }
        List<Picture> Pictures { get; set; }
        List<Video> Videos { get; set; }
        List<Picture> CoverPictures { get; set; }
        List<Event> Events { get; set; }
        List<Feed> Feeds { get; set; }
        List<Audio> Songs { get; set; }
        List<Release> Releases { get; set; }
        void Load(string token);
        void LoadFromUrl(string url);
        void Refresh();
    }

    public class FacebookDataSource : IDataSource
    {
        public string AccessToken { get; set; }
        public string Url { get; set; }
        public Profile Profile { get; set; }
        public List<Picture> Pictures { get; set; }
        public List<Video> Videos { get; set; }
        public List<Picture> CoverPictures { get; set; }
        public List<Event> Events { get; set; }
        public List<Feed> Feeds { get; set; }
        public List<Audio> Songs { get; set; }
        public List<Release> Releases { get; set; }
        public List<FacebookPage> Pages { get; set; }

        public FacebookDataSource()
        {
            Pictures = new List<Picture>();
            Videos = new List<Video>();
            CoverPictures = new List<Picture>();
            Events = new List<Event>();
            Feeds = new List<Feed>();
            Songs = new List<Audio>();
            Releases = new List<Release>();
            Pages = new List<FacebookPage>();
        }

        public FacebookDataSource(string token) : this()
        {
            Load(token);
        }

        public void ParseUrl(string url)
        {
            Url = url;
        }

        public async void Load(string token)
        {
            try
            {
                HttpClient client = new HttpClient();
                var result = await client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id={Startup.Configuration.Get("ExternalProviders:Facebook:AppId")}&client_secret={Startup.Configuration.Get("ExternalProviders:Facebook:AppSecret")}&fb_exchange_token={token}");
                if (!string.IsNullOrEmpty(result))
                {
                    AccessToken = result.Split('&', '=')[1];
                    result = await client.GetStringAsync($"https://graph.facebook.com/me/accounts?access_token={AccessToken}");
                    if (!string.IsNullOrEmpty(result))
                    {
                        var dynamicObj = JsonConvert.DeserializeObject<dynamic>(result);
                        foreach (var data in dynamicObj.data)
                        {
                            var page = JsonConvert.DeserializeObject<FacebookPage>(JsonConvert.SerializeObject(data));
                            Pages.Add(page);
                        }
                    }
                }
            }
            catch
            {
                //TODO
            }

        }

        public void LoadFromUrl(string url)
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {

        }
    }

    public class FacebookPage
    {
        [JsonProperty("id")]
        public string FacebookPageId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }

    public class Profile
    {
        public string About { get; set; }
    }

    public class Event
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public List<Picture> Posters { get; set; }
    }

    public class Release
    {
        public string Title { get; set; }
        public List<Picture> Pictures { get; set; }
        public List<Audio> Songs { get; set; }
        public string BuyUrl { get; set; }
        public DateTime ReleasedOn { get; set; }
    }

    public class Audio
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string BuyUrl { get; set; }
    }

    public class Feed
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }

    public class Video
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class Picture
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
