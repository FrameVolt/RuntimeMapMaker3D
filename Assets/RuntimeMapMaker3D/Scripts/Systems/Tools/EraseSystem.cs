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
                            SlotsHolder groundSlotsHolder,
                            ObstacleFacade.Factory obstacleFactory,
                            ToolGroupPanel toolGroupPanel,
                            BoxSelectionSystem boxSelectionSystem,
                            UndoRedoSystem undoRedoSystem
                            )
        {
            this.slotRaycastSystem = slotRaycastSystem;
            this.groundSlotsHolder = groundSlotsHolder;
            this.obstacleFactory = obstacleFactory;
            this.toolGroupPanel = toolGroupPanel;
            this.boxSelectionSystem = boxSelectionSystem;
            this.undoRedoSystem = undoRedoSystem;
        }



        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly SlotsHolder groundSlotsHolder;
        private readonly ToolGroupPanel toolGroupPanel;
        private readonly ObstacleFacade.Factory obstacleFactory;
        private readonly BoxSelectionSystem boxSelectionSystem;
        private readonly UndoRedoSystem undoRedoSystem;

        private Vector3Int currentHitID;
        private ObstacleFacade currentObstacle;
        // Update is called once per frame
        public void Tick()
        {
            if (toolGroupPanel.ToolTypeRP.Value != ToolType.Erase)
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

                var slotItem = groundSlotsHolder.slotMap.TryGetItem(currentHitID);


                if (slotItem != null)
                {

                    var hasSame = CheckHasSameSelectedObstacle();


                    //如果选框内有选中的对象
                    if (hasSame)
                    {
                        for (int i = 0; i < boxSelectionSystem.SelectedObstacles.Count; i++)
                        {
                            var obstacle = boxSelectionSystem.SelectedObstacles[i];
                            groundSlotsHolder.slotMap.ReleaseSlotItem(obstacle.slotID, obstacleFactory);
                        }
                        boxSelectionSystem.ClearSelections();
                    }
                    else
                    {
                        groundSlotsHolder.slotMap.ReleaseSlotItem(currentHitID, obstacleFactory);
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

    }
}