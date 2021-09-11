using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using RMM3D;
using System;
using System.Linq;
using System.Threading;

namespace RMM3D.Editor
{

    /// <summary>
    /// Main editor window of Runtime Map Maker 3D
    /// Responsible for convert your 3d Models to RMM3D prefabs and assetbundle
    /// and it while auto create sprite Preview from unity AssetImporter.
    /// </summary>
    public class ObstacleCreatorWindow : EditorWindow
    {

        static ObstacleCreatorData obstacleCreator;
        static SettingsInstaller settingsInstaller;
        static GroundGrid groundGrid;

        private int tab;

        [MenuItem("Tools/RuntimeMapMaker3D/Obstacle creator window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ObstacleCreatorWindow), false, "Obstacle creator window");
            {
                string[] guids = AssetDatabase.FindAssets("t:" + typeof(ObstacleCreatorData).Name);
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                obstacleCreator = AssetDatabase.LoadAssetAtPath<ObstacleCreatorData>(path);
            }
            {
                string[] guids = AssetDatabase.FindAssets("t:" + typeof(SettingsInstaller).Name);
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                settingsInstaller = AssetDatabase.LoadAssetAtPath<SettingsInstaller>(path);
            }
            {
                string[] guids = AssetDatabase.FindAssets("t:" + typeof(GroundGrid).Name);
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                groundGrid = AssetDatabase.LoadAssetAtPath<GroundGrid>(path);
            }

        }

        void OnDestroy()
        {
            EditorUtility.SetDirty(obstacleCreator);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void OnGUI()
        {
            GUIStyle myStyle = new GUIStyle(GUI.skin.label);
            myStyle.padding = new RectOffset(10, 10, 10, 10);

            EditorGUILayout.BeginVertical(myStyle);

            EditorGUILayout.Space(10);

            tab = GUILayout.Toolbar(tab, new string[] { "Folder mode", "Single mode", "Settings" });
            switch (tab)
            {
                case 0:
                    EditorGUILayout.Space(5);
                    GUILayout.Label("Source prefab or FBX files folder:", EditorStyles.boldLabel);
                    EditorGUILayout.Space(1);
                    obstacleCreator.sourcePrefabsPath = EditorGUILayout.TextField(obstacleCreator.sourcePrefabsPath);
                    EditorGUILayout.Space(10);
                    obstacleCreator.obstacleType = (ObstacleType)EditorGUILayout.EnumPopup("Obstacle type:", obstacleCreator.obstacleType);
                    EditorGUILayout.Space(10);
                    Rect rect1 = EditorGUILayout.GetControlRect(false, 1);
                    EditorGUI.DrawRect(rect1, new Color(0.5f, 0.5f, 0.5f, 1));
                    EditorGUILayout.Space(10);
                    ConvertAllPrefabsButton();
                    EditorGUILayout.Space(5);
                    CreateBundleButton();
                    break;
                case 1:
                    EditorGUILayout.Space(5);
                    GUILayout.Label("Source obstacle prefab or FBX file:", EditorStyles.boldLabel);
                    EditorGUILayout.Space(1);
                    obstacleCreator.fbx = (GameObject)EditorGUILayout.ObjectField(obstacleCreator.fbx, typeof(GameObject), true);
                    EditorGUILayout.Space(5);
                    obstacleCreator.obstacleType = (ObstacleType)EditorGUILayout.EnumPopup("Obstacle type:", obstacleCreator.obstacleType);
                    EditorGUILayout.Space(10);
                    Rect rect2 = EditorGUILayout.GetControlRect(false, 1);
                    EditorGUI.DrawRect(rect2, new Color(0.5f, 0.5f, 0.5f, 1));
                    EditorGUILayout.Space(10);
                    ConvertPrefabButton();
                    EditorGUILayout.Space(5);
                    CreateBundleButton();
                    break;
                case 2:
                    EditorGUILayout.Space(5);
                    GUILayout.Label("Source prefab or FBX files folder:", EditorStyles.boldLabel);
                    EditorGUILayout.Space(1);
                    obstacleCreator.sourcePrefabsPath = EditorGUILayout.TextField(obstacleCreator.sourcePrefabsPath);
                    EditorGUILayout.Space(5);
                    GUILayout.Label("Output prefabs folder:", EditorStyles.boldLabel);
                    EditorGUILayout.Space(1);
                    obstacleCreator.targetPrefabsPath = EditorGUILayout.TextField(obstacleCreator.targetPrefabsPath);
                    EditorGUILayout.Space(5);
                    GUILayout.Label("Button sprites folder:", EditorStyles.boldLabel);
                    EditorGUILayout.Space(1);
                    obstacleCreator.texturesPath = EditorGUILayout.TextField(obstacleCreator.texturesPath);
                    EditorGUILayout.Space(5);
                    GUILayout.Label("Output obstacle models folder:", EditorStyles.boldLabel);
                    EditorGUILayout.Space(1);
                    obstacleCreator.obstacleModelPath = EditorGUILayout.TextField(obstacleCreator.obstacleModelPath);
                    EditorGUILayout.Space(5);
                    GUILayout.Label("Output bundle folder:", EditorStyles.boldLabel);
                    EditorGUILayout.Space(1);
                    obstacleCreator.bundleOutputPath = EditorGUILayout.TextField("Assets/StreamingAssets/", obstacleCreator.bundleOutputPath);
                    EditorGUILayout.Space(5);
                    groundGrid.SetAmount(EditorGUILayout.Vector3IntField("Grid size:", groundGrid.GetAmount()));
                    EditorGUILayout.Space(10);
                    Rect rect3 = EditorGUILayout.GetControlRect(false, 1);
                    EditorGUI.DrawRect(rect3, new Color(0.5f, 0.5f, 0.5f, 1));

                    EditorGUILayout.Space(10);
                    var oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.7f, 0.7f, 0.341f);
                    if (GUILayout.Button("Reset settings", GUILayout.Height(25)))
                    {
                        obstacleCreator.sourcePrefabsPath = ObstacleCreatorData.defaultSourcePrefabsPath;
                        obstacleCreator.texturesPath = ObstacleCreatorData.defaultTexturesPath;
                        obstacleCreator.targetPrefabsPath = ObstacleCreatorData.defaultTargetPrefabsPath;
                        obstacleCreator.obstacleModelPath = ObstacleCreatorData.defaultObstacleModelPath;
                        obstacleCreator.bundleOutputPath = ObstacleCreatorData.defaultbundleOutputPath;

                        EditorUtility.SetDirty(obstacleCreator);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    GUI.backgroundColor = oldColor;
                    break;
                default:
                    break;
            }
            
            EditorGUILayout.EndVertical();

        }


        private void ConvertPrefabButton()
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.341f);
            if (GUILayout.Button("Convert this prefab", GUILayout.Height(25)))
            {
                EditorUtility.DisplayProgressBar("Creating files", "Creating target prefabs, obstacleModel and assetsbundle", 0.5f);
                CreateSingleMode();
                EditorUtility.ClearProgressBar();
            }
            GUI.backgroundColor = oldColor;
        }

        private void ConvertAllPrefabsButton()
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.341f);
            if (GUILayout.Button("Convert All prefabs", GUILayout.Height(25)))
            {
                EditorUtility.DisplayProgressBar("Creating files", "Creating target prefabs, obstacleModel and assetsbundle", 0.5f);
                CreateByFolder();
                EditorUtility.ClearProgressBar();
            }
            GUI.backgroundColor = oldColor;
        }

        private void CreateBundleButton()
        {
            EditorGUILayout.Space(5);
            GUILayout.Label("For load obstacles and button sprites at runtime:", EditorStyles.boldLabel);
            EditorGUILayout.Space(1);


            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.341f);

            if (GUILayout.Button("Build Asset bundle", GUILayout.Height(25)))
            {
                BuildAssetBundles.BuildAllAssetBundles(obstacleCreator.bundleOutputPath, obstacleCreator.targetPrefabsPath, obstacleCreator.texturesPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUI.backgroundColor = oldColor;
        }


        private void CreateSingleMode()
        {
            CreatePrefab(obstacleCreator.fbx, obstacleCreator.targetPrefabsPath);

            ObstacleModel obstacleModel = CreateObstacleModel(obstacleCreator.fbx.name, obstacleCreator.obstacleType);

            var list = settingsInstaller.gameSettings.obstacleDatas.ToList();
            list.Add(obstacleModel);
            settingsInstaller.gameSettings.obstacleDatas = list.ToArray();
            EditorUtility.SetDirty(settingsInstaller);
            EditorUtility.SetDirty(obstacleModel);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void CreateByFolder()
        {
            string sourcePrefabsPath = obstacleCreator.sourcePrefabsPath;


            var filePaths = System.IO.Directory.GetFiles(sourcePrefabsPath).Where(name => !name.EndsWith(".meta")).ToArray();

            for (int j = 0; j < filePaths.Length; j++)
            {

                CreatePrefabBySourcePath(filePaths[j], obstacleCreator.targetPrefabsPath);
            }


            List<GameObject> assets = new List<GameObject>();
            TryGetUnityObjectsOfTypeFromPath<GameObject>(sourcePrefabsPath, assets);

            for (int i = 0; i < assets.Count; i++)
            {
                CreateObstacleModel(assets[i].name, obstacleCreator.obstacleType);
            }

            List<ObstacleModel> obstacleModels = new List<ObstacleModel>();
            TryGetUnityObjectsOfTypeFromPath<ObstacleModel>(obstacleCreator.obstacleModelPath, obstacleModels);
            settingsInstaller.gameSettings.obstacleDatas = obstacleModels.ToArray();
            EditorUtility.SetDirty(settingsInstaller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        private ObstacleModel CreateObstacleModel(string name, ObstacleType obstacleType)
        {
            ObstacleModel asset = ScriptableObject.CreateInstance<ObstacleModel>();
            asset.assetName = name;
            asset.obstacleType = obstacleType;
            string assetPath = obstacleCreator.obstacleModelPath + name + ".asset";
            AssetDatabase.CreateAsset(asset, assetPath);
            EditorUtility.SetDirty(asset);
            return asset;
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

            CreateTextureFromPreview(go);


            var obstacleLayer = LayerMask.NameToLayer("Obstacle");

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


        private void CreateTextureFromPreview(UnityEngine.Object asset)
        {
            //TODO Check exist

            //var assetPath = AssetDatabase.GetAssetPath(asset);
            //var originalTexture = (Texture2D)AssetDatabase.GetCachedIcon(assetPath);
            ////var originalTexture = AssetPreview.GetAssetPreview(asset);

            //Texture2D copyTexture = new Texture2D(originalTexture.width, originalTexture.height);
            //copyTexture.SetPixels(originalTexture.GetPixels());
            //copyTexture.Apply();
            //byte[] _bytes = copyTexture.EncodeToPNG();
            //System.IO.File.WriteAllBytes(obstacleCreator.texturesPath + asset.name + ".png", _bytes);

            Texture2D bTexture = null;
            while (bTexture == null)
            {

                bTexture = AssetPreview.GetAssetPreview(asset);

                Thread.Sleep(80);
            }

            if (bTexture != null)
            {
                bTexture.mipMapBias = -1.5f;
                bTexture.Apply();
                byte[] data = bTexture.EncodeToPNG();
                string pathAndName = obstacleCreator.texturesPath + asset.name + ".png";
                File.WriteAllBytes(pathAndName, data);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Convert2Sprite(pathAndName);
            }
        }

        private void Convert2Sprite(string path)
        {
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite; 
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
            }
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
    }
}