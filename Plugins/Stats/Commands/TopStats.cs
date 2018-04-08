﻿using SharedLibraryCore;
using SharedLibraryCore.Objects;
using SharedLibraryCore.Services;
using IW4MAdmin.Plugins.Stats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW4MAdmin.Plugins.Stats.Commands
{
    class TopStats : Command
    {
        public TopStats() : base("topstats", "view the top 5 players on this server", "ts", Player.Permission.User, false) { }

        public override async Task ExecuteAsync(Event E)
        {
            var statsSvc = new GenericRepository<EFClientStatistics>();
            int serverId = E.Owner.GetHashCode();
            var iqStats = statsSvc.GetQuery(cs => cs.ServerId == serverId);

            var topStats = iqStats.Where(cs => cs.Skill > 100)
                .Where(cs => cs.TimePlayed >= 3600)
                .Where(cs => cs.Client.Level != Player.Permission.Banned)
                .OrderByDescending(cs => cs.Skill)
                .Take(5)
                .ToList();

            if (!E.Message.IsBroadcastCommand())
            {
                await E.Origin.Tell("^5--Top Players--");

                foreach (var stat in topStats)
                    await E.Origin.Tell($"^3{stat.Client.Name}^7 - ^5{stat.KDR} ^7KDR | ^5{stat.Skill} ^7SKILL");
            }
            else
            {
                await E.Owner.Broadcast("^5--Top Players--");

                foreach (var stat in topStats)
                    await E.Owner.Broadcast($"^3{stat.Client.Name}^7 - ^5{stat.KDR} ^7KDR | ^5{stat.Skill} ^7SKILL");
            }
        }
    }
}