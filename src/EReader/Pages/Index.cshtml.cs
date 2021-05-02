using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using WebDav;

namespace EReader.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebDavClient _webDavClient;
        private readonly EReaderSettings _settings;

        public IndexModel(HttpClient httpClient, IOptionsMonitor<EReaderSettings> settings)
        {
            _settings = settings.CurrentValue;
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("BASIC",
                                              Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.User}:{_settings.Password}")));
            _webDavClient = new WebDavClient(httpClient);
        }

        public List<File> Files { get; private set; }

        public async Task OnGetAsync()
        {
            Files = await GetFilesAsync();
        }

        public async Task<IActionResult> OnGetDownloadAsync(int id)
        {
            var file = (await GetFilesAsync()).ElementAt(id);
            var response = await _webDavClient.GetRawFile(file.Uri);
            return File(response.Stream, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
        }

        private async Task<List<File>> GetFilesAsync()
        {
            var results = new List<File>();

            var response = await _webDavClient.Propfind(_settings.WebDavUrl);
            if (response.IsSuccessful)
            {
                var fileUris = response.Resources.Skip(1).Select(x => x.Uri); // first entry is the folder itself and can therefore be skipped

                var webDavUri = new Uri(_settings.WebDavUrl);
                var host = webDavUri.Host;
                var scheme = webDavUri.Scheme;
                var port = webDavUri.Port;

                results = fileUris.Select(x => new File { Uri = new Uri(new UriBuilder(scheme, host, port).Uri, x) }).ToList();
            }

            return results;
        }
    }
}