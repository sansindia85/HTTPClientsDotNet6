﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class HttpClientFactoryInstanceManagementService : IIntegrationService
    {  
        private readonly CancellationTokenSource _cancellationTokenSource = 
            new CancellationTokenSource();

        public async Task Run()
        {
            //await TestDisposeHttpClient(_cancellationTokenSource.Token);
            await TestReuseHttpClient(_cancellationTokenSource.Token);
        }

        private async Task TestDisposeHttpClient(CancellationToken cancellationToken)
        {
            for (var index = 0; index < 10; index++)
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(
                        HttpMethod.Get,
                        "https://www.google.com");

                    using (var response = await httpClient.SendAsync(request,
                               HttpCompletionOption.ResponseHeadersRead,
                               cancellationToken))
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        response.EnsureSuccessStatusCode();

                        Console.WriteLine($"Request completed with status code {response.StatusCode}");
                    }
                }
            }

        }

        private async Task TestReuseHttpClient(CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();

            for (var index = 0; index < 10; index++)
            {
                var request = new HttpRequestMessage(
                        HttpMethod.Get,
                        "https://www.google.com");

                using (var response = await httpClient.SendAsync(request,
                               HttpCompletionOption.ResponseHeadersRead,
                               cancellationToken))
                {
                        var stream = await response.Content.ReadAsStreamAsync();
                        response.EnsureSuccessStatusCode();

                        Console.WriteLine($"Request completed with status code {response.StatusCode}");
                }
            }

        }
    }
}
