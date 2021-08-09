using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RMM3D
{
    public class OutLineSystem : MonoBehaviour
    {

        private List<Renderer> outLineRenderers = new List<Renderer>();
        [ContextMenu("SetOutlines")]
        public void SetOutlines(GameObject targetGO)
        {

            var renderers = targetGO.GetComponentsInChildren<Renderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = new Color(0.3f, 0.3f, 0.9f);

                outLineRenderers.Add(renderers[i]);
            }
        }

        public void RemoveAllOutlines()
        {
            foreach (var item in outLineRenderers)
            {
                item.material.color = Color.white;
            }
            outLineRenderers.Clear();
        }

    }
}