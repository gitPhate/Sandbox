using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Authentication;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Swan.Formatters;
using Swan.Logging;
namespace EmbedioTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://localhost:9696/";
            if (args.Length > 0)
                url = args[0];

            // Our web server is disposable.
            using (var server = CreateWebServer(url))
            {
                // Once we've registered our modules and configured them, we call the RunAsync() method.
                server.RunAsync();

                var browser = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }
                };
                browser.Start();
                // Wait for any key to be pressed before disposing of our web server.
                // In a service, we'd manage the lifecycle of our web server using
                // something like a BackgroundWorker or a ManualResetEvent.
                Console.ReadKey(true);
            }
        }        

        // Create and configure our web server.
        private static WebServer CreateWebServer(string url)
        {
            var server = 
                new WebServer(o => o
                    .WithMicrosoftHttpListener()
                    .WithUrlPrefix(url)
                )
                .WithLocalSessionManager()
                .WithModule(new BasicAuthenticationModule("/").WithAccount("user", "password"))
                .WithWebApi("/api", m => m.WithController<TestController>())
                ;

            // Listen for state changes.
            server.StateChanged += (s, e) => $"WebServer New State - {e.NewState}".Info();

            return server;
        }
    }
}
