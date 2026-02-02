using BepInEx;
using BepInEx.Logging;
using DontFearTheReaper.Utilities;
using HarmonyLib;
using Nautilus.Handlers;

namespace DontFearTheReaper
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Options Options { get; } = OptionsPanelHandler.RegisterModOptions<Options>();

        public static new ManualLogSource? Logger;

        internal const string GUID = "com.visolar6.dontfearthereaper";

        internal const string Name = "Dont Fear The Reaper";

        internal const string Version = "1.0.0";

        private readonly Harmony _harmony = new(GUID);

        /// <summary>
        /// Awakes the plugin (on game start).
        /// </summary>
        public void Awake()
        {
            Logger = base.Logger;
        }

        public void Start()
        {
            Logger?.LogInfo($"Patching hooks...");
            _harmony.PatchAll();

            Logger?.LogInfo($"Patching localization...");
            LanguagesHandler.GlobalPatch();

            Logger?.LogInfo($"Initialized!");
        }
    }
}
