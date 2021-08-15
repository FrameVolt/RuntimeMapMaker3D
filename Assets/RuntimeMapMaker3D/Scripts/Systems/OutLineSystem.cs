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
                renderers[i].gameObject.layer = LayerMask.NameToLayer("Outline");

                outLineRenderers.Add(renderers[i]);
            }
        }

        public void RemoveAllOutlines()
        {
            foreach (var item in outLineRenderers)
            {
                item.gameObject.layer = LayerMask.NameToLayer("Default");
                //item.material.renderQueue = 3000;
                //item.material.color = Color.white;
            }
            outLineRenderers.Clear();
        }

    }
}