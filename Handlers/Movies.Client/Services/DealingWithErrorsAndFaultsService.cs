using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marvin.StreamExtensions;
using Newtonsoft.Json;

namespace Movies.Client.Services
{
    public class DealingWithErrorsAndFaultsService : IIntegrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public async Task Run()
        {
            //await GetMovieAndDealWithInvalidResponses(_cancellationTokenSource.Token);
            await PostMovieAndHandleValidationErrors(_cancellationTokenSource.Token);
        }

        public DealingWithErrorsAndFaultsService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task GetMovieAndDealWithInvalidResponses(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("MoviesClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/movies/D0EA8B20-7309-40D5-9EBC- D95E7F25A7D0");
                
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await httpClient.SendAsync(request,
                       HttpCompletionOption.ResponseHeadersRead,
                       cancellationToken))
            {
                //Inspect the status code
                if (!response.IsSuccessStatusCode)
                {
                    //inspect the status code
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        //show this to the user
                        Console.WriteLine("The requested movie cannot be found");
                        return;
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        //trigger a login flow
                        return;
                    }

                    response.EnsureSuccessStatusCode();
                }
                var stream = await response.Content.ReadAsStreamAsync();
                var movie = stream.ReadAndDeserializeFromJson<Movie>();
            }

        }

        private async Task PostMovieAndHandleValidationErrors(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("MoviesClient");

            var movieForCreation = new MovieForCreation()
            {
                Title = "Pulp Fiction",
            };

            var serializedMovieForCreation = JsonConvert.SerializeObject(movieForCreation);

            using (var request = new HttpRequestMessage(
                       HttpMethod.Post,
                       "api/movies"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                request.Content = new StringContent(serializedMovieForCreation);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var response = await httpClient.SendAsync(request,
                           HttpCompletionOption.ResponseHeadersRead,
                           cancellationToken))
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    //Inspect the status code
                    if (!response.IsSuccessStatusCode)
                    {
                        //inspect the status code
                        if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                        {
                            //read out the response body and log it to the console window
                            var validationErrors = stream.ReadAndDeserializeFromJson();
                            Console.WriteLine(validationErrors);
                            return;
                        }
                        else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            //trigger a login flow
                            return;
                        }

                        response.EnsureSuccessStatusCode();

                        var movie = stream.ReadAndDeserializeFromJson<Movie>();
                    }
                }
            }
        }
    }
}
