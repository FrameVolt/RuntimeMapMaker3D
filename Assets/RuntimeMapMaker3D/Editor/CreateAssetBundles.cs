using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace RMM3D.Editor
{
    public class CreateAssetBundles
    {

        public static void BuildAllAssetBundles(string outputPath, params string[] assetFolders)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(outputPath);
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

            BuildPipeline.BuildAssetBundles(outputPath, buildMap.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }

    }
}