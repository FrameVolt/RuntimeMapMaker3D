// Copyright (c) 2021 LouYaoMing. All Right Reserved. 
// Licensed under the MIT License.

using UnityEditor;

namespace RMM3D.Editor
{
    public static class LayerUtility
{
    public static void CreateLayer(string layerName)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

        if ((asset != null) && (asset.Length > 0))
        {
            SerializedObject serializedObject = new SerializedObject(asset[0]);
            SerializedProperty layers = serializedObject.FindProperty("layers");

            for (int i = 0; i < layers.arraySize; ++i)
            {
                if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                {
                    return;     // Layer already present, nothing to do.
                }
            }

            //  layers.InsertArrayElementAtIndex(0);
            //  layers.GetArrayElementAtIndex(0).stringValue = layerName;

            for (int i = 0; i < layers.arraySize; i++)
            {
                if (layers.GetArrayElementAtIndex(i).stringValue == "")
                {
                    // layers.InsertArrayElementAtIndex(i);
                    layers.GetArrayElementAtIndex(i).stringValue = layerName;
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                    if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                    {
                        return;     // to avoid unity locked layer
                    }
                }
            }
        }

    }
}
}