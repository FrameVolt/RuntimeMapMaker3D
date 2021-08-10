using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RMM3D
{
    public class AssetBundleSystem
    {
        public AssetBundle assetBundle { get; private set; }

        public AssetBundleSystem(ObstacleCreatorData obstacleCreatorData)
        {
            assetBundle = AssetBundle.LoadFromFile(obstacleCreatorData.bundleOutputPath + "obstaclebundle");
        }

    }
}