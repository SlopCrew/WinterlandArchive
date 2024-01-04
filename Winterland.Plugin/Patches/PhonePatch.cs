using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile.Phone;
using Reptile;
using UnityEngine;
using Winterland.Common.Phone;

namespace Winterland.Plugin.Patches {
    [HarmonyPatch(typeof(Phone))]
    internal class PhonePatch {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Phone.PhoneInit))]
        private static void PhoneInit_Prefix(Phone __instance, Player setPlayer) {
            var appRoot = __instance.transform.Find("OpenCanvas/PhoneContainerOpen/MainScreen/Apps") as RectTransform;

            AppUtility.CreateApp<WinterlandApp>("WinterlandApp", appRoot!);
        }
    }
}
