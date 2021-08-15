using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RMM3D
{
    [CreateAssetMenu(fileName = "GroundGridData", menuName = "Game/GroundGridData")]
    public class GroundGrid : ScriptableObject
    {
        public int xAmount;
        public int yAmount;
        public int zAmount;
        
        public float size;

        public void SetAmount(Vector3Int vector)
        {
            xAmount = vector.x;
            yAmount = vector.y;
            zAmount = vector.z;
        }

        public Vector3Int GetAmount()
        {
            return new Vector3Int(xAmount, yAmount, zAmount);
        }
    }
}