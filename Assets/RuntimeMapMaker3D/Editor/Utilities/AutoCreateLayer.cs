using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[InitializeOnLoad]
public class AutoCreateLayer : Editor {

    static AutoCreateLayer()
    {
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        int count = 0;
        SetLayer("Ground", ref count, layers, tagManager);
        SetLayer("Outline", ref count, layers, tagManager);
        SetLayer("Obstacle", ref count, layers, tagManager);
        SetLayer("Handler", ref count, layers, tagManager);
    }

    private static void SetLayer(string layName, ref int count, SerializedProperty layers, SerializedObject tagManager)
    {
        
        while (layers.NextVisible(true))
        {
            if (layers.name == "data")
            {

                if (layers.stringValue == layName)
                    return;

                if (layers.stringValue == "" && count > 8)
                {
                    //Debug.Log(layers.stringValue);
                    layers.stringValue = layName;
                    tagManager.ApplyModifiedProperties();

                    return;
                }
            }
            count++;
        }
    }
}

