using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using System.Text.Json;

namespace newWebAppLocal.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IDownstreamApi _downstreamWebApi;

        public IndexModel(ILogger<IndexModel> logger, IDownstreamApi downstreamApi)
        {
            _logger = logger;
            _downstreamWebApi = downstreamApi;
        }

        public async Task OnGet()
        {
            using var response = await _downstreamWebApi.CallApiForUserAsync("DownstreamApi").ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiResult = await response.Content.ReadFromJsonAsync<JsonDocument>().ConfigureAwait(false);
                ViewData["ApiResult"] = JsonSerializer.Serialize(apiResult, new JsonSerializerOptions { WriteIndented = true });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}: {error}");
            }
        }
    }
}