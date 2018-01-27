using System;
using System.Drawing;
using System.Linq;
using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace test {
    public class EFCoreInMemoryTests {
     
        private static Team CreateTeamAjax () {
            return new Team ("AFC Ajax", "The Lancers", "1900", "Amsterdam Arena");
        }

        [Fact]
        public void CanStoreAndMaterializeImmutableTeamNameFromDataStore () {
            var team = CreateTeamAjax ();
            var options = new DbContextOptionsBuilder<TeamContext> ().UseInMemoryDatabase ("immutableTeamName").Options;
            using (var context = new TeamContext (options)) {
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext (options)) {
                var storedTeam = context.Teams.FirstOrDefault ();
                Assert.Equal ("AFC Ajax", storedTeam.TeamName);
            }
        }
        [Fact]
        public void CanStoreAndMaterializeTeamPlayers () {
            var team = CreateTeamAjax ();
            team.AddPlayer ("André", "Onana", out string response);
          
            var options = new DbContextOptionsBuilder<TeamContext> ().UseInMemoryDatabase ("storeretrieveplayer").Options;
            using (var context = new TeamContext (options)) {
                context.Teams.Add (team);
                context.SaveChanges ();
            }
             using (var context = new TeamContext (options)) {
                var storedTeam=context.Teams.Include(t=>t.Players).FirstOrDefault();
                Assert.Equal(1,storedTeam.Players.Count());
                Assert.Equal("André Onana", storedTeam.Players.First().Name.FullName; )
            }
        }
         [Fact]
        public void TeamPreventsAddingPlayersToExistingTeamWhenPlayersNotInMemory () {
            var team = CreateTeamAjax ();
            var options = new DbContextOptionsBuilder<TeamContext> ().UseInMemoryDatabase ("protextplayers").Options;
            using (var context = new TeamContext (options)) {
                context.Teams.Add (team);
                context.SaveChanges ();
            }
        }

    }
}