using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PingPong.Repository;
using PingPong.Types;

namespace PingPong.Test
{
    public class TeamRepoTests
    {
        [Test, Explicit]
        public void TestFileWrite()
        {
            var repo = new FileTeamRepository();
            repo.AddPlayer("one", "one@email.com", ShirtSizeType.XSmall);
            repo.AddPlayer("two", "two@email.com", ShirtSizeType.Small);
            repo.AddPlayer("three", "three@email.com", ShirtSizeType.Medium);

            var teams = repo.GetPlayers();

            Assert.AreEqual(2, teams.BlackTeam.Count);
            Assert.Contains("one", teams.BlackTeam);
            Assert.Contains("three", teams.BlackTeam);

            Assert.AreEqual(1, teams.RedTeam.Count);
            Assert.Contains("two", teams.RedTeam);

        }
    }
}
