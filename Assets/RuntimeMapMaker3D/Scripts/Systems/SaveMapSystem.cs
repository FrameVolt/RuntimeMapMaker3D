using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace RMM3D
{
    public class SaveMapSystem
    {
        public SaveMapSystem(SlotsHolder groundSlotsHolder)
        {
            this.groundSlotsHolder = groundSlotsHolder;
        }

        private SlotsHolder groundSlotsHolder;

        private string filePath = Application.dataPath + "map.txt";

        public void SaveMap()
        {
            string str = JsonConvert.SerializeObject(groundSlotsHolder.slotMap.Solts);
            File.WriteAllText(str, filePath);
        }

        public void LoadMap()
        {
            groundSlotsHolder.ResetSoltMap();
            string str = File.ReadAllText(filePath);
            var slotMap = JsonConvert.DeserializeObject<Solt[,,]>(str);
            groundSlotsHolder.SetSoltMap(slotMap);
        }

        public void ResetMap()
        {
            groundSlotsHolder.ResetSoltMap();
        }
    }
}