using RuntimeHandle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace RMM3D
{
    [RequireComponent(typeof(RuntimeTransformHandle))]
    public class RuntimeTransformHandleIntegrate : MonoBehaviour, IInitializable
    {
        [Inject]
        public void Contrust(GroundGrid groundGrid)
        {
            this.groundGrid = groundGrid;

        }
        private GroundGrid groundGrid;


        [SerializeField] private RuntimeTransformHandle runtimeTransformHandle;

        private ObstacleFacade currentObstacle;

        public void Initialize()
        {
            runtimeTransformHandle.positionSnap = Vector3.one * groundGrid.size;
        }

        private void RelpaceSoltItem()
        {

        }


        private void MovePrepare()
        {
            //currentObstacle = runtimeTransformHandle.target.GetComponent<>
        }

        //private void RemoveItemsFromOldSlot()
        //{
        //    for (int i = 0; i < selectedObstacles.Count; i++)
        //    {
        //        var obstacle = selectedObstacles[i];

        //        oldObstacleModels.Add(slotsHolder.slotMap.TryGetObstacleModel(obstacle.slotID));

        //        slotsHolder.slotMap.RemoveSlotItem(obstacle.slotID);
        //    }
        //}
        //private void PlacementItemsToNewSlot()
        //{

        //    bool isOutRange = false;

        //    for (int i = 0; i < selectedObstacles.Count; i++)
        //    {
        //        var obstacle = selectedObstacles[i];
        //        var newTransPos = obstacle.transform.position;

        //        var newSlotID = slotsHolder.slotMap.TranPos2SlotID(newTransPos, groundGrid);

        //        if (newSlotID.y != obstacle.slotID.y)
        //        {
        //            Debug.LogWarning("y 不相同");
        //        }

        //        newSlotID.y = obstacle.slotID.y;//some times it's make y different

        //        Debug.Log(newTransPos + ":" + newSlotID);
        //        if (!slotRaycastSystem.CheckInIDRange(newSlotID))//如果新位置在地图外面，则释放这个对象
        //        {
        //            isOutRange = true;
        //            obstacleFactory.DeSpawn(obstacle);
        //            continue;
        //        }


        //        //移除新位置上的已经存在的对象
        //        var stayedItem = slotsHolder.slotMap.TryGetItem(newSlotID);
        //        if (stayedItem != null)
        //            slotsHolder.slotMap.ReleaseSlotItem(newSlotID, obstacleFactory);

        //        obstacle.SetSlotID(newSlotID);
        //        slotsHolder.slotMap.SetSlotItem(newSlotID, obstacle, oldObstacleModels[i]);

        //    }
        //    oldObstacleModels.Clear();

        //    if (isOutRange)
        //    {
        //        boxSelectionSystem.ClearSelections();
        //    }

        //}
    }
}