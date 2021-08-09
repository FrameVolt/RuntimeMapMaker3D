using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RMM3D
{
    public class SaveMapSystem
    {
        public SaveMapSystem(SlotsHolder groundSlotsHolder)
        {
            this.groundSlotsHolder = groundSlotsHolder;
        }

        private SlotsHolder groundSlotsHolder;

        public void SaveMap()
        {
            string str = JsonConvert.SerializeObject(groundSlotsHolder.slotMap.Solts);
            //ES3.Save("Solts", str);
        }

        public void LoadMap()
        {
            groundSlotsHolder.ResetSoltMap();

            //string str = ES3.Load<string>("Solts");

            //var slotMap = JsonConvert.DeserializeObject<Solt[,,]>(str);
            //groundSlotsHolder.SetSoltMap(slotMap);
        }

        public void ResetMap()
        {
            groundSlotsHolder.ResetSoltMap();
        }
    }
}