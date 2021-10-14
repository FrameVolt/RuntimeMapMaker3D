// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using Zenject;

namespace RMM3D
{
    /// <summary>
    /// Globle ray cast system for RMM3D
    /// </summary>
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
        public Vector3Int CurrentGroundSlotID { get; private set; }
        public Vector3 CurrentGroundSlotPos { get; private set; }

        public Vector3 HitPos { get; private set; }

        public bool IsPlaceableIDInRnage { get; private set; }

        public ObstacleFacade CurrentObstacle { get; private set; }
        public Vector3Int CurrentObstacleSlotID { get; private set; }

        private int maxX_ID;
        private int maxY_ID;
        private int maxZ_ID;


        private LayerMask obstacleLayMask = 1 << LayerMask.NameToLayer("Obstacle") | 0 << LayerMask.NameToLayer("Handler");

        //private LayerMask layerMask = LayerMask.GetMask("Default", "Obstacle", "Outline");
        private LayerMask groundLayerMask = 
            1 << LayerMask.NameToLayer("Ground") |
            //1 << LayerMask.NameToLayer("Obstacle") |
            //1 << LayerMask.NameToLayer("Outline") | 
            0 << LayerMask.NameToLayer("Handler");

        public void Initialize()
        {
            maxX_ID = slotsHolder.Slots.GetLength(0) - 1;
            maxY_ID = slotsHolder.Slots.GetLength(1) - 1;
            maxZ_ID = slotsHolder.Slots.GetLength(2) - 1;
        }

        public void Tick()
        {
            CalculateObstacleRayPos();
            CalculateGroundRayPos();
            CheckInIDRange(CurrentGroundSlotID);
        }
        /// <summary>
        /// Check if this slot inside range
        /// </summary>
        /// <param name="slotID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Ray cast on Obstacle
        /// </summary>
        private void CalculateObstacleRayPos()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            var hited = Physics.Raycast(ray, out hit, 100, obstacleLayMask);

            if (!hited)
                return;

            var obstacleFacade = hit.collider.GetComponentInParent<ObstacleFacade>();

            if (obstacleFacade == null)
                return;

            CurrentObstacle = obstacleFacade;
            CurrentObstacleSlotID = obstacleFacade.slotID;
        }


        /// <summary>
        /// Ray cast on ground
        /// </summary>
        private void CalculateGroundRayPos()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            Vector3 offset = new Vector3(0.4F, 0, 0.4F);
            Vector3 halfExtents = (Vector3)toolHandlers.BrushOddScaleInt * 0.5f - offset;
            halfExtents.y = 0;

            RaycastHit hit;
            var hited = Physics.BoxCast(ray.origin, halfExtents, ray.direction, out hit, Quaternion.identity, 100, groundLayerMask);

            ExtDebug.DrawBoxCastOnHit(ray.origin, halfExtents, Quaternion.identity, ray.direction, hit.distance, Color.yellow);
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

            CurrentGroundSlotID = tempHitID;
            CurrentGroundSlotPos = slotsHolder.GetSlotPos(tempHitID, groundGrid);

            HitPos = hitPoint;
        }

        public bool IsBetween(float inputValue, float bound1, float bound2)
        {
            return (inputValue >= Mathf.Min(bound1, bound2) && inputValue <= Mathf.Max(bound1, bound2));
        }
    }
}