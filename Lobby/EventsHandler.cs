using CentralAuth;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using Lobby.API;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lobby
{
    public class EventsHandler
    {
        public static bool IsLobby { get; private set; } = true;
        private static CoroutineHandle _lobbyTimerCoroutine;

        public static void OnWaitingForPlayers()
        {
            IsLobby = true;
            LobbyLocationHandler.Point = new GameObject("LobbyPoint");
            
            Lobby.Instance.Harmony.PatchAll();
            RegisterHandlers();
            InitializeSpawnLocation();

            Timing.CallDelayed(0.1f, () =>
            {
                HideStartRoundButton();
                StartLobbyTimer();
            });
        }

        private static void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            if (!IsLobby)
                return;

            if (!IsValidLobbyState())
                return;

            Timing.CallDelayed(1f, () => SpawnPlayerInLobby(ev.Player));
        }

        public static void OnRoundStarted()
        {
            IsLobby = false;
            UnregisterHandlers();
            StopLobbyTimer();
            ResetPlayers();
            
            Lobby.Instance.Harmony.UnpatchAll("com.afitol.lobby");
        }

        private static void RegisterHandlers()
        {
            PlayerEvents.InteractingDoor += RestrictionsHandler.OnPlayerInteractingDoor;
            PlayerEvents.InteractingElevator += RestrictionsHandler.OnPlayerInteractingElevator;
            PlayerEvents.SearchingPickup += RestrictionsHandler.OnPlayerSearchingPickup;
            PlayerEvents.DroppingItem += RestrictionsHandler.OnPlayerDroppingItem;
            PlayerEvents.DroppingAmmo += RestrictionsHandler.OnPlayerDroppingAmmo;
            PlayerEvents.ThrowingItem += RestrictionsHandler.OnPlayerThrowingItem;
            PlayerEvents.Joined += OnPlayerJoined;
        }

        public static void UnregisterHandlers()
        {
            PlayerEvents.InteractingDoor -= RestrictionsHandler.OnPlayerInteractingDoor;
            PlayerEvents.InteractingElevator -= RestrictionsHandler.OnPlayerInteractingElevator;
            PlayerEvents.SearchingPickup -= RestrictionsHandler.OnPlayerSearchingPickup;
            PlayerEvents.DroppingItem -= RestrictionsHandler.OnPlayerDroppingItem;
            PlayerEvents.DroppingAmmo -= RestrictionsHandler.OnPlayerDroppingAmmo;
            PlayerEvents.ThrowingItem -= RestrictionsHandler.OnPlayerThrowingItem;
            PlayerEvents.Joined -= OnPlayerJoined;
        }

        private static bool IsValidLobbyState()
        {
            var timer = GameCore.RoundStart.singleton.NetworkTimer;
            return timer > 1 || timer == -2;
        }

        private static void HideStartRoundButton()
        {
            var startRound = GameObject.Find("StartRound");
            if (startRound != null)
                startRound.transform.localScale = Vector3.zero;
        }

        private static void StartLobbyTimer()
        {
            if (_lobbyTimerCoroutine.IsRunning)
                Timing.KillCoroutines(_lobbyTimerCoroutine);

            _lobbyTimerCoroutine = Timing.RunCoroutine(LobbyTimerCoroutine());
        }

        private static void StopLobbyTimer()
        {
            Timing.CallDelayed(1f, () =>
            {
                if (_lobbyTimerCoroutine.IsRunning)
                    Timing.KillCoroutines(_lobbyTimerCoroutine);
            });
        }

        private static void SpawnPlayerInLobby(Player player)
        {
            if (player.IsOverwatchEnabled)
                return;

            var config = Lobby.Instance.Config;
            if (config != null)
            {
                player.SetRole(config.LobbyPlayerRole[Random.Range(0, config.LobbyPlayerRole.Count)], RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                player.IsGodModeEnabled = true;

                foreach (var item in config.LobbyInventory)
                    player.AddItem(item);

                Timing.CallDelayed(0.1f, () =>
                {
                    player.Position = LobbyLocationHandler.Point.transform.position;
                    player.Rotation = LobbyLocationHandler.Point.transform.rotation;

                    if (config.MovementBoostEnabled)
                        player.EnableEffect<MovementBoost>(config.MovementBoostIntensity);
                });
            }
        }

        private static void ResetPlayers()
        {
            var config = Lobby.Instance.Config;
            foreach (var player in Player.List.Where(x => x.Role != RoleTypeId.Overwatch))
            {
                player.SetRole(RoleTypeId.Spectator);

                Timing.CallDelayed(0.1f, () =>
                {
                    player.IsGodModeEnabled = false;
                    
                    if (config != null && config.MovementBoostEnabled)
                        player.DisableEffect<MovementBoost>();
                });
            }
        }

        private static void InitializeSpawnLocation()
        {
            var locationList = new List<LocationData>();
            var config = Lobby.Instance.Config;

            if (config != null && config.SpawnLocations?.Count > 0)
            {
                locationList.AddRange(config.SpawnLocations
                    .Where(x => LobbyLocationHandler.LocationDatas.ContainsKey(x))
                    .Select(x => LobbyLocationHandler.LocationDatas[x]));
            }

            if (config != null && config.CustomLocations?.Count > 0)
                locationList.AddRange(config.CustomLocations);

            if (config != null && config.CustomRoomLocations?.Count > 0)
                locationList.AddRange(config.CustomRoomLocations);

            if (locationList.Count == 0)
            {
                var randomIndex = Random.Range(0, LobbyLocationHandler.LocationDatas.Count);
                locationList.Add(LobbyLocationHandler.LocationDatas.ElementAt(randomIndex).Value);
            }

            LobbyLocationHandler.SetLocation(locationList.RandomItem());
        }

        private static IEnumerator<float> LobbyTimerCoroutine()
        {
            var config = Lobby.Instance.Config;

            while (!Round.IsRoundStarted)
            {
                var displayText = BuildDisplayText(config);
                BroadcastToPlayers(displayText);

                yield return Timing.WaitForSeconds(1f);
            }
        }

        private static string BuildDisplayText(Config config)
        {
            var text = string.Empty;

            if (config.VerticalPosition < 0)
            {
                for (var i = 0; i < ~config.VerticalPosition; i++)
                    text += "\n";
            }

            text += $"<size={config.TitleTextSize}>{config.TitleText}</size>";
            text += $"\n<size={config.PlayerCountTextSize}>{config.PlayerCountText}</size>";

            text = ReplaceTimerPlaceholder(text, config);
            text = ReplacePlayerCountPlaceholder(text, config);

            if (config.VerticalPosition >= 0)
            {
                for (var i = 0; i < config.VerticalPosition; i++)
                    text += "\n";
            }

            return text;
        }

        private static string ReplaceTimerPlaceholder(string text, Config config)
        {
            var networkTimer = GameCore.RoundStart.singleton.NetworkTimer;

            switch (networkTimer)
            {
                case -2:
                    return text.Replace("{seconds}", config.ServerPauseText);
                case -1:
                case 0:
                    return text.Replace("{seconds}", config.RoundStartText);
                case 1:
                    return text.Replace("{seconds}",
                        config.SecondLeftText.Replace("{seconds}", networkTimer.ToString()));
                default:
                    return text.Replace("{seconds}",
                        config.SecondsLeftText.Replace("{seconds}", networkTimer.ToString()));
            }
        }

        private static string ReplacePlayerCountPlaceholder(string text, Config config)
        {
            var playerText = Player.Count == 1 ? config.PlayerJoinText : config.PlayersJoinText;
            return text.Replace("{players}", $"{Player.Count} {playerText}");
        }

        private static void BroadcastToPlayers(string text)
        {
            foreach (var player in Player.List)
            {
                if (player.ReferenceHub.Mode != ClientInstanceMode.Unverified && 
                    player.ReferenceHub.Mode != ClientInstanceMode.DedicatedServer)
                {
                    player.SendHint(text, 1.05f);
                }
            }
        }
    }
}
