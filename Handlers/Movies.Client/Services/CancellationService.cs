﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Marvin.StreamExtensions;

namespace Movies.Client.Services
{
    public class CancellationService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient(
            new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip

            });

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public CancellationService()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:5001");
            _httpClient.Timeout = new TimeSpan(0, 0, 2);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task Run()
        {
            _cancellationTokenSource.CancelAfter(1000);
            //await GetTrailerAndCancel(_cancellationTokenSource.Token);
            await GetTrailerAndHandleTimeout();
        }

        private async Task GetTrailerAndCancel(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            try
            {
                using (var response = await _httpClient.SendAsync(request,
                           HttpCompletionOption.ResponseHeadersRead,
                           cancellationToken))
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    response.EnsureSuccessStatusCode();
                    var trailer = stream.ReadAndDeserializeFromJson<Trailer>();
                }

            }
            catch (OperationCanceledException operationCanceledException)
            {
                Console.WriteLine($"An operation was cancelled with message {operationCanceledException.Message}");
                //additional cleanup...depends on use case.
            }
            

        }

        private async Task GetTrailerAndHandleTimeout()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            try
            {
                using (var response = await _httpClient.SendAsync(request,
                           HttpCompletionOption.ResponseHeadersRead))
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    response.EnsureSuccessStatusCode();
                    var trailer = stream.ReadAndDeserializeFromJson<Trailer>();
                }

            }
            catch (OperationCanceledException operationCanceledException)
            {
                Console.WriteLine($"An operation was cancelled with message {operationCanceledException.Message}");
                //additional cleanup...depends on use case.
            }
        }
    }
}
