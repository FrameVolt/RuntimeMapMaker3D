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
                if (currentHitID == slotRaycastSystem.CurrentSoltID)
                    return;

                if (!slotRaycastSystem.IsPlaceableIDInRnage)
                    return;

                currentHitID = slotRaycastSystem.CurrentSoltID;
                currentObstacle = slotRaycastSystem.CurrentObstacle;

                var slotItem = slotsHolder.slotMap.TryGetItem(currentHitID);


                if (slotItem != null)
                {

                    var hasSame = CheckHasSameSelectedObstacle();


                    //如果选框内有选中的对象
                    if (hasSame)
                    {
                        EraseSelections();
                    }
                    else
                    {
                        EraseSingle();
                    }


                }
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
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    for (int k = 0; k < size.z; k++)
                    {
                        Vector3Int targetSlotID = centerID + new Vector3Int(i - size.x / 2, j, k - size.z / 2);

                        if (!slotRaycastSystem.CheckInIDRange(targetSlotID))
                            continue;

                        var itemGO = slotsHolder.slotMap.TryGetItem(targetSlotID);
                        if (itemGO != null)
                        {
                            slotsHolder.slotMap.ReleaseSlotItem(currentHitID, obstacleFactory);
                        }
                    }
                }
            }
        }
    }
}