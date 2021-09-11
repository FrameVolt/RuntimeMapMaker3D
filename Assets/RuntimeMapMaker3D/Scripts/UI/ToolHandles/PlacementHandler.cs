// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
namespace RMM3D
{
    public class PlacementHandler : MonoBehaviour, IInitializable, ITickable
    {
        [Inject]
        public void Construct(
            ToolHandlers toolHandlers,
            SlotRaycastSystem slotRaycastSystem,
            PlacementSystem placementSystem
            )
        {
            this.toolHandlers = toolHandlers;
            this.slotRaycastSystem = slotRaycastSystem;
            this.placementSystem = placementSystem;
        }


        private ToolHandlers toolHandlers;
        private SlotRaycastSystem slotRaycastSystem;
        private PlacementSystem placementSystem;

        [SerializeField] private Transform trans;
        [SerializeField] private Transform scaleTrans;


        public void Initialize()
        {
            toolHandlers.OnChangeToolType.AddListener(toolType =>
            {
                if (toolType == ToolType.Placement)
                {
                    trans.gameObject.SetActive(true);
                }
                else
                {
                    trans.gameObject.SetActive(false);
                }
            });
            toolHandlers.onHandlerScaleChangeEvent.AddListener(v =>
            {
                var oddScale = new Vector3(v.x * 2 - 1, v.y, v.z * 2 - 1);
                scaleTrans.localScale = oddScale;
            });
        }


        public void Tick()
        {

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (toolHandlers.CurrentToolType == ToolType.Placement)
            {
                Vector3 temp;
                //if (Input.GetMouseButton(0))
                //{
                    temp = slotRaycastSystem.CurrentGroundSlotPos;
                //}
                //else
                //{
                //    temp = slotRaycastSystem.PlaceableSlotPos;
                //}

                var pos = new Vector3(temp.x, temp.y, temp.z);
                trans.position = pos;
            }
        }
    }
}