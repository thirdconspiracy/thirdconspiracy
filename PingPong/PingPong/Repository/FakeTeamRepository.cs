using PingPong.Models;
using PingPong.Types;

namespace PingPong
{
    public class FakeTeamRepository
        : ITeamRepository
    {
        public TeamType AddPlayer(string name, string email, ShirtSizeType shirtSize)
        {
            return TeamType.Red;
        }

        public TeamsModel GetPlayers()
        {
            var model = new TeamsModel();
            model.RedTeam.Add("John Doe");
            model.RedTeam.Add("Jane Doe");
            model.BlackTeam.Add("Mr Smith");
            return model;
        }
    }
}
