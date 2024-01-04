// https://github.com/SlopCrew/SlopCrew/blob/f356897d0e480673fd6a0715c6f3a6bec6338255/SlopCrew.Plugin/Patches/AppHomeScreenPatch.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile.Phone;
using HarmonyLib;
using Reptile;
using UnityEngine;
using Winterland.Common.Phone;
using Winterland.Common;

namespace Winterland.Plugin.Patches {
    [HarmonyPatch(typeof(AppHomeScreen))]
    internal class AppHomeScreenPatch {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(AppHomeScreen.Awake))]
        private static void Awake_Postfix(AppHomeScreen __instance) {
            var apps = __instance.availableHomeScreenApps;
            Array.Resize(ref apps, apps.Length + 1);
            var winterlandApp = ScriptableObject.CreateInstance<HomeScreenApp>();
            winterlandApp.m_AppName = "WinterlandApp";
            winterlandApp.m_DisplayName = "winterland";
            winterlandApp.m_AppIcon = WinterAssets.Instance.PhoneResources.AppIcon;
            winterlandApp.appType = HomeScreenApp.HomeScreenAppType.NONE;
            apps[apps.Length - 1] = winterlandApp;
            __instance.availableHomeScreenApps = apps;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(AppHomeScreen.OnAppEnable))]
        private static void OnAppEnable_Postfix(AppHomeScreen __instance) {
            var winterlandApp = __instance.availableHomeScreenApps.First(x => x.m_AppName == "WinterlandApp");

            if (winterlandApp == null) return;

            if (Core.Instance.BaseModule.CurrentStage == Stage.square)
                __instance.AddApp(winterlandApp);
            else
                __instance.RemoveApp(winterlandApp);
        }
    }
}
