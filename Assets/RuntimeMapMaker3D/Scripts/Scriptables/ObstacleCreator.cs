using UnityEditor;
using UnityEngine;

namespace RMM3D
{
    [CreateAssetMenu(fileName = "ObstacleCreator", menuName = "RuntimeMapMaker3D/ObstacleCreator")]
    public class ObstacleCreator : ScriptableObject
    {
        public GameObject fbx;
        public Sprite obstacleIcon;

        public string sourcePrefabsPath;
        public string texturesPath;
        public string targetPrefabsPath;
        public string obstacleModelPath;

        public const string defaultSourcePrefabsPath = "Assets/RuntimeMapMaker3D/ArtWorks/FBX/Obstacles/";
        public const string defaultTexturesPath = "Assets/RuntimeMapMaker3D/ArtWorks/Textures/ObstaclesIcon/";
        public const string defaultTargetPrefabsPath = "Assets/RuntimeMapMaker3D/Prefabs/Obstacles/";
        public const string defaultObstacleModelPath = "Assets/RuntimeMapMaker3D/Scriptables/Obstacles/";

    }
}