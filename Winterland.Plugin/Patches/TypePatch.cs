using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace Winterland.Plugin.Patches {
    [HarmonyPatch(typeof(Type))]
    internal class TypePatch {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Type.GetType), typeof(string))]
        private static void GetType_Postfix(string typeName, ref Type __result) {
            if (typeName == $"Reptile.Phone.WinterlandApp")
                __result = typeof(Common.Phone.WinterlandApp);
        }
    }
}
