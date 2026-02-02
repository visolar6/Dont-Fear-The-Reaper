using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;

namespace DontFearTheReaper.Patches
{
    [HarmonyPatch(typeof(Creature))]
    public static class Creature_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Creature.Start))]
        static void Start_Postfix(Creature __instance)
        {
            if (__instance is ReaperLeviathan reaper)
            {
                reaper.gameObject.AddComponent<Mono.ReaperLeviathan.ReaperLeviathanModifications>();
                Plugin.Log?.LogInfo("[Creature_Patch] Added ReaperLeviathanModifications component to Reaper Leviathan");
            }
        }
    }
}
