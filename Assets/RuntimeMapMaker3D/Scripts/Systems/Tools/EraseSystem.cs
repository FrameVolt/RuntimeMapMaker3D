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
                            UndoRedoSystem undoRedoSystem
                            )
        {
            this.slotRaycastSystem = slotRaycastSystem;
            this.slotsHolder = slotsHolder;
            this.obstacleFactory = obstacleFactory;
            this.toolHandlers = toolHandlers;
            this.undoRedoSystem = undoRedoSystem;
        }

        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly SlotsHolder slotsHolder;
        private readonly ToolHandlers toolHandlers;
        private readonly ObstacleFacade.Factory obstacleFactory;
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

                EraseBrush(currentHitID, toolHandlers.BrushOddScaleInt);

            }

            if (Input.GetMouseButtonUp(0))
            {
                undoRedoSystem.AppendStatus();
            }

        }

        /// <summary>
        /// Erase from current slots
        /// </summary>
        /// <param name="centerID"></param>
        /// <param name="size"></param>
        private void EraseBrush(Vector3Int centerID, Vector3Int size)
        {
            toolHandlers.CheckSlotsInBrush(centerID, size);
            List<ObstacleFacade> selectedObstacles = toolHandlers.SelectedObstacles;
            foreach (var obstacle in selectedObstacles)
            {
                slotsHolder.ReleaseSlotItem(obstacle.slotID, obstacleFactory);
            }
        }
    }
}