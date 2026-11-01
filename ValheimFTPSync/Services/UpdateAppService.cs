using System.Net;
using System.Text.Json;
using ValheimFTPSync.Models;
using ValheimFTPSync.Properties;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class UpdateAppService
    {
        private IAppLogger _logger;
        internal UpdateAppService(IAppLogger logger)
        {
            _logger = logger;
        }
        public async Task<AppVersion?> CheckForUpdatesAsync(bool preRelease = false)
        {
            const string latestApiUrl = "https://api.github.com/repos/evgeniystalker/ValheimFTPSync/releases/latest";
            const string allReleasesApiUrl = "https://api.github.com/repos/evgeniystalker/ValheimFTPSync/releases";
            const string commitHashUrl = "https://api.github.com/repos/evgeniystalker/ValheimFTPSync/commits/"; //+{Hash}
            using HttpClient UpdateClient = new() { Timeout = TimeSpan.FromSeconds(10), DefaultRequestHeaders = { { "User-Agent", "ValheimFTPSync" }, { "Accept", "application/vnd.github+json" } } };
            try
            {
                using var response = await UpdateClient.GetAsync(preRelease ? allReleasesApiUrl : latestApiUrl);
                // Пока нет опубликованных релизов.
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();

                using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                AppVersion? latest;
                String? tagName;
                if (json.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var maxBy = json.RootElement.EnumerateArray()
                        .Select(x =>
                        {
                            var tagName = x.GetProperty("tag_name").GetString();
                            AppVersion.TryParse(tagName, out AppVersion? appVersion);
                            return (tagName, appVersion);
                        })
                        .DefaultIfEmpty()
                        .MaxBy(x => x.appVersion);
                    latest = maxBy.appVersion;
                    tagName = maxBy.tagName;
                }
                else
                {
                    tagName = json.RootElement.GetProperty("tag_name").GetString();
                    AppVersion.TryParse(tagName, out latest);
                }

                if (latest != null)
                {
                    using var hashResponse = await UpdateClient.GetAsync(commitHashUrl + "/" + tagName);
                    if (hashResponse.IsSuccessStatusCode)
                    {
                        using var hashJson = JsonDocument.Parse(await hashResponse.Content.ReadAsStringAsync());
                        latest.CommitHash = hashJson.RootElement.GetProperty("sha").GetString();
                    }
                }

                return latest;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                _logger.Error(Messages.ErrorUpdate, ex);
                return null;
            }
        }
    }
}
