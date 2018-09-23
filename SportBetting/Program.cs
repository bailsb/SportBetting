using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SportBetting
{
    class NFLCriteria
    {
        private static string InFile = "C:\\Test\\NFLInputFile.csv";
        private static string OutFile = "C:\\Test\\NFLOutputFile.csv";
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            List<GameModel> values = File.ReadAllLines(InFile)
                .Select(x => GameModel.FromCsv(x)).ToList();
            Console.WriteLine("Imported Lines");
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
            CalculateResultsForTeam(game.HomeTeam);
            CalculateResultsForTeam(game.VisitingTeam);
        }

        private static void CalculateResultsForTeam(TeamModel team)
        {
            team.Results = (((team.DecimalOdds - 1) * team.WinProb) - (1 - team.WinProb)) /
                           (team.DecimalOdds - 1);
        }
    }

    public class GameModel
    {
        public TeamModel HomeTeam;
        public TeamModel VisitingTeam;

        public static GameModel FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            GameModel result = new GameModel();
            result.VisitingTeam.Name = values[0];
            result.HomeTeam.Name = values[1];
            result.VisitingTeam.DecimalOdds = Convert.ToDecimal(values[2]);
            result.HomeTeam.DecimalOdds = Convert.ToDecimal(values[3]);
            if (String.IsNullOrEmpty(values[4]))
            {
                //visitorWinProb not filled in
                result.HomeTeam.WinProb = Convert.ToDecimal(values[5]);
                result.VisitingTeam.WinProb = 1.0m - result.HomeTeam.WinProb;
            }
            else
            {
                result.VisitingTeam.WinProb = Convert.ToDecimal(values[4]);
                if (String.IsNullOrEmpty(values[5]))
                {
                    //homeWinProb is empty
                    result.HomeTeam.WinProb = 1.0m - result.VisitingTeam.WinProb;
                }
                else
                {
                    result.HomeTeam.WinProb = Convert.ToDecimal(values[5]);
                }
            }

            return result;
        }
    }

    public class TeamModel
    {
        public string Name;
        public decimal DecimalOdds;
        public decimal WinProb;
        public decimal Results;
    }
}