// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;

namespace RMM3D
{
    /// <summary>
    /// Data of ObstacleCreator window, save at ScriptableObject folder.
    /// </summary>
    [CreateAssetMenu(fileName = "ObstacleCreatorData", menuName = "RuntimeMapMaker3D/ObstacleCreatorData")]
    public class ObstacleCreatorData : ScriptableObject
    {
        public GameObject fbx;
        public ObstacleType obstacleType;

        public string sourcePrefabsPath;
        public string texturesPath;
        public string targetPrefabsPath;
        public string obstacleModelPath;
        public string bundleOutputPath;

        public const string defaultSourcePrefabsPath = "Assets/RuntimeMapMaker3D/ArtWorks/FBX/Obstacles/";
        public const string defaultTexturesPath = "Assets/RuntimeMapMaker3D/ArtWorks/Textures/ObstaclesIcon/";
        public const string defaultTargetPrefabsPath = "Assets/RuntimeMapMaker3D/Prefabs/Obstacles/";
        public const string defaultObstacleModelPath = "Assets/RuntimeMapMaker3D/Scriptables/Obstacles/";
        public const string defaultbundleOutputPath = "Obstacles/";

    }
}