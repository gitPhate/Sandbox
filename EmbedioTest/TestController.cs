using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.Utilities;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using Swan.Formatters;

namespace EmbedioTest
{
    // A very simple controller to handle People CRUD.
    // Notice how it Inherits from WebApiController and the methods have WebApiHandler attributes
    // This is for sampling purposes only.
    public sealed class TestController : WebApiController
    {
        [Route(HttpVerbs.Get, "/empty")]
        public void GetEmpty()
        {
        }

        [Route(HttpVerbs.Get, "/GetTest/{id?}")]
        public string GetTest(int? id)
        {
            return $"GetTest success [id={id}]!";
        }

        [Route(HttpVerbs.Get, "/GetTest2")]
        public string TestQuery([QueryData] NameValueCollection data)
        {
            string json = Json.Serialize(data.ToDictionary());
            return "TestQuery success!" + Environment.NewLine + json;
        }

        [Route(HttpVerbs.Post, "/PostTest")]
        public string PostTest([FormData] NameValueCollection data)
        {
            var dict0 = data.ToDictionary();
            return "PostTest success!";
        }

        [Route(HttpVerbs.Post, "/tracking")]
        public async Task Tracking()
        {
            var request = await HttpContext.GetRequestDataAsync<TrackingRequest>().ConfigureAwait(false);
            string stringRequest = Json.Serialize(request);
            File.WriteAllText("c:\\!Tmp\\tracking_x.txt", stringRequest);
        }

        [Route(HttpVerbs.Post, "/ReturnBytes")]
        public async Task ReturnBytes()
        {
            byte[] bytes = File.ReadAllBytes(@"C:\temp\UninstalItems.log");

            using (var stream = HttpContext.OpenResponseStream())
            {
                await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }
        }

        [Route(HttpVerbs.Post, "/JsonCall")]
        public async Task JsonCall()
        {
            string body;
            using (TextReader reader = HttpContext.OpenRequestText())
            {
                body = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            dynamic request = JsonConvert.DeserializeObject<dynamic>(body);

        }

        [Route(HttpVerbs.Post, "/probe")]
        public async Task<bool> Probe()
        {
            return await Task.FromResult(true).ConfigureAwait(false);
        }
    }
}