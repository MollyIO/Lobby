using MapGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lobby.API
{
    public static class LobbyLocationHandler
    {
        public static GameObject Point { get; set; }

        public static readonly Dictionary<LobbyLocationType, LocationData> LocationDatas = new Dictionary<LobbyLocationType, LocationData>()
        {
            { LobbyLocationType.Tower1, new CustomRoomLocationData() { RoomNameType = nameof(RoomName.Outside), OffsetX = 162.893f, OffsetY = 20f, OffsetZ = -13.430f, RotationX = 0f, RotationY = 270f, RotationZ = 0f } },
            { LobbyLocationType.Tower2, new CustomRoomLocationData() { RoomNameType = nameof(RoomName.Outside), OffsetX = 108.03f, OffsetY = 15f, OffsetZ = -13.71f, RotationX = 0f, RotationY = 90f, RotationZ = 0f } },
            { LobbyLocationType.Tower3, new CustomRoomLocationData() { RoomNameType = nameof(RoomName.Outside), OffsetX = 39.12f, OffsetY = 15f, OffsetZ = -32f, RotationX = 0f, RotationY = 270f, RotationZ = 0f } },
            { LobbyLocationType.Tower4, new CustomRoomLocationData() { RoomNameType = nameof(RoomName.Outside), OffsetX = -15.854f, OffsetY = 15f, OffsetZ = -31.543f, RotationX = 0f, RotationY = 90f, RotationZ = 0f } },
            { LobbyLocationType.Tower5, new CustomRoomLocationData() { RoomNameType = nameof(RoomName.Outside), OffsetX = 130.43f, OffsetY = -5.6f, OffsetZ = 21f, RotationX = 0f, RotationY = 180f, RotationZ = 0f } },
            { LobbyLocationType.GlassRoom, new CustomRoomLocationData() { RoomNameType = nameof(RoomName.LczGlassroom), OffsetX = 4.8f, OffsetY = 1f, OffsetZ = 2.3f, RotationX = 0f, RotationY = 180f, RotationZ = 0f } },
            { LobbyLocationType.Scp173, new CustomRoomLocationData() { RoomNameType = nameof(RoomName.Lcz173), OffsetX = 17f, OffsetY = 13f, OffsetZ = 8f, RotationX = 0f, RotationY = -90f, RotationZ = 0f } }
        };

        public static void SetLocation(LocationData locationData)
        {
            switch (locationData)
            {
                case CustomRoomLocationData roomLocation:
                    SetRoomLocation(roomLocation);
                    break;
                case CustomLocationData worldLocation:
                    SetWorldLocation(worldLocation);
                    break;
            }
        }

        private static void SetRoomLocation(CustomRoomLocationData roomLocation)
        {
            var room = FindRoom(roomLocation.RoomNameType);

            if (room == null)
            {
                var fallbackLocation = (CustomRoomLocationData)LocationDatas[LobbyLocationType.GlassRoom];
                room = RoomIdentifier.AllRoomIdentifiers.FirstOrDefault(x => x.Name == ParseEnum<RoomName>(fallbackLocation.RoomNameType));
                roomLocation = fallbackLocation;
            }

            if (room == null)
                return;

            Point.transform.SetParent(room.transform);
            Point.transform.localPosition = new Vector3(roomLocation.OffsetX, roomLocation.OffsetY, roomLocation.OffsetZ);
            Point.transform.localRotation = Quaternion.Euler(roomLocation.RotationX, roomLocation.RotationY, roomLocation.RotationZ);
        }

        private static void SetWorldLocation(CustomLocationData worldLocation)
        {
            Point.transform.localPosition = new Vector3(worldLocation.PositionX, worldLocation.PositionY, worldLocation.PositionZ);
            Point.transform.localRotation = Quaternion.Euler(worldLocation.RotationX, worldLocation.RotationY, worldLocation.RotationZ);
        }

        private static RoomIdentifier FindRoom(string roomNameType)
        {
            if (Enum.TryParse<RoomName>(roomNameType, out var roomName))
                return RoomIdentifier.AllRoomIdentifiers.FirstOrDefault(x => x.Name == roomName);

            return RoomIdentifier.AllRoomIdentifiers.FirstOrDefault(x => x.name.Contains(roomNameType));
        }

        private static T ParseEnum<T>(string value) => (T)Enum.Parse(typeof(T), value, true);
    }
}
