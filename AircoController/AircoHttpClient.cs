using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AircoController
{
    public class AircoHttpClient
    {
        private HttpClient _client;
        private JsonMediaTypeFormatter _formatter;
        private List<MediaTypeFormatter> _formatters;

        public AircoHttpClient()
        {
            InitializeHttpClient();
            InitializeJsonFormatter();
        }

        private void InitializeHttpClient()
        {
#if XXDEBUG
            _client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
#else
            _client = new HttpClient();
#endif
            _client.BaseAddress = new Uri("https://accsmart.panasonic.com/");

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("X-APP-TYPE", "1");
            _client.DefaultRequestHeaders.Add("X-APP-VERSION", "2.0.0");
            _client.DefaultRequestHeaders.Add("User-Agent", "G-RAC");
        }

        private void InitializeJsonFormatter()
        {
            _formatter = new JsonMediaTypeFormatter();
            _formatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            _formatters = new List<MediaTypeFormatter> { _formatter };
        }

        public void SetAuthorizationHeader(string token)
        {
            _client.DefaultRequestHeaders.Add("X-User-Authorization", token);
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            // the api requires a Content-Type header, even for get requests (which do not have content...).
            request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);

            await ValidateResponse(response);

            return await response.Content.ReadAsAsync<T>();
        }

        public async Task<TResponse> PostAsync<TResponse, TRequest>(string uri, TRequest request)
        {
            var response = await _client.PostAsync(uri, request, _formatter);

            await ValidateResponse(response);

            return await response.Content.ReadAsAsync<TResponse>();
        }

        private async Task ValidateResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseMsg = await response.Content.ReadAsStringAsync();

                throw new Exception($"Got HTTP {response.StatusCode}: {responseMsg}");
            }
        }
    }

    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Request:");
            Console.WriteLine(request.ToString());
            if (request.Content != null)
            {
                Console.WriteLine(await request.Content.ReadAsStringAsync());
            }
            Console.WriteLine();

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            Console.WriteLine("Response:");
            Console.WriteLine(response.ToString());
            if (response.Content != null)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            Console.WriteLine();

            return response;
        }
    }
}
