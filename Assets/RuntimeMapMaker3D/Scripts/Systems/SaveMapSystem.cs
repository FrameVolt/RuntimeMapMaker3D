// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using Newtonsoft.Json;
using System;
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

        public event Action OnReset = () => { };
        public event Action OnLoad = () => { };


        private SlotsHolder slotsHolder;

        private string filePath = Application.streamingAssetsPath + "/map.txt";
        /// <summary>
        /// Do save
        /// </summary>
        public void SaveMap()
        {
            string str = JsonConvert.SerializeObject(slotsHolder.Solts);
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
            OnLoad.Invoke();
        }
        /// <summary>
        /// reset current map data
        /// </summary>
        public void ResetMap()
        {
            slotsHolder.ResetSoltMap();
            OnReset.Invoke();
        }
    }
}