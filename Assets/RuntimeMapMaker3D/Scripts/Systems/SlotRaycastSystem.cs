using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace RMM3D
{
    public class SlotRaycastSystem : ITickable, IInitializable
    {
        public SlotRaycastSystem(GroundGrid groundGrid, SlotsHolder slotsHolder, ToolHandlers toolHandlers)
        {
            this.mainCamera = Camera.main;
            this.groundGrid = groundGrid;
            this.plane = new Plane(Vector3.up, Vector3.zero);

            this.halfXAmount = groundGrid.xAmount / 2;
            this.yAmount = groundGrid.yAmount;
            this.halfZAmount = groundGrid.zAmount / 2;
            this.slotsHolder = slotsHolder;
            this.toolHandlers = toolHandlers;
        }

        private Camera mainCamera;
        private GroundGrid groundGrid;
        private Plane plane;
        private int halfXAmount;
        private int yAmount;
        private int halfZAmount;
        private readonly SlotsHolder slotsHolder;
        private readonly ToolHandlers toolHandlers;

        private LayerMask groundLayer = 1 << LayerMask.NameToLayer("Ground");


        //public ReactiveProperty<int> GroundY { get; private set; } = new ReactiveProperty<int>();
        private int groundY;
        public int GroundY { 
            get {
                return groundY;
            }
            set {
                if (groundY == value)
                    return;
                groundY = value;
                OnChangeGroundY.Invoke(value);
            } 
        
        
        }
        public IntEvent OnChangeGroundY = new IntEvent();
        public Vector3Int CurrentSoltID { get; private set; }
        public Vector3 CurrentSoltIDPos { get; private set; }
        public Vector3 CurrentInRangeSlotPos { get; private set; }
        public Vector3Int PlaceableSlotID { get; private set; }
        public Vector3 PlaceableSlotPos { get; private set; }
        public Vector3 CurrentGroundSlotPos { get; private set; }
        public Vector3Int CurrentGroundSlotID { get; private set; }
        public Vector3Int CurrentInRangeGroundSlotID { get; private set; }
        public Vector3 CurrentInRangeGroundSlotPos { get; private set; }

        public Vector3Int InRangePlaceableID { get; private set; }

        public Vector3 HitPos { get; private set; }
        public Vector3 GroundHitPos { get; private set; }

        public bool IsPlaceableIDInRnage { get; private set; }

        public ObstacleFacade CurrentObstacle { get; private set; }

        private int maxX_ID;
        private int maxY_ID;
        private int maxZ_ID;

        //private LayerMask layerMask = LayerMask.GetMask("Default", "Obstacle", "Outline");
        private LayerMask layerMask = 
            1 << LayerMask.NameToLayer("Ground") |
            1 << LayerMask.NameToLayer("Obstacle") |
            1 << LayerMask.NameToLayer("Outline") | 
            0 << LayerMask.NameToLayer("Handler");

        public void Initialize()
        {
            maxX_ID = slotsHolder.slotMap.Solts.GetLength(0) - 1;
            maxY_ID = slotsHolder.slotMap.Solts.GetLength(1) - 1;
            maxZ_ID = slotsHolder.slotMap.Solts.GetLength(2) - 1;
        }

        public void Tick()
        {
            CalculateCurrentGroundPos();
            CalculateCurrentRayPos();
            CheckInIDRange(PlaceableSlotID);
        }

        public bool CheckInIDRange(Vector3Int slotID)
        {
            bool inRange = true;
            if (!IsBetween(slotID.x, 0, maxX_ID) || !IsBetween(slotID.y, 0, maxY_ID) || !IsBetween(slotID.z, 0, maxZ_ID))
                inRange = false;

            IsPlaceableIDInRnage = inRange;
            return inRange;
        }
        public void SetGroundY(int y)
        {
            GroundY = y;
        }

        private void CalculateCurrentGroundPos()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            float enter = 0.0f;

            var hited = plane.Raycast(ray, out enter);

            if (!hited)
                return;

            Vector3 hitPoint = ray.GetPoint(enter);



            int x = Mathf.FloorToInt(hitPoint.x + halfXAmount / groundGrid.size);
            int z = Mathf.FloorToInt(hitPoint.z + halfZAmount / groundGrid.size);

            GroundHitPos = hitPoint;
            CurrentGroundSlotID = new Vector3Int(x, GroundY, z);
            CurrentGroundSlotPos = SoltMap.GetSlotPos(CurrentGroundSlotID, groundGrid);

            CurrentInRangeGroundSlotID = new Vector3Int(Mathf.Clamp(CurrentGroundSlotID.x, 0, maxX_ID), Mathf.Clamp(CurrentGroundSlotID.y, GroundY, maxY_ID), Mathf.Clamp(CurrentGroundSlotID.z, 0, maxZ_ID));
            CurrentInRangeGroundSlotPos = SoltMap.GetSlotPos(CurrentInRangeGroundSlotID, groundGrid);
        }

        private void CalculateCurrentRayPos()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            


            //var hited = Physics.Raycast(ray, out hit, 100, layerMask);
            Vector3 offset = new Vector3(0.4F, 0, 0.4F);
            RaycastHit hit;
            var hited = Physics.BoxCast(ray.origin, (Vector3)toolHandlers.BrushOddScaleInt * 0.5f - offset, ray.direction, out hit, Quaternion.identity, 100, layerMask);

            ExtDebug.DrawBoxCastOnHit(ray.origin, (Vector3)toolHandlers.BrushOddScaleInt * 0.5f - offset, Quaternion.identity, ray.direction, hit.distance, Color.yellow);

            Debug.DrawLine(ray.origin, mainCamera.transform.position + ray.direction * hit.distance, Color.blue);

            if (!hited)
                return;

            Vector3 hitPoint = ray.GetPoint(hit.distance);

            Vector3Int tempHitID = new Vector3Int();
            Vector3Int tempPlaceableID = new Vector3Int();
            tempHitID.x = Mathf.FloorToInt(hitPoint.x + halfXAmount / groundGrid.size);
            tempHitID.z = Mathf.FloorToInt(hitPoint.z + halfZAmount / groundGrid.size);
            tempHitID.y = GroundY;

            tempPlaceableID = tempHitID;

            tempPlaceableID.y = Mathf.Clamp(Mathf.RoundToInt(hitPoint.y), 0, yAmount);



            //hit on Obstacle
            //if (hit.collider.gameObject.layer != groundLayer)
            //{
            //    tempHitID = obstacleFacade.slotID;
            //    Vector3 normal = hit.normal;

            //    tempNormal = Vector3Int.RoundToInt(normal);
            //    tempPlaceableID = tempHitID + Vector3Int.RoundToInt(tempNormal);
            //}

            CurrentSoltID = tempHitID;
            CurrentSoltIDPos = SoltMap.GetSlotPos(tempHitID, groundGrid);
            CurrentInRangeSlotPos = SoltMap.GetSlotPos(tempHitID, groundGrid);

            PlaceableSlotID = tempPlaceableID;
            PlaceableSlotPos = SoltMap.GetSlotPos(tempPlaceableID, groundGrid);

            InRangePlaceableID = new Vector3Int(Mathf.Clamp(PlaceableSlotID.x, 0, maxX_ID), Mathf.Clamp(PlaceableSlotID.y, GroundY, maxY_ID), Mathf.Clamp(PlaceableSlotID.z, 0, maxZ_ID));

            HitPos = hit.point;
        }

        public bool IsBetween(float inputValue, float bound1, float bound2)
        {
            return (inputValue >= Mathf.Min(bound1, bound2) && inputValue <= Mathf.Max(bound1, bound2));
        }

      

    }
}