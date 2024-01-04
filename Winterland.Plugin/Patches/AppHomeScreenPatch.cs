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

namespace Winterland.Plugin.Patches {
    [HarmonyPatch(typeof(AppHomeScreen))]
    internal class AppHomeScreenPatch {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AppHomeScreen.Awake))]
        private static void Awake_Prefix(AppHomeScreen __instance) {
            var apps = __instance.m_Apps;
            Array.Resize(ref apps, apps.Length + 1);
            var winterlandApp = ScriptableObject.CreateInstance<HomeScreenApp>();
            winterlandApp.m_AppName = "WinterlandApp";
            winterlandApp.m_DisplayName = "winterland";
            winterlandApp.appType = HomeScreenApp.HomeScreenAppType.NONE;
            apps[apps.Length - 1] = winterlandApp;
            __instance.m_Apps = apps;
        }
    }
}
