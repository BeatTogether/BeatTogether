using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using IPA.Utilities;
using BeatTogether.Configuration;
using UnityEngine;

namespace BeatTogether.Model
{
    internal class ServerStatusFetcher
    {
        private readonly List<ServerDetails> _serverDetails;

        private readonly ServerStatusProvider _provider;

        public ServerStatusFetcher(List<ServerDetails> servers, ServerStatusProvider provider)
        {
            _serverDetails = servers;
            _provider = provider;
        }

        public async void FetchAll()
        {
            var result = await Task.WhenAll<KeyValuePair<string, MasterServerAvailabilityData>>(_serverDetails
                .Where(server => server.StatusUri != null)
                .Select(server => FetchSingle(server)));

            foreach ((var key, var value) in result.Where(x => x.Value != null))
            {
                _provider.SetServerStatus(key, value);
            }
        }

        #region private
        private async Task<KeyValuePair<string, MasterServerAvailabilityData>> FetchSingle(ServerDetails server)
        {
            var url = server.StatusUri;
            Plugin.Logger.Info($"Fetching status for {server.ServerId} from {url}");
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30.0);

            try
            {
                var response = await httpClient.GetStringAsync(url);
                Plugin.Logger.Info($"Fetching status from {url} done.");

                return new KeyValuePair<string, MasterServerAvailabilityData>(
                    server.ServerId,
                    JsonUtility.FromJson<MasterServerAvailabilityData>(response)
                );
            }
            catch (HttpRequestException e)
            {
                Plugin.Logger.Error($"Request for {server.ServerId} failed. {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                Plugin.Logger.Error($"Request for {server.ServerId} failed. {e.Message}");
            }
            return new KeyValuePair<string, MasterServerAvailabilityData>(server.ServerId, null);
        }
        #endregion
    }
}
