using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace RMM3D.Editor
{
    /// <summary>
    /// Build obstacles and button sprites to assetbundle
    /// </summary>
    public class BuildAssetBundles
    {

        public static void BuildAllAssetBundles(string outputPath, params string[] assetFolders)
        {

            string outputFullPath = Application.streamingAssetsPath + "/" + outputPath;


            if (!Directory.Exists(outputFullPath))
            {
                Directory.CreateDirectory(outputFullPath);
            }
            List<string> filePathList = new List<string>();

            foreach (var assetFolder in assetFolders)
            {
                string[] filePaths = System.IO.Directory.GetFiles(assetFolder);
                filePathList.AddRange(filePaths);
            }
            var buildMap = new List<AssetBundleBuild> {
                new AssetBundleBuild {
                        assetBundleName = "obstacleBundle",
                        assetNames = filePathList.ToArray()
                }
            };

            BuildPipeline.BuildAssetBundles(outputFullPath, buildMap.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }

    }
}