// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
namespace RMM3D
{
    public class RotateHandler : MonoBehaviour, IInitializable, ITickable
    {
        [Inject]
        public void Construct(
               ToolHandlers toolHandlers,
               SlotRaycastSystem slotRaycastSystem,
               RotateObstacleSystem rotateObstacleSystem)
        {
            this.toolHandlers = toolHandlers;
            this.slotRaycastSystem = slotRaycastSystem;
            this.rotateObstacleSystem = rotateObstacleSystem;
        }

        private ToolHandlers toolHandlers;
        private SlotRaycastSystem slotRaycastSystem;
        private RotateObstacleSystem rotateObstacleSystem;

        [SerializeField] private Transform trans;
        [SerializeField] private Material rotateHandlerMat;

        public void Initialize()
        {
            toolHandlers.OnChangeToolType.AddListener(toolType => {
                if (toolType == ToolType.Rotate)
                {
                    trans.gameObject.SetActive(true);
                }
                else
                {
                    trans.gameObject.SetActive(false);
                }
            });
            rotateObstacleSystem.OnChangeCurrentAxis.AddListener(axis =>
            {
                switch (axis)
                {
                    case Axis.X:
                        trans.eulerAngles = new Vector3(60, 0, 90);
                        break;
                    case Axis.Y:
                        trans.eulerAngles = Vector3.zero;
                        break;
                    case Axis.Z:
                        trans.eulerAngles = new Vector3(90, 0, 0);
                        break;
                    default:
                        break;
                }
            });
        }

        public void Tick()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (toolHandlers.CurrentToolType == ToolType.Rotate)
            {
                RotateHandlerColor();
                Vector3 temp = slotRaycastSystem.CurrentGroundSlotPos;

                var pos = new Vector3(temp.x, temp.y, temp.z);
                trans.position = pos;
            }
        }

        private void RotateHandlerColor()
        {
            //if (slotRaycastSystem.CurrentObstacle != null)
            //{
            //    rotateHandlerMat.color = Color.white;
            //}
            //else
            //{
            //    rotateHandlerMat.color = new Color(1, 1, 1, 0.3f);
            //}
        }
    }
}