﻿using LevelImposter.Core;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace LevelImposter.DB
{
    /// <summary>
    /// Database of PlayerTask objects
    /// </summary>
    public class TaskDB : SubDB<PlayerTask>
    {
        public TaskDB(SerializedAssetDB serializedDB) : base(serializedDB) { }

        public override void LoadShip(ShipStatus shipStatus, MapType mapType)
        {
            DB.TaskDB.ForEach((elem) =>
            {
                if (elem.MapType != mapType)
                    return;

                // Task Type
                var taskArr = elem.TaskType switch
                {
                    TaskLength.Common => shipStatus.CommonTasks.Cast<Il2CppReferenceArray<PlayerTask>>(),
                    TaskLength.Long => shipStatus.LongTasks.Cast<Il2CppReferenceArray<PlayerTask>>(),
                    TaskLength.Short => shipStatus.NormalTasks.Cast<Il2CppReferenceArray<PlayerTask>>(),
                    _ => shipStatus.SpecialTasks
                };

                // Task
                var task = taskArr.FirstOrDefault((e) =>
                {
                    return e.name == elem.Name;
                });
                if (task == null)
                {
                    LILogger.Warn($"TaskDB could not find {elem.ID} in {shipStatus.name}");
                    return;
                }

                Add(elem.ID, task);
            });
        }

        [Serializable]
        public class DBElement
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public int Map { get; set; }
            public int Type { get; set; }

            [JsonIgnore]
            public MapType MapType => (MapType)Map;
            [JsonIgnore]
            public TaskLength TaskType => (TaskLength)Type;
        }
    }
}
