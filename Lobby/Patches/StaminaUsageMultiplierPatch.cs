using HarmonyLib;
using InventorySystem;

namespace Lobby.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.StaminaUsageMultiplier), MethodType.Getter)]
    public class StaminaUsageMultiplierPatch
    {
        private static void Postfix(ref float __result)
        {
            if (Lobby.Instance.Config?.InfiniteStamina == true && EventsHandler.IsLobby)
                __result = 0f;
        }
    }
}
