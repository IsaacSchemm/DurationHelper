using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DurationHelper.Credentials {
    public interface ITwitchCredentials {
        Task<string> GetAccessTokenAsync();
    }

    public class TwitchAccessToken : ITwitchCredentials {
        private readonly string _accessToken;

        public TwitchAccessToken(string accessToken) {
            _accessToken = accessToken;
        }

        public Task<string> GetAccessTokenAsync() {
            return Task.FromResult(_accessToken);
        }
    }

    public class TwitchClientCredentials : ITwitchCredentials {
        private readonly string _clientId;
        private readonly string _clientSecret;

        private string _accessToken;
        private DateTimeOffset _expirationDate;

        public TwitchClientCredentials(string clientId, string clientSecret) {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<string> GetAccessTokenAsync() {
            if (_accessToken != null && _expirationDate < DateTimeOffset.UtcNow) {
                return _accessToken;
            }

            var req = WebRequest.CreateHttp($"https://api.twitch.tv/kraken/oauth2/token?client_id={WebUtility.UrlEncode(_clientId)}&client_secret={WebUtility.UrlEncode(_clientSecret)}&grant_type=client_credentials");
            req.Method = "POST";
            using (var resp = await req.GetResponseAsync()) {
                using (var sr = new StreamReader(resp.GetResponseStream())) {
                    string json = await sr.ReadToEndAsync();
                    var obj = JsonConvert.DeserializeAnonymousType(json, new {
                        access_token = "",
                        expires_in = 0.0
                    });
                    _accessToken = obj.access_token;
                    _expirationDate = DateTimeOffset.Now.AddSeconds(obj.expires_in);
                    return _accessToken;
                }
            }
        }
    }
}
