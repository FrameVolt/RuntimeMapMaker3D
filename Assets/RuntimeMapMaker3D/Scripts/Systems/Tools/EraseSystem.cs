using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace RMM3D
{
    public class EraseSystem : ITickable
    {

        public EraseSystem(SlotRaycastSystem slotRaycastSystem,
                            SlotsHolder slotsHolder,
                            ObstacleFacade.Factory obstacleFactory,
                            ToolHandlers toolHandlers,
                            BoxSelectionSystem boxSelectionSystem,
                            UndoRedoSystem undoRedoSystem
                            )
        {
            this.slotRaycastSystem = slotRaycastSystem;
            this.slotsHolder = slotsHolder;
            this.obstacleFactory = obstacleFactory;
            this.toolHandlers = toolHandlers;
            this.boxSelectionSystem = boxSelectionSystem;
            this.undoRedoSystem = undoRedoSystem;
        }



        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly SlotsHolder slotsHolder;
        private readonly ToolHandlers toolHandlers;
        private readonly ObstacleFacade.Factory obstacleFactory;
        private readonly BoxSelectionSystem boxSelectionSystem;
        private readonly UndoRedoSystem undoRedoSystem;

        private Vector3Int currentHitID;
        private ObstacleFacade currentObstacle;
        // Update is called once per frame
        public void Tick()
        {
            if (toolHandlers.CurrentToolType != ToolType.Erase)
                return;

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButton(0))
            {
                if (currentHitID == slotRaycastSystem.CurrentGroundSlotID)
                    return;

                if (!slotRaycastSystem.IsPlaceableIDInRnage)
                    return;

                currentHitID = slotRaycastSystem.CurrentGroundSlotID;
                //currentObstacle = slotRaycastSystem.CurrentObstacle;

                //var slotItem = slotsHolder.slotMap.TryGetItem(currentHitID);


                //if (slotItem != null)
                //{

                //    var hasSame = CheckHasSameSelectedObstacle();


                //    //如果选框内有选中的对象
                //    if (hasSame)
                //    {
                //        EraseSelections();
                //    }
                //    else
                //    {
                //        EraseSingle();
                //    }


                //}

                EraseBrush(currentHitID, toolHandlers.BrushOddScaleInt);

            }

            if (Input.GetMouseButtonUp(0))
            {
                undoRedoSystem.AppendStatus();
            }

        }

        private bool CheckHasSameSelectedObstacle()
        {
            bool sameObstacle = false;
            for (int i = 0; i < boxSelectionSystem.SelectedObstacles.Count; i++)
            {
                if (boxSelectionSystem.SelectedObstacles[i] == currentObstacle)
                {
                    sameObstacle = true;
                    break;
                }
            }
            return sameObstacle;
        }

        private void EraseSingle()
        {
            slotsHolder.slotMap.ReleaseSlotItem(currentHitID, obstacleFactory);
        }

        private void EraseSelections()
        {
            for (int i = 0; i < boxSelectionSystem.SelectedObstacles.Count; i++)
            {
                var obstacle = boxSelectionSystem.SelectedObstacles[i];
                slotsHolder.slotMap.ReleaseSlotItem(obstacle.slotID, obstacleFactory);
            }
            boxSelectionSystem.ClearSelections();
        }

        private void EraseBrush(Vector3Int centerID, Vector3Int size)
        {
            toolHandlers.CheckSlotsInBrush(centerID, size);
            List<ObstacleFacade> selectedObstacles = toolHandlers.SelectedObstacles;
            foreach (var obstacle in selectedObstacles)
            {
                slotsHolder.slotMap.ReleaseSlotItem(obstacle.slotID, obstacleFactory);
            }
        }
    }
}