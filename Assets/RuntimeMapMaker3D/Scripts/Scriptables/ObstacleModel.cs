// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;

namespace RMM3D
{
    /// <summary>
    /// Each obstacle data model
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "ObstacleModel", menuName = "Game/ObstacleModel")]
    public class ObstacleModel : ScriptableObject
    {
        public ObstacleType obstacleType;
        public string assetName;
        public int slotSize;
    }

    public enum ObstacleType
    {
        Obstacle,
        Harmful,
        Prop
    }
}