﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

namespace Movies.Client.Services
{
    public class PartialUpdateService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();

        public PartialUpdateService()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:5001");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task Run()
        {
            //await PatchResource();
            await PatchResourceShortcut();
        }

        public async Task PatchResource()
        {
            var patchDocument = new JsonPatchDocument<MovieForUpdate>();
            patchDocument.Replace(m => m.Title, "Updated title");
            patchDocument.Remove(m => m.Description);

            var serializedChangeSet = JsonConvert.SerializeObject(patchDocument);

            var request = new HttpRequestMessage(HttpMethod.Patch,
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
            
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(serializedChangeSet);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);

        }

        public async Task PatchResourceShortcut()
        {
            var patchDocument = new JsonPatchDocument<MovieForUpdate>();
            patchDocument.Replace(m => m.Title, "Updated title");
            patchDocument.Remove(m => m.Description);

            var response = await _httpClient.PatchAsync(
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b",
                new StringContent(JsonConvert.SerializeObject(patchDocument),
                    Encoding.UTF8,
                    "application/json-patch+json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);
        }
    }
}
