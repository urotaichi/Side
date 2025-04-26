using System;
using System.Net.Http;
using System.Net.Http.Handlers;

namespace MasaoPlus
{
    public class HttpClientManager
    {
        private static HttpClient CreateHttpClient(HttpMessageHandler? handler = null)
        {
            var client = handler != null ? new HttpClient(handler) : new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            return client;
        }

        private static readonly string _userAgent = 
            $"{Global.definition.AppName}/{Global.definition.Version} ({Global.definition.AppNameFull}; Windows NT 10.0; Win64; x64)";

        private static readonly Lazy<HttpClient> _defaultClient = new(() => CreateHttpClient());

        private static readonly Lazy<ProgressMessageHandler> _progressHandler = new(() =>
        {
            var handler = new HttpClientHandler();
            return new ProgressMessageHandler(handler);
        });

        private static readonly Lazy<HttpClient> _progressClient = new(() => 
            CreateHttpClient(_progressHandler.Value));

        public static HttpClient DefaultClient => _defaultClient.Value;
        public static HttpClient ProgressClient => _progressClient.Value;
        public static ProgressMessageHandler ProgressHandler => _progressHandler.Value;
    }
}
