// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace RMM3D
{
    /// <summary>
    /// Rotate obstacle system
    /// </summary>
    public class RotateObstacleSystem : ITickable
    {
        /// <summary>
        /// Inject dependence
        /// </summary>
        [Inject]
        public RotateObstacleSystem(
            SlotRaycastSystem slotRaycastSystem,
            SlotsHolder slotsHolder,
            ObstacleBtnsPanel obstacleBtnPanel,
            ToolHandlers toolHandlers,
            UndoRedoSystem undoRedoSystem)
        {
            this.slotRaycastSystem = slotRaycastSystem;
            this.slotsHolder = slotsHolder;
            this.obstacleBtnPanel = obstacleBtnPanel;
            this.toolHandlers = toolHandlers;
            this.undoRedoSystem = undoRedoSystem;
        }

        private SlotRaycastSystem slotRaycastSystem;
        private SlotsHolder slotsHolder;
        private ObstacleBtnsPanel obstacleBtnPanel;
        private ToolHandlers toolHandlers;
        private UndoRedoSystem undoRedoSystem;

        private Axis axis;
        public Axis CurrentAxis
        {
            get
            {
                return axis;
            }
            set
            {
                if (axis == value)
                    return;

                axis = value;
            }
        }

        public ChangeAxisEvent OnChangeCurrentAxis = new ChangeAxisEvent();
        /// <summary>
        /// Tick is Same as Unity Update event, control by Zenject
        /// </summary>
        public void Tick()
        {
            if (toolHandlers.CurrentToolType != ToolType.Rotate)
                return;
            if (obstacleBtnPanel.CurrentObstacleData == null)
                return;
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            if (!slotRaycastSystem.IsPlaceableIDInRnage)
                return;


            if (Input.GetMouseButtonDown(0))
            {
                var currentHitID = slotRaycastSystem.CurrentGroundSlotID;

                var slot = slotsHolder.Slots[currentHitID.x, currentHitID.y, currentHitID.z];

                if (slot.item != null)
                {
                    var trans = slot.item.transform;

                    switch (CurrentAxis)
                    {
                        case Axis.X:
                            trans.Rotate(new Vector3(-90, 0, 0), Space.World);
                            break;
                        case Axis.Y:
                            trans.Rotate(new Vector3(0, 90, 0), Space.World);
                            break;
                        case Axis.Z:
                            trans.Rotate(new Vector3(0, 0, 90), Space.World);
                            break;
                        default:
                            break;
                    }

                    slot.rotation = trans.eulerAngles;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                undoRedoSystem.AppendStatus();
            }
        }
        /// <summary>
        /// Change rotate axis
        /// </summary>
        public void ChangeAxis()
        {
            var array = Enum.GetValues(typeof(Axis));
            var length = array.Length;
            int index = (int)CurrentAxis;
            index++;
            if (index >= length)
                index = 0;
            CurrentAxis = (Axis)index;
        }
    }
}