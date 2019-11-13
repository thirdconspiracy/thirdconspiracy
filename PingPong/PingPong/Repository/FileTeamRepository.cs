using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PingPong.Models;
using PingPong.Types;

namespace PingPong.Repository
{
    public class FileTeamRepository : ITeamRepository
    {
        private readonly string _adminFilename = "bnbTeam.csv";
        private readonly string _redTeamFilename = "RedTeam.csv";
        private readonly string _blackTeamFilename = "BlackTeam.csv";

        public TeamType AddPlayer(string name, string email, ShirtSizeType shirtSize)
        {
            var redTeamCount = GetTeamCount(_redTeamFilename);
            var blackTeamCount = GetTeamCount(_blackTeamFilename);

            if (blackTeamCount > redTeamCount)
            {
                WriteToRedTeam(name);
                WriteToAdmin(name, email, shirtSize, TeamType.Red);
                return TeamType.Red;
            }

            WriteToBlackTeam(name);
            WriteToAdmin(name, email, shirtSize, TeamType.Black);
            return TeamType.Black;
        }

        private int GetTeamCount(string filepath)
        {
            if (!File.Exists(filepath))
            {
                using (File.Create(filepath)) { }
            }
            return File.ReadLines(filepath).Count();

        }

        private void WriteToRedTeam(string name)
        {
            using (var fileStream = new FileStream(_redTeamFilename, FileMode.Append))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine($"{name}");
            }
        }

        private void WriteToBlackTeam(string name)
        {
            using (var fileStream = new FileStream(_blackTeamFilename, FileMode.Append))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine($"{name}");
            }
        }

        private void WriteToAdmin(string name, string email, ShirtSizeType shirtSize, TeamType color)
        {
            using (var fileStream = new FileStream(_adminFilename, FileMode.Append))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine($"{name},{email},{shirtSize:G},{color}");
            }
        }

        public TeamsModel GetPlayers()
        {
            var teamsModel = new TeamsModel();
            teamsModel.BlackTeam.AddRange(File.ReadLines(_blackTeamFilename));
            teamsModel.RedTeam.AddRange(File.ReadLines(_redTeamFilename));
            return teamsModel;

        }
    }
}
