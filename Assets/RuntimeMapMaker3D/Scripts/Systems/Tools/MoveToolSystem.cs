using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UniRx;
using UnityEngine.Assertions;

namespace RMM3D
{
    public class MoveToolSystem : ITickable
    {

        public MoveToolSystem(
                ToolGroupPanel toolGroupPanel,
                SlotRaycastSystem slotRaycastSystem,
                BoxSelectionSystem boxSelectionSystem,
                SlotsHolder groundSlotsHolder,
                GroundGrid groundGrid,
                ObstacleFacade.Factory obstacleFactory,
                UndoRedoSystem undoRedoSystem
            )
        {
            this.toolGroupPanel = toolGroupPanel;
            this.slotRaycastSystem = slotRaycastSystem;
            this.boxSelectionSystem = boxSelectionSystem;
            this.groundSlotsHolder = groundSlotsHolder;
            this.groundGrid = groundGrid;
            this.obstacleFactory = obstacleFactory;
            this.undoRedoSystem = undoRedoSystem;
        }

        private readonly ToolGroupPanel toolGroupPanel;
        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly BoxSelectionSystem boxSelectionSystem;
        private readonly SlotsHolder groundSlotsHolder;
        private readonly GroundGrid groundGrid;
        private readonly ObstacleFacade.Factory obstacleFactory;
        private readonly UndoRedoSystem undoRedoSystem;

        private List<Vector3> relatives = new List<Vector3>();
        public ObstacleFacade CurrentGrabedObstacle { get; private set; }
        private List<ObstacleModel> oldObstacleModels = new List<ObstacleModel>();


        public void Tick()
        {
            if (toolGroupPanel.ToolTypeRP.Value != ToolType.Move)
                return;

            if (EventSystem.current.IsPointerOverGameObject())
                return;


            if (Input.GetMouseButtonDown(0))
            {
                CurrentGrabedObstacle = slotRaycastSystem.CurrentObstacle;

                if (CurrentGrabedObstacle == null)
                    return;

                //如果没有点选中的Items,则使用当前点的对象
                var hasSame = CheckHasSameSelectedObstacle();
                if (hasSame == false)
                {
                    boxSelectionSystem.ClearSelections();
                    boxSelectionSystem.SetDefaultSelection(slotRaycastSystem.CurrentGroundSlotID, CurrentGrabedObstacle.gameObject, CurrentGrabedObstacle);
                }


                relatives.Clear();
                var startPos = slotRaycastSystem.GroundHitPos;

                for (int i = 0; i < boxSelectionSystem.SelectedGOs.Count; i++)
                {
                    var relative = boxSelectionSystem.SelectedGOs[i].transform.position - startPos;
                    relatives.Add(relative);
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (CurrentGrabedObstacle == null)
                    return;

                for (int i = 0; i < boxSelectionSystem.SelectedGOs.Count; i++)
                {
                    boxSelectionSystem.SelectedGOs[i].transform.position = slotRaycastSystem.GroundHitPos + relatives[i];
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                //Debug.Log("AmountItem1: " + groundSlotsHolder.slotMap.AmountItem() + ":" + groundSlotsHolder.slotMap.AmountObstacleModel());
                RemoveItemsFromOldSlot();
                //Debug.Log("AmountItem1: " + groundSlotsHolder.slotMap.AmountItem() + ":" + groundSlotsHolder.slotMap.AmountObstacleModel());
                PlacementItemsToNewSlot();
                //Debug.Log("AmountItem1: " + groundSlotsHolder.slotMap.AmountItem() + ":" + groundSlotsHolder.slotMap.AmountObstacleModel());
                undoRedoSystem.AppendStatus();
            }
        }


        private bool CheckHasSameSelectedObstacle()
        {
            bool sameObstacle = false;
            for (int i = 0; i < boxSelectionSystem.SelectedObstacles.Count; i++)
            {
                if (boxSelectionSystem.SelectedObstacles[i] == CurrentGrabedObstacle)
                {
                    sameObstacle = true;
                    break;
                }
            }
            return sameObstacle;
        }



        private void RemoveItemsFromOldSlot()
        {
            for (int i = 0; i < boxSelectionSystem.SelectedObstacles.Count; i++)
            {
                var obstacle = boxSelectionSystem.SelectedObstacles[i];

                oldObstacleModels.Add(groundSlotsHolder.slotMap.TryGetObstacleModel(obstacle.slotID));

                groundSlotsHolder.slotMap.RemoveSlotItem(obstacle.slotID);
            }
        }
        private void PlacementItemsToNewSlot()
        {

            bool isOutRange = false;

            for (int i = 0; i < boxSelectionSystem.SelectedObstacles.Count; i++)
            {
                var obstacle = boxSelectionSystem.SelectedObstacles[i];
                var newTransPos = obstacle.transform.position;

                var newSlotID = groundSlotsHolder.slotMap.TranPos2SlotID(newTransPos, groundGrid);

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
                var stayedItem = groundSlotsHolder.slotMap.TryGetItem(newSlotID);
                if (stayedItem != null)
                    groundSlotsHolder.slotMap.ReleaseSlotItem(newSlotID, obstacleFactory);

                obstacle.transform.position = SoltMap.GetSlotPos(newSlotID, groundGrid);
                obstacle.slotID = newSlotID;
                groundSlotsHolder.slotMap.SetSlotItem(newSlotID, obstacle.gameObject, oldObstacleModels[i]);

            }
            oldObstacleModels.Clear();

            if (isOutRange)
            {
                boxSelectionSystem.ClearSelections();
            }

        }

    }
}