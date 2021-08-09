using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UniRx;
using UniRx.Triggers;
using System;

namespace RMM3D
{
    public class PlacementSystem : ITickable
    {

        [Inject]
        public PlacementSystem(SlotRaycastSystem slotRaycastSystem,
                SlotsHolder groundSlotsHolder,
                ArrangeObstacleBtnsPanel arrangeObstacleBtnSystem,
                ToolGroupPanel toolGroupPanel,
                ObstacleFacade.Factory obstacleFactory,
                UndoRedoSystem undoRedoSystem,
                GroundGrid groundGrid,
                BoxSelectionSystem boxSelectionSystem,
                ToolHandlers groundSelectionHighlightSystem
                )
        {
            this.slotRaycastSystem = slotRaycastSystem;
            this.groundSlotsHolder = groundSlotsHolder;
            this.arrangeObstacleBtnSystem = arrangeObstacleBtnSystem;
            this.toolGroupPanel = toolGroupPanel;
            this.obstacleFactory = obstacleFactory;
            this.undoRedoSystem = undoRedoSystem;
            this.groundGrid = groundGrid;
            this.boxSelectionSystem = boxSelectionSystem;
            this.groundSelectionHighlightSystem = groundSelectionHighlightSystem;
        }


        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly SlotsHolder groundSlotsHolder;
        private readonly ArrangeObstacleBtnsPanel arrangeObstacleBtnSystem;
        private readonly ToolGroupPanel toolGroupPanel;
        private readonly ObstacleFacade.Factory obstacleFactory;
        private readonly UndoRedoSystem undoRedoSystem;
        private readonly GroundGrid groundGrid;
        private readonly BoxSelectionSystem boxSelectionSystem;
        private readonly ToolHandlers groundSelectionHighlightSystem;


        private Vector3Int currentHitID;

        private Vector2 lastPos;
        private float threshold = 1f;

        private int lastY;


        public void Tick()
        {
            if (toolGroupPanel.ToolTypeRP.Value != ToolType.BaseSelection)
                return;
            if (arrangeObstacleBtnSystem.CurrentObstacleData == null)
                return;
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                lastY = slotRaycastSystem.PlaceableSlotID.y;

                if (groundSelectionHighlightSystem.BoxSelectionTransSize.sqrMagnitude > 1)
                {
                    //var selecteVector = groundSelectionHighlightSystem.EndSlotID - groundSelectionHighlightSystem.StartSlotID;

                    GroupSpawn(arrangeObstacleBtnSystem.CurrentObstacleData);

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


                if (!slotRaycastSystem.IsPlaceableIDInRnage)
                    return;


                lastPos = Input.mousePosition;

                if (lastY != currentHitID.y)
                {
                    return;
                }

                Spawn(currentHitID, slotRaycastSystem.CurrentObstacle, arrangeObstacleBtnSystem.CurrentObstacleData);
                lastY = currentHitID.y;
            }
        }

        private void Spawn(Vector3Int slotID, ObstacleFacade obstacle, ObstacleModel obstacleModel)
        {
            var slot = groundSlotsHolder.slotMap.Solts[slotID.x, slotID.y, slotID.z];
            if (slot.item == null)
            {
                obstacle = obstacleFactory.Create(slotID, obstacleModel);
                obstacle.transform.position = slot.position;
                groundSlotsHolder.slotMap.SetSlotItem(slotID, obstacle.gameObject, obstacleModel);
            }
        }


        private void GroupSpawn(ObstacleModel obstacleModel)
        {
            for (int i = 0; i < boxSelectionSystem.coveringSlotIDs.Count; i++)
            {
                var slotID = boxSelectionSystem.coveringSlotIDs[i];

                if (slotID.y == slotRaycastSystem.GroundY.Value)
                {
                    var itemGO = groundSlotsHolder.slotMap.TryGetItem(slotID);
                    var slot = groundSlotsHolder.slotMap.Solts[slotID.x, slotID.y, slotID.z];

                    if (itemGO == null)
                    {
                        var obstacle = obstacleFactory.Create(slotID, obstacleModel);
                        obstacle.transform.position = slot.position;
                        groundSlotsHolder.slotMap.SetSlotItem(slotID, obstacle.gameObject, obstacleModel);
                    }
                }
            }
        }
    }
}