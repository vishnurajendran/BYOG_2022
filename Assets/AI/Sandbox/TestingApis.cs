using System;
using System.Net.Http;
using UnityEngine;

namespace AI.Sandbox
{
    public class TestingApis : MonoBehaviour
    {
        public string word;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                DoThings(word);
            }
        }

        private async void DoThings(string word)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://twinword-word-associations-v1.p.rapidapi.com/associations/?entry=sound"),
                Headers =
                {
                    { "X-RapidAPI-Key", "d992c872b2msha392a64d75d1405p10c9dajsn16a899831534" },
                    { "X-RapidAPI-Host", "twinword-word-associations-v1.p.rapidapi.com" },
                },
            };
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Debug.Log(body);
        }
    }
}