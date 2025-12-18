using MapGeneration;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lobby
{
    public class Config
    {
        [Description("Main text. Use {seconds} for countdown display.")]
        public string TitleText { get; set; } = "<color=#F0FF00><b>Waiting for players, {seconds}</b></color>";

        [Description("Text showing the number of players. Use {players} for player count.")]
        public string PlayerCountText { get; set; } = "<color=#FFA600><i>{players}</i></color>";

        [Description("Text displayed when the lobby is locked.")]
        public string ServerPauseText { get; set; } = "Server is suspended";

        [Description("Text displayed when there is one second left.")]
        public string SecondLeftText { get; set; } = "{seconds} second left";

        [Description("Text displayed when there is more than one second left.")]
        public string SecondsLeftText { get; set; } = "{seconds} seconds left";

        [Description("Text displayed when the round is starting.")]
        public string RoundStartText { get; set; } = "Round starting";

        [Description("Text displayed when there is only one player on the server.")]
        public string PlayerJoinText { get; set; } = "player joined";

        [Description("Text displayed when there is more than one player on the server.")]
        public string PlayersJoinText { get; set; } = "players joined";

        [Description("Vertical text position offset.")]
        public int VerticalPosition { get; set; } = 25;

        [Description("Title text size.")]
        public int TitleTextSize { get; set; } = 50;

        [Description("Player count text size.")]
        public int PlayerCountTextSize { get; set; } = 40;

        [Description("Enable the movement boost effect.")]
        public bool MovementBoostEnabled { get; set; } = true;

        [Description("Movement boost intensity. (Max 255)")]
        public byte MovementBoostIntensity { get; set; } = 50;

        [Description("Enable infinite stamina for players in the lobby.")]
        public bool InfiniteStamina { get; set; } = true;

        [Description("The role players will have in the lobby.")]
        public List<RoleTypeId> LobbyPlayerRole { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.ClassD,
            RoleTypeId.Scientist,
            RoleTypeId.Tutorial,
            RoleTypeId.NtfSergeant,
            RoleTypeId.NtfCaptain,
            RoleTypeId.NtfPrivate,
            RoleTypeId.NtfSpecialist,
            RoleTypeId.FacilityGuard,
            RoleTypeId.ChaosRifleman,
            RoleTypeId.ChaosMarauder,
            RoleTypeId.ChaosRepressor,
            RoleTypeId.ChaosConscript
        };

        [Description("Items given to players when spawning in the lobby. Leave empty for no items.")]
        public List<ItemType> LobbyInventory { get; set; } = new List<ItemType>
        {
            ItemType.Coin
        };

        [Description("Available spawn locations. If empty, a random custom location will be selected.")]
        public List<LobbyLocationType> SpawnLocations { get; set; } = new List<LobbyLocationType>
        {
            LobbyLocationType.Tower1,
            LobbyLocationType.Tower2,
            LobbyLocationType.Tower3,
            LobbyLocationType.Tower4,
            LobbyLocationType.Tower5,
            LobbyLocationType.GlassRoom,
            LobbyLocationType.Scp173
        };

        [Description("Custom room-based spawn locations.")]
        public List<CustomRoomLocationData> CustomRoomLocations { get; set; } = new List<CustomRoomLocationData>
        {
            new CustomRoomLocationData
            {
                RoomNameType = nameof(RoomName.EzGateA),
                OffsetX = 0f,
                OffsetY = 1f,
                OffsetZ = 0f,
                RotationX = 0f,
                RotationY = 0f,
                RotationZ = 0f
            }
        };

        [Description("Custom world-position spawn locations.")]
        public List<CustomLocationData> CustomLocations { get; set; } = new List<CustomLocationData>
        {
            new CustomLocationData
            {
                PositionX = 39.262f,
                PositionY = 315f,
                PositionZ = -31.844f,
                RotationX = 0f,
                RotationY = 0f,
                RotationZ = 0f
            }
        };
    }

    public abstract class LocationData { }

    public class CustomRoomLocationData : LocationData
    {
        public string RoomNameType { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float OffsetZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
    }

    public class CustomLocationData : LocationData
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
    }
}