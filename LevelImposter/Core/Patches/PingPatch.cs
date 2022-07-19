using HarmonyLib;
using UnityEngine;
using LevelImposter.Core;
using LevelImposter.Shop;

namespace LevelImposter.Core
{
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingPatch
    {
        public static void Postfix(PingTracker __instance)
        {
            __instance.gameObject.SetActive(true);
            LIMap currentMap = MapLoader.currentMap;
            if (currentMap != null)
                __instance.text.text += "\n" + currentMap.name + " \n<size=2>by " + currentMap.authorName + "</size>";
        }
    }
}
