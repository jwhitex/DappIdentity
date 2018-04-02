using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace DappIdentity.Test
{
    public static class Configuration
    {
        public static IConfigurationRoot Root { get; set; }
        static Configuration()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Directory.GetCurrentDirectory());
            //todo: Load Based On Environment.
            configBuilder.AddJsonFile("config.dev.json");
            Root = configBuilder.Build();
            
        }
        public static Task Reset()
        {
            //todo: Is there a callback when config changes?
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Directory.GetCurrentDirectory());
            //todo: Load Based On Environment.
            configBuilder.AddJsonFile("config.dev.json");
            Root = configBuilder.Build();
            return Task.FromResult(0);
        }
    }
}
