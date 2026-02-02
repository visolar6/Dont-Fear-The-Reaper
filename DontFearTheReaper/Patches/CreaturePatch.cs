using HarmonyLib;

namespace DontFearTheReaper.Patches
{
    [HarmonyPatch(typeof(Creature))]
    public static class CreaturePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Creature.Start))]
        static void Start_Postfix(Creature __instance)
        {
            if (__instance is ReaperLeviathan reaper)
            {
                reaper.gameObject.AddComponent<Mono.ReaperLeviathan.ReaperLeviathanVisuals>();
                Plugin.Logger?.LogInfo("[Creature_Patch] Added ReaperLeviathanVisuals component to Reaper Leviathan");

                reaper.gameObject.AddComponent<Mono.ReaperLeviathan.ReaperLeviathanSpeed>();
                Plugin.Logger?.LogInfo("[Creature_Patch] Added ReaperLeviathanSpeed component to Reaper Leviathan");

                reaper.gameObject.AddComponent<Mono.ReaperLeviathan.ReaperLeviathanRoar>();
                Plugin.Logger?.LogInfo("[Creature_Patch] Added ReaperLeviathanRoar component to Reaper Leviathan");
            }
        }
    }
}
