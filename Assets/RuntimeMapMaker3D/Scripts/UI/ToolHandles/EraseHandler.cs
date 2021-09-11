// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
namespace RMM3D
{
    public class EraseHandler : MonoBehaviour, IInitializable, ITickable
    {
        [Inject]
        public void Construct(
         ToolHandlers toolHandlers,
         SlotRaycastSystem slotRaycastSystem,
         MoveToolSystem moveToolSystem)
        {
            this.toolHandlers = toolHandlers;
            this.slotRaycastSystem = slotRaycastSystem;
            this.moveToolSystem = moveToolSystem;
        }

        private ToolHandlers toolHandlers;
        private SlotRaycastSystem slotRaycastSystem;
        private MoveToolSystem moveToolSystem;

        [SerializeField] private Transform trans;
        [SerializeField] private Transform scaleTrans;

        public void Initialize()
        {
            toolHandlers.OnChangeToolType.AddListener(toolType =>
            {
                if (toolType == ToolType.Erase)
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

            if(toolHandlers.CurrentToolType == ToolType.Erase)
            {
                var pos = slotRaycastSystem.CurrentGroundSlotPos;
                trans.position = pos;
            }

        }
    }
}