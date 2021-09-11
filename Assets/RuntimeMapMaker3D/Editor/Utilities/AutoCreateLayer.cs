using UnityEditor;

namespace RMM3D
{
    [InitializeOnLoad]
    public class AutoCreateLayer : UnityEditor.Editor
    {

        static AutoCreateLayer()
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            int count = 0;

            SetLayer("Outline", ref count, layers, tagManager);
            SetLayer("Obstacle", ref count, layers, tagManager);
            SetLayer("Handler", ref count, layers, tagManager);
            SetLayer("Ground", ref count, layers, tagManager);

            tagManager.ApplyModifiedProperties();
            tagManager.Update();
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

}