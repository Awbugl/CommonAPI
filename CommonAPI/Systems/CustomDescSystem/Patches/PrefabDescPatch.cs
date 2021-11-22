﻿using System.Collections.Generic;
using CommonAPI.Systems;
using HarmonyLib;

namespace CommonAPI.Patches
{
    [HarmonyPatch]
    public static class PrefabDescPatch
    {
        [HarmonyPatch(typeof(PrefabDesc), "ReadPrefab")]
        [HarmonyPostfix]
        public static void Postfix(PrefabDesc __instance)
        {
            if (__instance.prefab != null)
            {
                __instance.customData = new Dictionary<string, object>();
                CustomDesc[] descs = __instance.prefab.GetComponentsInChildren<CustomDesc>();
                foreach (CustomDesc desc in descs)
                {
                    desc.ApplyProperties(__instance);
                }
            }
        }


        [HarmonyPatch(typeof(PrefabDesc), "Free")]
        [HarmonyPostfix]
        public static void Free(PrefabDesc __instance)
        {
            __instance.customData = null;
        }
    }
}