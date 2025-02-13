using Il2CppInterop.Runtime.Attributes;
using LevelImposter.Core;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LevelImposter.Shop
{
    /// <summary>
    /// API to manage cached map asset bundles in the local filesystem
    /// </summary>
    public static class MapFileCache
    {
        private static readonly JsonSerializerOptions SERIALIZE_OPTIONS = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Checks if a map exists in the local map cache
        /// </summary>
        /// <param name="mapID">ID of the map to check</param>
        /// <returns><c>true</c> if the map exists in the cache, <c>false</c> otherwise</returns>
        public static bool Exists(string mapID) => FileCache.Exists($"{mapID}.lim");

        /// <summary>
        /// Gets the path to a map file in the local map cache
        /// </summary>
        /// <param name="mapID">ID of the map to find</param>
        /// <returns>The path to the map file</returns>
        public static string GetPath(string mapID) => FileCache.GetPath($"{mapID}.lim");

        /// <summary>
        /// Reads and parses a map file into a LIMap.
        /// </summary>
        /// <param name="mapID">Map ID to load</param>
        /// <param name="callback">Callback on success</param>
        [HideFromIl2Cpp]
        public static LIMap? Get(string mapID)
        {
            if (!Exists(mapID))
            {
                LILogger.Warn($"Could not find map [{mapID}] in cache");
                return null;
            }

            LILogger.Info($"Loading map [{mapID}] from cache");

            string mapPath = GetPath(mapID);
            using (FileStream mapStream = File.OpenRead(mapPath))
            {
                LIMap? mapData = JsonSerializer.Deserialize<LIMap?>(mapStream);
                if (mapData != null)
                {
                    mapData.id = mapID;
                    return mapData;
                }
            }

            LILogger.Warn($"Failed to read map [{mapID}] from cache");
            return null;
        }

        /// <summary>
        /// Saves a map to the local map cache
        /// </summary>
        /// <param name="map">Map to save to cache</param>
        [HideFromIl2Cpp]
        public static void Save(LIMap map)
        {
            LILogger.Info($"Saving {map} to filesystem");
            string mapJson = JsonSerializer.Serialize(map, SERIALIZE_OPTIONS);
            FileCache.Save($"{map.id}.lim", mapJson);
        }
    }
}