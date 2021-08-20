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
                ToolHandlers toolHandlers,
                GroundGrid groundGrid
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
            this.groundGrid = groundGrid;
        }


        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly SlotsHolder slotsHolder;
        private readonly ObstacleBtnsPanel obstacleBtnPanel;
        private readonly ObstacleFacade.Factory obstacleFactory;
        private readonly UndoRedoSystem undoRedoSystem;
        private readonly BoxSelectionSystem boxSelectionSystem;
        private readonly ColorPicker colorPicker;
        private readonly ToolHandlers toolHandlers;
        private readonly GroundGrid groundGrid;

        private Vector3Int currentHitID;

        private Vector2 lastPos;
        private float threshold = 1f;
        private int lastY;

        private Vector3 handlerScale = Vector3.one;
        public Vector3 HandlerScale {
            get {
                return handlerScale;
            }
            set { 
                if(handlerScale == value)
                {
                    return;
                }
                if(value.x < 1 || value.y < 1 || value.z < 1)
                {
                    return;
                }
                if(value.x > groundGrid.xAmount || value.y > groundGrid.yAmount || value.z > groundGrid.zAmount)
                {
                    return;
                }

                handlerScale = value;
                onHandlerScaleChangeEvent.Invoke(value);
            } }

        public HandlerScaleChangeEvent onHandlerScaleChangeEvent = new HandlerScaleChangeEvent();


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
                lastY = slotRaycastSystem.PlaceableSlotID.y;

                //var selecteVector = toolHandlers.EndSlotID - toolHandlers.StartSlotID;

                if (CheckInSelectionRange(slotRaycastSystem.CurrentSoltID, boxSelectionSystem.EndSlotID, boxSelectionSystem.StartSlotID))
                {
                    GroupSpawnFromBoxSelection(obstacleBtnPanel.CurrentObstacleData);
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

                //Spawn(currentHitID, obstacleBtnPanel.CurrentObstacleData);


                GroupSpawnBrush(currentHitID, Vector3Int.one * 3, obstacleBtnPanel.CurrentObstacleData);

                lastY = currentHitID.y;
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
            var slot = slotsHolder.slotMap.Solts[slotID.x, slotID.y, slotID.z];
            if (slot.item == null)
            {
                var obstacle = obstacleFactory.Create(slotID, obstacleModel, Vector3.zero, colorPicker.CurrentColor);
                obstacle.transform.position = slot.position;
                slotsHolder.slotMap.SetSlotItem(slotID, obstacle.gameObject, obstacleModel);
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

        private void GroupSpawnBrush(Vector3Int centerID, Vector3Int size, ObstacleModel obstacleModel)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    for (int k = 0; k < size.z; k++)
                    {
                        Vector3Int targetSlotID = centerID + new Vector3Int(i, j, k);

                        var slot = slotsHolder.slotMap.Solts[targetSlotID.x, targetSlotID.y, targetSlotID.z];

                        var obstacle = obstacleFactory.Create(targetSlotID, obstacleModel, Vector3.zero, colorPicker.CurrentColor);
                        obstacle.transform.position = slot.position;
                        slotsHolder.slotMap.SetSlotItem(targetSlotID, obstacle.gameObject, obstacleModel);
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

        private Vector3Int Offset(Vector3Int slotID)
        {
            var halfXAmount = groundGrid.xAmount / 2;
            var halfZAmount = groundGrid.zAmount / 2;

            return new Vector3Int(
                Mathf.FloorToInt(slotID.x + halfXAmount / groundGrid.size), 
                0, 
                Mathf.FloorToInt(slotID.z + halfZAmount / groundGrid.size));
        }

    }
}