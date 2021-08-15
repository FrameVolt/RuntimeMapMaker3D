using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace RMM3D
{
    /// <summary>
    /// Storage all of obstacles data on the map to streamingAssets/map.txt
    /// </summary>
    public class SaveMapSystem
    {
        /// <summary>
        /// Inject dependence
        /// </summary>
        public SaveMapSystem(SlotsHolder slotsHolder)
        {
            this.slotsHolder = slotsHolder;
        }

        private SlotsHolder slotsHolder;

        private string filePath = Application.streamingAssetsPath + "/map.txt";
        /// <summary>
        /// Do save
        /// </summary>
        public void SaveMap()
        {
            string str = JsonConvert.SerializeObject(slotsHolder.slotMap.Solts);
            File.WriteAllText(filePath, str);

        }
        /// <summary>
        /// Load from streamingAssets
        /// </summary>
        public void LoadMap()
        {
            if (!File.Exists(filePath))
                return;
            slotsHolder.ResetSoltMap();
            string str = File.ReadAllText(filePath);

            var slotMap = JsonConvert.DeserializeObject<Solt[,,]>(str);
            slotsHolder.SetSoltMap(slotMap);
        }
        /// <summary>
        /// reset current map data
        /// </summary>
        public void ResetMap()
        {
            slotsHolder.ResetSoltMap();
        }
    }
}