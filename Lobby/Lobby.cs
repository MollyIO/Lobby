using HarmonyLib;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using System;

namespace Lobby
{
    public class Lobby : Plugin<Config>
    {
        public override string Name => "Lobby";
        public override string Description => "A plugin that adds a lobby when waiting for players.";
        public override string Author => "MrAfitol";
        public override Version Version { get; } = new Version(1, 6, 2);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);

        public static Lobby Instance { get; private set; }
        public Harmony Harmony { get; private set; }

        public override void Enable()
        {
            Instance = this;
            Harmony = new Harmony("com.afitol.lobby");
            
            ServerEvents.WaitingForPlayers += EventsHandler.OnWaitingForPlayers;
            ServerEvents.RoundStarted += EventsHandler.OnRoundStarted;
        }

        public override void Disable()
        {
            ServerEvents.WaitingForPlayers -= EventsHandler.OnWaitingForPlayers;
            ServerEvents.RoundStarted -= EventsHandler.OnRoundStarted;
            EventsHandler.UnregisterHandlers();

            Harmony = null;
            Instance = null;
        }
    }
}