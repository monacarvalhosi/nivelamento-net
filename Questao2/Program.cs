using Questao2.Clients;
using Questao2.Helper;
using Questao2.Models.Requests;

public class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        var url = "https://jsonmock.hackerrank.com/api/football_matches";

        var queryHomeTeam = new QueryFilter();
        queryHomeTeam.Team1 = team;
        queryHomeTeam.Year = year;

        var queryAwayTeam = new QueryFilter();
        queryAwayTeam.Team2 = team;
        queryAwayTeam.Year = year;

        var httpClient = new HttpClient();
        var helper = new HttpClientHelper(httpClient);
        var resultHomeTeam = helper.GetAllPages(url, queryHomeTeam);
        var resultAwayTeam = helper.GetAllPages(url, queryAwayTeam);


        var golsHomeTeam = StatisticsHelper.CalculateGoalsByYear(resultHomeTeam);
        var golsAwayTeam = StatisticsHelper.CalculateGoalsByYear(resultAwayTeam);

        return golsHomeTeam.GolsTeam1 + golsAwayTeam.GolsTeam2;
    }

}