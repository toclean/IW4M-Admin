﻿using IW4MAdmin.Application;
using SharedLibraryCore;
using SharedLibraryCore.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests
{
    [Collection("ManagerCollection")]
    public class PluginTests
    {
        readonly ApplicationManager Manager;

        public PluginTests(ManagerFixture fixture)
        {
            Manager = fixture.Manager;
        }

        [Fact]
        public void ClientSayObjectionalWordShouldWarn()
        {
            var e = new GameEvent()
            {
                Type = GameEvent.EventType.Connect,
                Origin = new Player()
                {
                    Name = $"Player1",
                    NetworkId = 1,
                    ClientNumber = 1
                },
                Owner = Manager.GetServers()[0]
            };

            Manager.GetEventHandler().AddEvent(e);
            e.OnProcessed.Wait();

            var client = Manager.GetServers()[0].Players[0];

            e = new GameEvent()
            {
                Type = GameEvent.EventType.Say,
                Origin = client,
                Data = "nigger",
                Owner = client.CurrentServer
            };

            Manager.GetEventHandler().AddEvent(e);
            e.OnProcessed.Wait();

            Assert.True(client.Warnings == 1, "client wasn't warned for objectional language");
        }
    }
}