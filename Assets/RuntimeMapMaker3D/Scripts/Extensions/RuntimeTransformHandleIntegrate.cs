using RuntimeHandle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace RMM3D
{
    /// <summary>
    /// Integrate with RuntimeTransformHandle which is a 3rd plugin created by Peter @sHTiF Stefcek, thanks for your great work.
    /// </summary>
    [RequireComponent(typeof(RuntimeTransformHandle))]
    public class RuntimeTransformHandleIntegrate : MonoBehaviour, IInitializable, ITickable
    {
        [Inject]
        public void Contrust(
            ToolHandlers toolHandlers,
            GroundGrid groundGrid, 
            SlotRaycastSystem slotRaycastSystem, 
            SlotsHolder slotsHolder, 
            ObstacleFacade.Factory obstacleFactory,
            UndoRedoSystem undoRedoSystem,
            SaveMapSystem saveMapSystem
            )
        {
            this.toolHandlers = toolHandlers;
            this.groundGrid = groundGrid;
            this.slotsHolder = slotsHolder;
            this.slotRaycastSystem = slotRaycastSystem;
            this.obstacleFactory = obstacleFactory;
            this.undoRedoSystem = undoRedoSystem;
            this.saveMapSystem = saveMapSystem;
        }
        private ToolHandlers toolHandlers;
        private GroundGrid groundGrid;
        private SlotsHolder slotsHolder;
        private SlotRaycastSystem slotRaycastSystem;
        private ObstacleFacade.Factory obstacleFactory;
        private UndoRedoSystem undoRedoSystem;
        private SaveMapSystem saveMapSystem;


        [SerializeField] private RuntimeTransformHandle runtimeTransformHandle;

        private ObstacleFacade currentObstacle;
        private ObstacleModel oldObstacleModel;
        public void Initialize()
        {
            //runtimeTransformHandle.positionSnap = Vector3.one * groundGrid.size;
            undoRedoSystem.OnRedo += ClearTarget;
            undoRedoSystem.OnUndo += ClearTarget;
            saveMapSystem.OnLoad += ClearTarget;
            saveMapSystem.OnReset += ClearTarget;

            toolHandlers.OnChangeToolType.AddListener((x) => {
                if(x != ToolType.Selection)
                {
                    runtimeTransformHandle.Target = null;
                }
            });
        }
        private void OnDestroy()
        {
            undoRedoSystem.OnRedo -= ClearTarget;
            undoRedoSystem.OnUndo -= ClearTarget;
            saveMapSystem.OnLoad -= ClearTarget;
            saveMapSystem.OnReset -= ClearTarget;
        }

        private void ClearTarget()
        {
            runtimeTransformHandle.Target = null;
        }

        public void Tick()
        {
            if (toolHandlers.CurrentToolType != ToolType.Selection)
                return;
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonUp(0))
            {
                if (runtimeTransformHandle.Target == null)
                    return;

                Perpare();

                if (runtimeTransformHandle.type == HandleType.POSITION)
                {
                    RelpaceSoltItem();
                }
                else if (runtimeTransformHandle.type == HandleType.ROTATION)
                {
                    SetRotationToMap();
                }
                else if (runtimeTransformHandle.type == HandleType.SCALE)
                {
                    SetScaleToMap();
                }

                undoRedoSystem.AppendStatus();
            }
        }
        /// <summary>
        /// Set new rotation to slotsholder
        /// </summary>
        private void SetRotationToMap()
        {
            var obstacle = currentObstacle;
            slotsHolder.SetRoatation(obstacle.slotID, obstacle.transform.eulerAngles);
        }

        /// <summary>
        /// set new Scale to slotsholder
        /// </summary>
        private void SetScaleToMap()
        {
            var obstacle = currentObstacle;
            slotsHolder.SetScale(obstacle.slotID, obstacle.transform.localScale);
        }
        private void Perpare()
        {
            currentObstacle = runtimeTransformHandle.Target.GetComponent<ObstacleFacade>();
        }

        private void RelpaceSoltItem()
        {
            RemoveItemFromOldSlot();
            PlacementItemToNewSlot();
        }
        /// <summary>
        /// Remove old obstacle from old slot
        /// </summary>
        private void RemoveItemFromOldSlot()
        {

            var obstacle = currentObstacle;

            oldObstacleModel = slotsHolder.TryGetObstacleModel(obstacle.slotID);

            slotsHolder.RemoveSlotItem(obstacle.slotID);
        }
        /// <summary>
        /// Place this Obstacle to new slot.
        /// </summary>
        private void PlacementItemToNewSlot()
        {
            var obstacle = currentObstacle;
            var newTransPos = obstacle.transform.position;

            var newSlotID = slotsHolder.TranPos2SlotID(newTransPos, groundGrid);

            if (!slotRaycastSystem.CheckInIDRange(newSlotID))//If new slotID is out of map, despawn it
            {
                obstacleFactory.DeSpawn(obstacle);
                runtimeTransformHandle.Target = null;
                return;
            }

            //release new slot's old item, and place this obstacle on it.
            var stayedItem = slotsHolder.TryGetItem(newSlotID);
            if (stayedItem != null)
                slotsHolder.ReleaseSlotItem(newSlotID, obstacleFactory);

            obstacle.SetSlotID(newSlotID);
            slotsHolder.SetSlotItem(newSlotID, obstacle, oldObstacleModel);
            runtimeTransformHandle.Target = obstacle.gameObject.transform;
        }

       
    }
}