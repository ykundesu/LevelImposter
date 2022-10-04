﻿using System;
using HarmonyLib;
using UnityEngine;
using LevelImposter.Shop;
using Reactor.Networking.MethodRpc;

namespace LevelImposter.Core
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CreatePlayer))]
    public static class SendRpcPatch
    {
        public static void Postfix()
        {
            MapUtils.SyncMapID();
        }
    }

    public static class ReactorRPC
    {
        public const int RPC_ID = 99; // Must be <= 99 for TOU-R Support

        public static Guid? ActiveDownloadingID = null;

        [MethodRpc(RPC_ID)]
        public static void RPCSendMapID(PlayerControl _p, string mapIDStr)
        {
            if (AmongUsClient.Instance.AmHost)
                return;
            LILogger.Info("[RPC] Received map ID [" + mapIDStr + "]");

            // Parse ID
            Guid mapID;
            if (!Guid.TryParse(mapIDStr, out mapID))
            {
                LILogger.Error("Invalid map ID");
            }

            // Get Current
            string currentMapID = MapLoader.CurrentMap == null ? "" : MapLoader.CurrentMap.id;
            if (ActiveDownloadingID != null)
            {
                LILogger.Notify("Download stopped.");
                ActiveDownloadingID = null;
            }

            // Handle ID
            if (mapID.Equals(Guid.Empty))
            {
                MapLoader.UnloadMap();
            }
            else if (currentMapID == mapIDStr || ActiveDownloadingID == mapID)
            {
                return;
            }
            else if (MapFileAPI.Instance.Exists(mapIDStr))
            {
                MapLoader.LoadMap(mapIDStr);
            }
            else
            {
                ActiveDownloadingID = mapID;
                LILogger.Notify("<color=#1a95d8>Downloading map, please wait...</color>");
                LevelImposterAPI.Instance.DownloadMap(mapID, ((LIMap map) =>
                {
                    if (ActiveDownloadingID == mapID)
                    {
                        MapLoader.LoadMap(map);
                        LILogger.Notify("<color=#1a95d8>Download finished!</color>");
                        ActiveDownloadingID = null;
                    }
                }));
            }
        }
    }
}