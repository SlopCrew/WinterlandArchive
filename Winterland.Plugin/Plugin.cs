using BepInEx;
using System;
using Reptile;
using CommonAPI;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using HarmonyLib;
using System.IO;
using System.Reflection;
using System.Linq;
using Winterland.Common;
using System.Runtime.CompilerServices;

namespace Winterland.Plugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(WinterCharacters.CrewBoomGUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin {
        public static Plugin Instance;
        public static ManualLogSource Log = null;
        internal static bool DynamicCameraInstalled = false;
        internal static bool BunchOfEmotesInstalled = false;

        // Hack: we must reference dependent assemblies from a class that's guaranteed to execute or else they don't
        // load and MonoBehaviours are missing.
        private static Type ForceLoadMapStationCommonAssembly = typeof(Winterland.MapStation.Common.Dependencies.AssemblyDependencies);
        private static Type ForceLoadMapStationPluginAssembly = typeof(Winterland.MapStation.Plugin.Dependencies.AssemblyDependencies);

        private void Awake() {
            Instance = this;
            try {
                Initialize();
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} is loaded!");
            }
            catch(Exception e) {
                Logger.LogError($"Plugin {PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} failed to load!{Environment.NewLine}{e}");
            }
        }

        private void Initialize() {
            // Our local dev workflow uses a nested folder, but at release we realized last-minute it was breaking R2.
            // So this code will detect the nested folder and use it if it exists.
            var oldAssetBundlesFolder = Path.Combine(Path.GetDirectoryName(Info.Location), "AssetBundles");
            var newAssetBundlesFolder = Path.GetDirectoryName(Info.Location);
            var assetBundlesFolder = newAssetBundlesFolder;
            if(Directory.Exists(oldAssetBundlesFolder)) {
                assetBundlesFolder = oldAssetBundlesFolder;
            }
            var winterAssets = new WinterAssets(assetBundlesFolder);
            new WinterConfig(Config);
            WinterCharacters.Initialize();
            ObjectiveDatabase.Initialize(winterAssets.WinterBundle);
            NetManager.Create();
#if WINTER_DEBUG
            DebugUI.Create(WinterConfig.Instance.DebugUI.Value);
            NetManagerDebugUI.Create();
            TreeDebugUI.Create();
            LocalProgressDebugUI.Create();
            DebugShapeDebugUI.Create();
            FireworkDebugUI.Create();
#endif
            new WinterProgress();

            Log = Logger;
            StageAPI.OnStagePreInitialization += StageAPI_OnStagePreInitialization;
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            BunchOfEmotesInstalled = Chainloader.PluginInfos.Keys.Contains("com.Dragsun.BunchOfEmotes");
            DynamicCameraInstalled = Chainloader.PluginInfos.Keys.Contains("DynamicCamera") || Chainloader.PluginInfos.Keys.Contains("com.Dragsun.Savestate");
        }

        private void StageAPI_OnStagePreInitialization(Stage newStage, Stage previousStage) {
            var winterManager = WinterManager.Create();
            winterManager.SetupStage(newStage);
        }

        private void Update() {
            UpdateEvent?.Invoke();
        }

        public delegate void UpdateDelegate();
        public static UpdateDelegate UpdateEvent;
    }
}
