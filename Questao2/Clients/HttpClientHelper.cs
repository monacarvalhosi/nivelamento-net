using System.Text.Json;
using Questao2.Models.Requests;
using Questao2.Models.Responses;

namespace Questao2.Clients
{
    public class HttpClientHelper
    {
        private readonly HttpClient _httpClient;

        public HttpClientHelper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ResultGames GetAllPages(string baseUrl, QueryFilter filtro)
        {
            var allMatches = new List<Game>();
            int currentPage = 1;
            int totalPages;

            do
            {
                var parameters = filtro.ToDictionary();
                parameters.Add("page", currentPage.ToString());

                var query = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
                var fullUrl = $"{baseUrl}?{query}";

                var response = _httpClient.GetAsync(fullUrl).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Result not succes, status code: {response.StatusCode}");

                var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var result = JsonSerializer.Deserialize<ResultGames>(json);

                if (result?.Data != null)
                    allMatches.AddRange(result.Data);

                totalPages = result?.TotalPages ?? 0;
                currentPage++;

            } while (currentPage <= totalPages);

            return new ResultGames
            {
                Data = allMatches,
                Page = 1,
                PerPage = allMatches.Count,
                Total = allMatches.Count,
                TotalPages = 1
            };
        }
    }
}
