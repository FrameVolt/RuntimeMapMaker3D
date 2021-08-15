using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using System;
using HSVPicker;

namespace RMM3D
{
    /// <summary>
    /// Spawn tool for placement obstacle on map.
    /// </summary>
    public class PlacementSystem : ITickable
    {

        /// <summary>
        /// Inject dependence
        /// </summary>
        [Inject]
        public PlacementSystem(SlotRaycastSystem slotRaycastSystem,
                SlotsHolder slotsHolder,
                ObstacleBtnsPanel obstacleBtnSystem,
                ToolHandlers toolHandlers,
                ObstacleFacade.Factory obstacleFactory,
                UndoRedoSystem undoRedoSystem,
                GroundGrid groundGrid,
                BoxSelectionSystem boxSelectionSystem,
                ColorPicker colorPicker
                )
        {
            this.slotRaycastSystem = slotRaycastSystem;
            this.slotsHolder = slotsHolder;
            this.obstacleBtnPanel = obstacleBtnSystem;
            this.toolHandlers = toolHandlers;
            this.obstacleFactory = obstacleFactory;
            this.undoRedoSystem = undoRedoSystem;
            this.groundGrid = groundGrid;
            this.boxSelectionSystem = boxSelectionSystem;
            this.colorPicker = colorPicker;
        }


        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly SlotsHolder slotsHolder;
        private readonly ObstacleBtnsPanel obstacleBtnPanel;
        private readonly ToolHandlers toolHandlers;
        private readonly ObstacleFacade.Factory obstacleFactory;
        private readonly UndoRedoSystem undoRedoSystem;
        private readonly GroundGrid groundGrid;
        private readonly BoxSelectionSystem boxSelectionSystem;
        private readonly ColorPicker colorPicker;

        private Vector3Int currentHitID;

        private Vector2 lastPos;
        private float threshold = 1f;

        private int lastY;


        /// <summary>
        /// Tick is Same as Unity Update event, control by Zenject
        /// </summary>
        public void Tick()
        {
            if (toolHandlers.CurrentToolType != ToolType.BaseSelection)
                return;
            if (obstacleBtnPanel.CurrentObstacleData == null)
                return;
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (!slotRaycastSystem.IsPlaceableIDInRnage)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                lastY = slotRaycastSystem.PlaceableSlotID.y;

                if (toolHandlers.BoxSelectionTransSize.sqrMagnitude > 1)
                {
                    //var selecteVector = toolHandlers.EndSlotID - toolHandlers.StartSlotID;

                    if (CheckInSelectionRange(slotRaycastSystem.CurrentSoltID, toolHandlers.EndSlotID, toolHandlers.StartSlotID))
                    {
                        GroupSpawn(obstacleBtnPanel.CurrentObstacleData);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                currentHitID = Vector3Int.zero;

                undoRedoSystem.AppendStatus();

            }
            if (Input.GetMouseButton(0))
            {
                if (Vector3.Distance(lastPos, Input.mousePosition) < threshold)
                    return;

                if (slotRaycastSystem.PlaceableSlotID == currentHitID)
                    return;

                currentHitID = slotRaycastSystem.PlaceableSlotID;


                lastPos = Input.mousePosition;

                if (lastY != currentHitID.y)
                {
                    return;
                }

                Spawn(currentHitID, slotRaycastSystem.CurrentObstacle, obstacleBtnPanel.CurrentObstacleData);
                lastY = currentHitID.y;
            }
        }

        /// <summary>
        /// Spawn one obstacle 
        /// </summary>
        /// <param name="slotID"></param>
        /// <param name="obstacle"></param>
        /// <param name="obstacleModel"></param>
        private void Spawn(Vector3Int slotID, ObstacleFacade obstacle, ObstacleModel obstacleModel)
        {
            var slot = slotsHolder.slotMap.Solts[slotID.x, slotID.y, slotID.z];
            if (slot.item == null)
            {
                obstacle = obstacleFactory.Create(slotID, obstacleModel, Vector3.zero, colorPicker.CurrentColor);
                obstacle.transform.position = slot.position;
                slotsHolder.slotMap.SetSlotItem(slotID, obstacle.gameObject, obstacleModel);
            }
        }

        /// <summary>
        /// Spawn obstacles in selection range.
        /// </summary>
        /// <param name="obstacleModel"></param>
        private void GroupSpawn(ObstacleModel obstacleModel)
        {
            for (int i = 0; i < boxSelectionSystem.coveringSlotIDs.Count; i++)
            {
                var slotID = boxSelectionSystem.coveringSlotIDs[i];

                if (slotID.y == slotRaycastSystem.GroundY)
                {
                    var itemGO = slotsHolder.slotMap.TryGetItem(slotID);
                    var slot = slotsHolder.slotMap.Solts[slotID.x, slotID.y, slotID.z];

                    if (itemGO == null)
                    {
                        var obstacle = obstacleFactory.Create(slotID, obstacleModel, Vector3.zero, colorPicker.CurrentColor);
                        obstacle.transform.position = slot.position;
                        slotsHolder.slotMap.SetSlotItem(slotID, obstacle.gameObject, obstacleModel);
                    }
                }
            }
        }

        public bool CheckInSelectionRange(Vector3Int slotID, Vector3Int min, Vector3Int max)
        {
            bool inRange = true;
            if (!IsBetween(slotID.x, min.x, max.x) || !IsBetween(slotID.y, min.y, max.y) || !IsBetween(slotID.z, min.z, max.z))
                inRange = false;

            return inRange;
        }
        public bool IsBetween(float inputValue, float bound1, float bound2)
        {
            return (inputValue >= Mathf.Min(bound1, bound2) && inputValue <= Mathf.Max(bound1, bound2));
        }
    }
}