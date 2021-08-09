using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RMM3D
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ObstacleModel", menuName = "Game/ObstacleModel")]
    public class ObstacleModel : ScriptableObject
    {
        public ObstacleType obstacleType;
        public GameObject prefab;
        public Sprite sprite;
        public int slotSize;
    }

    public enum ObstacleType
    {
        Obstacle,
        Harmful,
        Prop
    }
}