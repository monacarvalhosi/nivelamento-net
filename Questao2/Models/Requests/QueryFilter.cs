namespace Questao2.Models.Requests
{
    public class QueryFilter
    {
        public int? Year { get; set; }
        public string? Team1 { get; set; }
        public string? Team2 { get; set; }
        public int? Page { get; set; }

        public Dictionary<string, string> ToDictionary()
        {
            return new[]
            {
                ("year", Year?.ToString()),
                ("team1", Team1),
                ("team2", Team2),
                ("page", Page?.ToString())
            }
            .Where(p => !string.IsNullOrWhiteSpace(p.Item2))
            .ToDictionary(p => p.Item1, p => p.Item2!);
        }
    }
}
