using LabApi.Events.Arguments.PlayerEvents;

namespace Lobby
{
    public class RestrictionsHandler
    {
        private static bool IsLobby => EventsHandler.IsLobby;

        public static void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs ev)
        {
            if (IsLobby)
                ev.IsAllowed = false;
        }

        public static void OnPlayerInteractingElevator(PlayerInteractingElevatorEventArgs ev)
        {
            if (IsLobby)
                ev.IsAllowed = false;
        }

        public static void OnPlayerSearchingPickup(PlayerSearchingPickupEventArgs ev)
        {
            if (IsLobby)
                ev.IsAllowed = false;
        }

        public static void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev)
        {
            if (IsLobby)
                ev.IsAllowed = false;
        }

        public static void OnPlayerDroppingAmmo(PlayerDroppingAmmoEventArgs ev)
        {
            if (IsLobby)
                ev.IsAllowed = false;
        }

        public static void OnPlayerThrowingItem(PlayerThrowingItemEventArgs ev)
        {
            if (IsLobby)
                ev.IsAllowed = false;
        }
    }
}