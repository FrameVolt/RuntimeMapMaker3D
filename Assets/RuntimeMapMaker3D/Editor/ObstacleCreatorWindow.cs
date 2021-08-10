using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using RMM3D;
using System;

namespace RMM3D.Editor
{
    public class ObstacleCreatorWindow : EditorWindow
    {

        static ObstacleCreatorData obstacleCreator;
        static SettingsInstaller settingsInstaller;

        private int tab;

        [MenuItem("Tools/RuntimeMapMaker3D/Obstacle creator window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ObstacleCreatorWindow), false, "Obstacle creator window");
        }

        void OnDestroy()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void OnGUI()
        {
            if (obstacleCreator == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:" + typeof(ObstacleCreatorData).Name);
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                obstacleCreator = AssetDatabase.LoadAssetAtPath<ObstacleCreatorData>(path);
            }

            if (settingsInstaller == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:" + typeof(SettingsInstaller).Name);
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                settingsInstaller = AssetDatabase.LoadAssetAtPath<SettingsInstaller>(path);
            }

            GUIStyle myStyle = new GUIStyle(GUI.skin.label);
            myStyle.padding = new RectOffset(10, 10, 10, 10);

            EditorGUILayout.BeginVertical(myStyle);

            EditorGUILayout.Space(10);

            tab = GUILayout.Toolbar(tab, new string[] { "Folder mode", "Single mode" });
            switch (tab)
            {
                case 0:
                    EditorGUILayout.Space(5);
                    GUILayout.Label("Source prefab or FBX files folder:", EditorStyles.boldLabel);
                    EditorGUILayout.Space(1);
                    obstacleCreator.sourcePrefabsPath = EditorGUILayout.TextField(obstacleCreator.sourcePrefabsPath);
                    EditorGUILayout.Space(5);
                    GUILayout.Label("Icon sprites folder:", EditorStyles.boldLabel);
                    EditorGUILayout.Space(1);
                    obstacleCreator.texturesPath = EditorGUILayout.TextField(obstacleCreator.texturesPath);

                    break;
                case 1:
                    EditorGUILayout.Space(5);
                    GUILayout.Label("Source obstacle prefab or FBX file:", EditorStyles.boldLabel);
                    EditorGUILayout.Space(1);
                    obstacleCreator.fbx = (GameObject)EditorGUILayout.ObjectField(obstacleCreator.fbx, typeof(GameObject), true);
                    EditorGUILayout.Space(5);
                    obstacleCreator.obstacleIcon = GUIUtitlty.SpriteField("Icon sprite:", obstacleCreator.obstacleIcon);
                    break;

                default:
                    break;
            }

            EditorGUILayout.Space(5);
            GUILayout.Label("Output prefabs folder:", EditorStyles.boldLabel);
            EditorGUILayout.Space(1);
            obstacleCreator.targetPrefabsPath = EditorGUILayout.TextField(obstacleCreator.targetPrefabsPath);
            EditorGUILayout.Space(5);
            GUILayout.Label("Output obstacle models folder:", EditorStyles.boldLabel);
            EditorGUILayout.Space(1);
            obstacleCreator.obstacleModelPath = EditorGUILayout.TextField(obstacleCreator.obstacleModelPath);
            EditorGUILayout.Space(5);
            GUILayout.Label("Output bundle folder:", EditorStyles.boldLabel);
            EditorGUILayout.Space(1);
            obstacleCreator.bundleOutputPath = EditorGUILayout.TextField(obstacleCreator.bundleOutputPath);
            EditorGUILayout.Space(5);
            obstacleCreator.obstacleType = (ObstacleType)EditorGUILayout.EnumPopup("Obstacle type:", obstacleCreator.obstacleType);
            EditorGUILayout.Space(10);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Reset settings", GUILayout.Height(25)))
            {
                obstacleCreator.sourcePrefabsPath = ObstacleCreatorData.defaultSourcePrefabsPath;
                obstacleCreator.texturesPath = ObstacleCreatorData.defaultTexturesPath;
                obstacleCreator.targetPrefabsPath = ObstacleCreatorData.defaultTargetPrefabsPath;
                obstacleCreator.obstacleModelPath = ObstacleCreatorData.defaultObstacleModelPath;
                obstacleCreator.bundleOutputPath = ObstacleCreatorData.defaultbundleOutputPath;
            }
            EditorGUILayout.Space(2);

            var oldColor = GUI.backgroundColor;

            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.341f);

            if (GUILayout.Button("Convert", GUILayout.Height(25)))
            {
                switch (tab)
                {
                    case 0:
                        EditorUtility.DisplayProgressBar("Creating files", "Creating target prefabs, obstacleModel and assetsbundle", 0.5f);
                        CreateByFolder();
                        EditorUtility.ClearProgressBar();
                        break;
                    case 1:
                        CreatePrefab(obstacleCreator.fbx, obstacleCreator.targetPrefabsPath);
                        break;

                }
            }

            GUI.backgroundColor = oldColor;
            EditorGUILayout.EndVertical();
        }



        private void CreateByFolder()
        {

           

            string sourcePrefabsPath = obstacleCreator.sourcePrefabsPath;

            List<GameObject> assets = new List<GameObject>();
            List<GameObject> createdPrefabs = new List<GameObject>();
            TryGetUnityObjectsOfTypeFromPath<GameObject>(sourcePrefabsPath, assets);

            foreach (var item in assets)
            {
                //var t = System.IO.Path.GetFullPath(obstacleCreator.targetPrefabsPath);
                createdPrefabs.Add(CreatePrefab(item, obstacleCreator.targetPrefabsPath));
            }

            string texturesPath = obstacleCreator.texturesPath;

            List<Sprite> sprites = new List<Sprite>();
            TryGetUnityObjectsOfTypeFromPath<Sprite>(texturesPath, sprites);

            for (int i = 0; i < assets.Count; i++)
            {
                CreateObstacleModel(assets[i].name, createdPrefabs[i], sprites[i]);
            }

            List<ObstacleModel> obstacleModels = new List<ObstacleModel>();
            TryGetUnityObjectsOfTypeFromPath<ObstacleModel>(obstacleCreator.obstacleModelPath, obstacleModels);
            settingsInstaller.gameSettings.obstacleDatas = obstacleModels.ToArray();


            CreateAssetBundles.BuildAllAssetBundles(obstacleCreator.bundleOutputPath, obstacleCreator.targetPrefabsPath, obstacleCreator.texturesPath);


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        private void CreateObstacleModel(string name, GameObject targetPrefab, Sprite icon)
        {
            ObstacleModel asset = ScriptableObject.CreateInstance<ObstacleModel>();
            asset.assetName = name;
            asset.prefab = targetPrefab;
            asset.sprite = icon;
            string assetPath = obstacleCreator.obstacleModelPath + name + ".asset";
            AssetDatabase.CreateAsset(asset, assetPath);
        }


        public static int TryGetUnityObjectsOfTypeFromPath<T>(string path, List<T> assetsFound) where T : UnityEngine.Object
        {
            Debug.Log(path);
            string[] filePaths = System.IO.Directory.GetFiles(path);

            int countFound = 0;

            Debug.Log(filePaths.Length);

            if (filePaths != null && filePaths.Length > 0)
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    Debug.Log(filePaths[i]);
                    UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(T));
                    if (obj is T asset)
                    {
                        countFound++;
                        if (!assetsFound.Contains(asset))
                        {
                            assetsFound.Add(asset);
                        }
                    }
                }
            }

            return countFound;
        }
        private void CreatePrefabBySourcePath(string sourcePath, string outputPath)
        {

            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);

            CreatePrefab(go, outputPath);
        }

        private GameObject CreatePrefab(GameObject go, string outputPath)
        {
            var obstacleLayer = LayerMask.NameToLayer("Obstacle");
            if (obstacleLayer == -1)
            {
                LayerUtility.CreateLayer("Obstacle");
                obstacleLayer = LayerMask.NameToLayer("Obstacle");
            }

            GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(go);
            GameObject parentGO = new GameObject();
            parentGO.name = gameObject.name;
            gameObject.name = "Virtuals";
            gameObject.transform.SetParent(parentGO.transform);
            parentGO.AddComponent<BoxCollider>();
            parentGO.layer = obstacleLayer;
            parentGO.AddComponent<ObstacleFacade>();
            var binding = parentGO.AddComponent<Zenject.ZenjectBinding>();




            var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(parentGO, outputPath + parentGO.name + ".prefab", InteractionMode.UserAction);
            DestroyImmediate(parentGO);


            return prefab;

        }

    }
}