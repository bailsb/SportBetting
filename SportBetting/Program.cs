using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SportBetting
{
    class NFLCriteria
    {
        private static string InFile = "NFLInputFile.csv";
        private static string OutFile = "NFLOutputFile.csv";
        
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                InFile = args[0];
            }
            Console.WriteLine("Hello World!");
            List<GameModel> values = File.ReadAllLines(InFile)
                .Select(x => GameModel.FromCsv(x)).ToList();
            Console.WriteLine("Imported Lines");
            
            // Apply Formula
            values.ForEach(ApplyKellyCriteria);
            
            WriteToFile(values);
            Console.WriteLine("Written to File");
        }

        private static void WriteToFile(List<GameModel> games)
        {
            string filePath = OutFile;

            var csv = new StringBuilder();
            games.ForEach(game =>
            {
                csv.AppendLine(string.Join(",", game.VisitingTeam.Name, game.VisitingTeam.Results, 
                    game.HomeTeam.Name, game.HomeTeam.Results));
            });
        
            File.WriteAllText(filePath, csv.ToString());
        }

        private static void ApplyKellyCriteria(GameModel game)
        {
            foreach (var oddsKey in game.HomeTeam.WinProb.Keys)
            {
                foreach (var percentKey in game.HomeTeam.WinProb.Keys)
                {
                    CalculateResultsForTeam(game.HomeTeam, oddsKey, percentKey);
                    CalculateResultsForTeam(game.VisitingTeam, oddsKey, percentKey);
                }
            }           
        }

        private static void CalculateResultsForTeam(TeamModel team, int oddsKey, int percentKey)
        {
            team.Results.Add(string.Concat(oddsKey.ToString(), percentKey.ToString()), 
                ((((team.DecimalOdds[oddsKey] - 1) * team.WinProb[percentKey]) - (1 - team.WinProb[percentKey])) /
                           (team.DecimalOdds[oddsKey] - 1)));
        }
    }
}