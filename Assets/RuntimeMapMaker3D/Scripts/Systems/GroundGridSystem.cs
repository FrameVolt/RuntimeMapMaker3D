// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using Zenject;

namespace RMM3D
{
    public class GroundGridSystem : MonoBehaviour, IInitializable
    {
        [Inject]
        public void Construct(GroundGrid groundGrid, Settings settings, SlotRaycastSystem slotRaycastSystem)
        {
            this.groundGrid = groundGrid;
            this.settings = settings;
            this.slotRaycastSystem = slotRaycastSystem;
        }

        public Transform gridTrans;
        private GroundGrid groundGrid;
        private Settings settings;
        private SlotRaycastSystem slotRaycastSystem;

        public void Initialize()
        {
            settings.material.mainTextureScale = new Vector2(groundGrid.xAmount, groundGrid.zAmount);
            gridTrans.localScale = new Vector3(groundGrid.xAmount * 0.1f, 1, groundGrid.zAmount * 0.1f);

            slotRaycastSystem.OnChangeGroundY.AddListener((v) =>
            {
                gridTrans.localPosition = new Vector3(0, -0.48f + v, 0);
            });

        }
        [System.Serializable]
        public class Settings
        {
            public Material material;
        }

    }
}