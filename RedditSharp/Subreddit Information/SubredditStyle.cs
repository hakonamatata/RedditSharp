﻿#pragma warning disable 1591
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RedditSharp.Things;
using System.Threading.Tasks;
using System.Net;

namespace RedditSharp
{
    public class SubredditStyle : RedditObject
    {
        private const string UploadImageUrl = "/api/upload_sr_img";
        private const string UpdateCssUrl = "/api/subreddit_stylesheet";

        #pragma warning disable 1591
        public SubredditStyle(Subreddit subreddit) : base(subreddit?.Reddit)
        {
            Subreddit = subreddit;
        }
        #pragma warning restore 1591

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subreddit">Subreddit.</param>
        /// <param name="json">json payload.</param>
        public SubredditStyle(Subreddit subreddit, JToken json) : this(subreddit)
        {
            Images = new List<SubredditImage>();
            var data = json["data"];
            CSS = WebUtility.HtmlDecode(data["stylesheet"].Value<string>());
            foreach (var image in data["images"])
            {
                Images.Add(new SubredditImage(this, image["link"].Value<string>(),
                    image["name"].Value<string>(), image["url"].Value<string>()));
            }
        }

        /// <summary>
        /// Subreddit stylesheet.
        /// </summary>
        public string CSS { get; set; }

        /// <summary>
        /// List of images for the stylesheet.
        /// </summary>
        public List<SubredditImage> Images { get; set; }

        /// <summary>
        /// Subreddit.
        /// </summary>
        public Subreddit Subreddit { get; set; }

        /// <summary>
        /// Update the css.
        /// </summary>
        public async Task UpdateCssAsync()
        {
            await WebAgent.Post(UpdateCssUrl, new
            {
                op = "save",
                stylesheet_contents = CSS,
                uh = Reddit.User.Modhash,
                api_type = "json",
                r = Subreddit.Name
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Upload an image to reddit.
        /// </summary>
        /// <param name="name">name of image.</param>
        /// <param name="imageType"><see cref="ImageType"/> of image</param>
        /// <param name="file">image buffer</param>
        public async Task UploadImageAsync(string name, ImageType imageType, byte[] file)
        {
            var request = WebAgent.CreateRequest(UploadImageUrl, "POST");
            var formData = new MultipartFormBuilder(request);
            formData.AddDynamic(new
                {
                    name,
                    uh = Reddit.User.Modhash,
                    r = Subreddit.Name,
                    formid = "image-upload",
                    img_type = imageType == ImageType.PNG ? "png" : "jpg",
                    upload = ""
                });
            formData.AddFile("file", "foo.png", file, imageType == ImageType.PNG ? "image/png" : "image/jpeg");
            formData.Finish();
            var response = await WebAgent.GetResponseAsync(request).ConfigureAwait(false);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            // TODO: Detect errors
        }
    }

    public enum ImageType
    {
        PNG,
        JPEG
    }
}
#pragma warning restore 1591
