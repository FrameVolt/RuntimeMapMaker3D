using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RMM3D
{
    /// <summary>
    /// Load assetbundle at game start
    /// </summary>
    public class AssetBundleSystem
    {
        public AssetBundleSystem(ObstacleCreatorData obstacleCreatorData)
        {
            assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + obstacleCreatorData.bundleOutputPath + "obstaclebundle");

        }
        private ObstacleCreatorData obstacleCreatorData;
        public AssetBundle assetBundle { get; private set; }


    }
}