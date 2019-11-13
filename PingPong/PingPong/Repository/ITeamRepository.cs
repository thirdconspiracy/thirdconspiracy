using System;
using System.Collections.Generic;
using System.Text;
using PingPong.Models;
using PingPong.Types;

namespace PingPong
{
    public interface ITeamRepository
    {
        TeamType AddPlayer(string name, string email, ShirtSizeType shirtSize);
        TeamsModel GetPlayers();
    }
}
