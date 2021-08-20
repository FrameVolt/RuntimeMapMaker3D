using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
namespace RMM3D
{
    public class MoveHandler : MonoBehaviour, IInitializable, ITickable
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

        public void Initialize()
        {
            toolHandlers.OnChangeCurrentToolType.AddListener(toolType => {
                if (toolType == ToolType.Move)
                {
                    trans.gameObject.SetActive(true);
                }
                else
                {
                    trans.gameObject.SetActive(false);
                }
            });
        }

        public void Tick()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (toolHandlers.CurrentToolType == ToolType.Move)
            {
                if (moveToolSystem.CurrentGrabedObstacle)
                {
                    trans.position = moveToolSystem.CurrentGrabedObstacle.transform.position;
                }
                else
                {
                    var pos = slotRaycastSystem.CurrentInRangeSlotPos;
                    trans.position = pos;
                }

            }
        }
    }
}