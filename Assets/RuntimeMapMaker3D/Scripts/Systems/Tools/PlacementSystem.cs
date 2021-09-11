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
                ObstacleFacade.Factory obstacleFactory,
                UndoRedoSystem undoRedoSystem,
                BoxSelectionSystem boxSelectionSystem,
                ColorPicker colorPicker,
                ToolHandlers toolHandlers
                )
        {
            this.slotRaycastSystem = slotRaycastSystem;
            this.slotsHolder = slotsHolder;
            this.obstacleBtnPanel = obstacleBtnSystem;
            this.obstacleFactory = obstacleFactory;
            this.undoRedoSystem = undoRedoSystem;
            this.boxSelectionSystem = boxSelectionSystem;
            this.colorPicker = colorPicker;
            this.toolHandlers = toolHandlers;
        }


        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly SlotsHolder slotsHolder;
        private readonly ObstacleBtnsPanel obstacleBtnPanel;
        private readonly ObstacleFacade.Factory obstacleFactory;
        private readonly UndoRedoSystem undoRedoSystem;
        private readonly BoxSelectionSystem boxSelectionSystem;
        private readonly ColorPicker colorPicker;
        private readonly ToolHandlers toolHandlers;

        private Vector3Int currentGroundSlotID;

        private Vector2 lastPos;
        private float threshold = 1f;


        /// <summary>
        /// Tick is Same as Unity Update event, control by Zenject
        /// </summary>
        public void Tick()
        {
            if (toolHandlers.CurrentToolType != ToolType.Placement)
                return;
            if (obstacleBtnPanel.CurrentObstacleData == null)
                return;
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (!slotRaycastSystem.IsPlaceableIDInRnage)
                return;

            if (Input.GetMouseButtonDown(0))
            {

            }
            else if (Input.GetMouseButtonUp(0))
            {
                currentGroundSlotID = Vector3Int.zero;

                undoRedoSystem.AppendStatus();

            }
            else if (Input.GetMouseButton(0))
            {
                if (Vector3.Distance(lastPos, Input.mousePosition) < threshold)
                    return;
                lastPos = Input.mousePosition;

                if (slotRaycastSystem.CurrentGroundSlotID == currentGroundSlotID)
                    return;

                
                currentGroundSlotID = slotRaycastSystem.CurrentGroundSlotID;


                GroupSpawnBrush(currentGroundSlotID, toolHandlers.BrushOddScaleInt, obstacleBtnPanel.CurrentObstacleData);

            }
        }

        /// <summary>
        /// Spawn one obstacle 
        /// </summary>
        /// <param name="slotID"></param>
        /// <param name="obstacle"></param>
        /// <param name="obstacleModel"></param>
        private void Spawn(Vector3Int slotID, ObstacleModel obstacleModel)
        {
            var slot = slotsHolder.Solts[slotID.x, slotID.y, slotID.z];
            if (slot.item == null)
            {
                var obstacle = obstacleFactory.Create(slotID, obstacleModel, Vector3.zero, Vector3.one, colorPicker.CurrentColor);
                obstacle.transform.position = slot.position;
                slotsHolder.SetSlotItem(slotID, obstacle, obstacleModel);
            }
        }

        /// <summary>
        /// Spawn obstacles in selection range.
        /// </summary>
        /// <param name="obstacleModel"></param>
        private void GroupSpawnFromBoxSelection(ObstacleModel obstacleModel)
        {
            for (int i = 0; i < boxSelectionSystem.coveringSlotIDs.Count; i++)
            {
                var slotID = boxSelectionSystem.coveringSlotIDs[i];

                if (slotID.y == slotRaycastSystem.GroundY)
                {
                    var itemGO = slotsHolder.TryGetItem(slotID);
                    var slot = slotsHolder.Solts[slotID.x, slotID.y, slotID.z];

                    if (itemGO == null)
                    {
                        var obstacle = obstacleFactory.Create(slotID, obstacleModel, Vector3.zero, Vector3.one, colorPicker.CurrentColor);
                        obstacle.transform.position = slot.position;
                        slotsHolder.SetSlotItem(slotID, obstacle, obstacleModel);
                    }
                }
            }
        }

        private void GroupSpawnBrush(Vector3Int centerID, Vector3Int size, ObstacleModel obstacleModel)
        {
            toolHandlers.CheckSlotsInBrush(centerID, size);
            List<Vector3Int> slotsInBrush = toolHandlers.SlotsInBrush;
            foreach (var targetSlotID in slotsInBrush)
            {
                if (!slotRaycastSystem.CheckInIDRange(targetSlotID))
                    continue;

                var itemGO = slotsHolder.TryGetItem(targetSlotID);
                if (itemGO == null)
                {
                    var slot = slotsHolder.Solts[targetSlotID.x, targetSlotID.y, targetSlotID.z];

                    var obstacle = obstacleFactory.Create(targetSlotID, obstacleModel, Vector3.zero, Vector3.one, colorPicker.CurrentColor);
                    obstacle.transform.position = slot.position;
                    slotsHolder.SetSlotItem(targetSlotID, obstacle, obstacleModel);
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