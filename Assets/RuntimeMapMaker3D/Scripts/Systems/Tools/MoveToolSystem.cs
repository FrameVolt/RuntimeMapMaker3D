using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UnityEngine.Assertions;

namespace RMM3D
{
    public class MoveToolSystem : ITickable
    {

        public MoveToolSystem(
                ToolHandlers toolHandlers,
                SlotRaycastSystem slotRaycastSystem,
                BoxSelectionSystem boxSelectionSystem,
                SlotsHolder slotsHolder,
                GroundGrid groundGrid,
                ObstacleFacade.Factory obstacleFactory,
                UndoRedoSystem undoRedoSystem
            )
        {
            this.toolHandlers = toolHandlers;
            this.slotRaycastSystem = slotRaycastSystem;
            this.boxSelectionSystem = boxSelectionSystem;
            this.slotsHolder = slotsHolder;
            this.groundGrid = groundGrid;
            this.obstacleFactory = obstacleFactory;
            this.undoRedoSystem = undoRedoSystem;
        }

        private readonly ToolHandlers toolHandlers;
        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly BoxSelectionSystem boxSelectionSystem;
        private readonly SlotsHolder slotsHolder;
        private readonly GroundGrid groundGrid;
        private readonly ObstacleFacade.Factory obstacleFactory;
        private readonly UndoRedoSystem undoRedoSystem;

        private List<Vector3> relatives = new List<Vector3>();
        private List<ObstacleModel> oldObstacleModels = new List<ObstacleModel>();


        private List<GameObject> selectedGOs;
        private List<ObstacleFacade> selectedObstacles;
        public void Tick()
        {
            if (toolHandlers.CurrentToolType != ToolType.Move)
                return;

            if (EventSystem.current.IsPointerOverGameObject())
                return;


            if (Input.GetMouseButtonDown(0))
            {
                relatives.Clear();
                var startPos = slotRaycastSystem.HitPos;
                var currentHitID = slotRaycastSystem.CurrentGroundSlotID;

                toolHandlers.CheckSlotsInBrush(currentHitID, toolHandlers.BrushOddScaleInt);
                selectedGOs = toolHandlers.SelectedGOs;
                selectedObstacles = toolHandlers.SelectedObstacles;
                for (int i = 0; i < selectedGOs.Count; i++)
                {
                    var relative = selectedGOs[i].transform.position - startPos;
                    relatives.Add(relative);
                }
            }

            if (Input.GetMouseButton(0))
            {
                for (int i = 0; i < selectedGOs.Count; i++)
                {
                    selectedGOs[i].transform.position = slotRaycastSystem.HitPos + relatives[i];
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                RemoveItemsFromOldSlot();
                PlacementItemsToNewSlot();
                undoRedoSystem.AppendStatus();
            }
        }



        private void RemoveItemsFromOldSlot()
        {
            for (int i = 0; i < selectedObstacles.Count; i++)
            {
                var obstacle = selectedObstacles[i];

                oldObstacleModels.Add(slotsHolder.slotMap.TryGetObstacleModel(obstacle.slotID));

                slotsHolder.slotMap.RemoveSlotItem(obstacle.slotID);
            }
        }
        private void PlacementItemsToNewSlot()
        {

            bool isOutRange = false;

            for (int i = 0; i < selectedObstacles.Count; i++)
            {
                var obstacle = selectedObstacles[i];
                var newTransPos = obstacle.transform.position;

                var newSlotID = slotsHolder.slotMap.TranPos2SlotID(newTransPos, groundGrid);

                if (newSlotID.y != obstacle.slotID.y)
                {
                    Debug.LogWarning("y 不相同");
                }

                newSlotID.y = obstacle.slotID.y;//some times it's make y different

                Debug.Log(newTransPos + ":" + newSlotID);
                if (!slotRaycastSystem.CheckInIDRange(newSlotID))//如果新位置在地图外面，则释放这个对象
                {
                    isOutRange = true;
                    obstacleFactory.DeSpawn(obstacle);
                    continue;
                }


                //移除新位置上的已经存在的对象
                var stayedItem = slotsHolder.slotMap.TryGetItem(newSlotID);
                if (stayedItem != null)
                    slotsHolder.slotMap.ReleaseSlotItem(newSlotID, obstacleFactory);

                obstacle.SetSlotID(newSlotID);
                slotsHolder.slotMap.SetSlotItem(newSlotID, obstacle, oldObstacleModels[i]);

            }
            oldObstacleModels.Clear();

            if (isOutRange)
            {
                boxSelectionSystem.ClearSelections();
            }

        }

    }
}