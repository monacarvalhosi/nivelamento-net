using Questao2.Models;
using Questao2.Models.Responses;

namespace Questao2.Helper
{
    public static class StatisticsHelper
    {
        public static GoalsYear CalculateGoalsByYear(ResultGames resultGames)
        {
            var gamesYear = resultGames.Data.ToList();

            var goalsYear = gamesYear.Aggregate(
                new GoalsYear(),
                (acc, jogo) =>
                {
                    if (int.TryParse(jogo.Team1Goals, out var golsTeam1))
                        acc.GolsTeam1 += golsTeam1;

                    if (int.TryParse(jogo.Team2Goals, out var golsTeam2))
                        acc.GolsTeam2 += golsTeam2;

                    return acc;
                });

            return goalsYear;
        }
    }
}