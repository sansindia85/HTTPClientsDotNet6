using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marvin.StreamExtensions;

namespace Movies.Client
{
    public class TestableClassWithApiAccess
    {
        private readonly HttpClient _httpClient;

        public TestableClassWithApiAccess(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task GetMovie(CancellationToken cancellationToken)
        {
            
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/movies/3d2880ae-5ba6-417c-845d-f4ebfd4bcac7");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await _httpClient.SendAsync(request,
                       HttpCompletionOption.ResponseHeadersRead,
                       cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                if (!response.IsSuccessStatusCode)
                {
                    //inspect the status code
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        //show this to the user
                        Console.WriteLine("The requested movie cannot be found.");
                        return;
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // trigger a login flow
                        throw new UnauthorizedApiAccessException();
                    }
                    response.EnsureSuccessStatusCode();
                }

                var movie = stream.ReadAndDeserializeFromJson<Movie>();

            }
        }
    }

    public class UnauthorizedApiAccessException : Exception
    {
    }
}
