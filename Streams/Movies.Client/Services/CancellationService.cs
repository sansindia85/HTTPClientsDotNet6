using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class CancellationService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient(
            new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip

            });
        public async Task Run()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:5001");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }
    }
}
