using System;
using System.Collections.Generic;


namespace SportBetting
{
    public class GameModel
    {
        public TeamModel HomeTeam;
        public TeamModel VisitingTeam;

        public static GameModel FromCsv(string csvLine)
        {
            int winProbsCount = 0;
            int decimalOddsCount = 0;
            
            string[] values = csvLine.Split(',');
            GameModel result = new GameModel();
            result.VisitingTeam.Name = values[0];
            result.HomeTeam.Name = values[1];
            int iter = 2;
            while (values.Length >= (iter + 2))
            {
                if (Convert.ToDecimal(values[iter]) > 1.0m && Convert.ToDecimal(values[iter + 1]) > 1.0m)
                {
                    ReadDecimalOdds(values[iter], values[iter + 1], result, decimalOddsCount);
                    decimalOddsCount++;
                }
                else if (Convert.ToDecimal(values[iter]) < 1.0m && Convert.ToDecimal(values[iter]) > 0.0m
                         || Convert.ToDecimal(values[iter + 1]) < 1.0m && Convert.ToDecimal(values[iter + 1]) > 0.0m)
                {
                    ReadWinProb(values[iter], values[iter + 1], result, winProbsCount);
                    winProbsCount++;
                }
                else
                {
                    Console.WriteLine("Incorrect values in column {0} and/or {1}", iter, iter + 1);
                    return result;
                }

                iter += 2;
            }

            return result;
        }
        
        // Reads the next two spaces of csv line as Win Probabilities
        // Input
        // string visitorVal, homeVal = Values of the two spaces in the line. Only one needs to be non-empty
        // the other will be calculated
        // Vales will be put in the game model under the 'winProbKey
        private static void ReadWinProb(string visitorVal, string homeVal, GameModel game, int winProbKey)
        {
            if (String.IsNullOrEmpty(visitorVal))
            {
                game.HomeTeam.WinProb.Add(winProbKey, Convert.ToDecimal(homeVal));
                game.VisitingTeam.WinProb.Add(winProbKey, (1.0m - game.HomeTeam.WinProb[winProbKey]));
            }
            else
            {
                game.VisitingTeam.WinProb.Add(winProbKey, Convert.ToDecimal(visitorVal));
                if (String.IsNullOrEmpty(homeVal))
                {
                    game.HomeTeam.WinProb.Add(winProbKey, 1.0m - game.VisitingTeam.WinProb[winProbKey]);
                }
                else
                {
                    game.HomeTeam.WinProb.Add(winProbKey, Convert.ToDecimal(homeVal));
                }
            }
        }
        
        // Reads the next two spaces of csv line as Decimal Odds
        // Input
        // string visitorVal, homeVal = Values of the two spaces in the line
        // Vales will be put in the game model under the 'winProbKey
        private static void ReadDecimalOdds(string visitorVal, string homeVal, GameModel game, int decimalOddsKey)
        {
            game.VisitingTeam.DecimalOdds.Add(decimalOddsKey, Convert.ToDecimal(visitorVal));
            game.HomeTeam.DecimalOdds.Add(decimalOddsKey, Convert.ToDecimal(homeVal));           
        }
    }
    
    public class TeamModel
    {
        public string Name;
        public Dictionary<int, decimal> DecimalOdds;
        public Dictionary<int, decimal> WinProb;
        public Dictionary<string, decimal> Results;
    }
}