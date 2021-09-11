using RuntimeHandle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace RMM3D
{
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
            UndoRedoSystem undoRedoSystem
            )
        {
            this.toolHandlers = toolHandlers;
            this.groundGrid = groundGrid;
            this.slotsHolder = slotsHolder;
            this.slotRaycastSystem = slotRaycastSystem;
            this.obstacleFactory = obstacleFactory;
            this.undoRedoSystem = undoRedoSystem;
        }
        private ToolHandlers toolHandlers;
        private GroundGrid groundGrid;
        private SlotsHolder slotsHolder;
        private SlotRaycastSystem slotRaycastSystem;
        private ObstacleFacade.Factory obstacleFactory;
        private UndoRedoSystem undoRedoSystem;


        [SerializeField] private RuntimeTransformHandle runtimeTransformHandle;

        private ObstacleFacade currentObstacle;
        private ObstacleModel oldObstacleModel;
        public void Initialize()
        {
            //runtimeTransformHandle.positionSnap = Vector3.one * groundGrid.size;
            undoRedoSystem.OnRedo += () => { runtimeTransformHandle.Target = null; };
            undoRedoSystem.OnUndo += () => { runtimeTransformHandle.Target = null; };

            toolHandlers.OnChangeToolType.AddListener((x) => {
                if(x != ToolType.Selection)
                {
                    runtimeTransformHandle.Target = null;
                }
            });
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

        private void SetRotationToMap()
        {
            var obstacle = currentObstacle;
            slotsHolder.slotMap.SetRoatation(obstacle.slotID, obstacle.transform.eulerAngles);
        }
        private void SetScaleToMap()
        {
            var obstacle = currentObstacle;
            slotsHolder.slotMap.SetScale(obstacle.slotID, obstacle.transform.localScale);
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

        private void RemoveItemFromOldSlot()
        {

            var obstacle = currentObstacle;

            oldObstacleModel = slotsHolder.slotMap.TryGetObstacleModel(obstacle.slotID);

            slotsHolder.slotMap.RemoveSlotItem(obstacle.slotID);
        }
        private void PlacementItemToNewSlot()
        {
            var obstacle = currentObstacle;
            var newTransPos = obstacle.transform.position;

            var newSlotID = slotsHolder.slotMap.TranPos2SlotID(newTransPos, groundGrid);

            if (!slotRaycastSystem.CheckInIDRange(newSlotID))//如果新位置在地图外面，则释放这个对象
            {
                obstacleFactory.DeSpawn(obstacle);
                runtimeTransformHandle.Target = null;
                return;
            }

            //移除新位置上的已经存在的对象
            var stayedItem = slotsHolder.slotMap.TryGetItem(newSlotID);
            if (stayedItem != null)
                slotsHolder.slotMap.ReleaseSlotItem(newSlotID, obstacleFactory);

            obstacle.SetSlotID(newSlotID);
            slotsHolder.slotMap.SetSlotItem(newSlotID, obstacle, oldObstacleModel);
            runtimeTransformHandle.Target = obstacle.gameObject.transform;
        }

       
    }
}