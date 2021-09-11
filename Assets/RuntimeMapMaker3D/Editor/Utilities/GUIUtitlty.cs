// Copyright (c) 2021 LouYaoMing. All Right Reserved. 
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace RMM3D.Editor
{
    public static class GUIUtitlty
    {
        public static Sprite SpriteField(string name, Sprite sprite)
        {
            GUILayout.BeginVertical();
            var style = new GUIStyle(EditorStyles.boldLabel);
            style.alignment = TextAnchor.UpperCenter;
            style.fixedWidth = 70;
            GUILayout.Label(name, style);
            var result = (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
            GUILayout.EndVertical();
            return result;
        }
    }
}