// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace RMM3D
{
    public class BoxSelectionSystem : ITickable
    {
        [Inject]
        public BoxSelectionSystem(SlotRaycastSystem slotRaycastSystem,
               SlotsHolder slotsHolder,
               GroundGrid groundGrid,
               OutLineSystem obstacleOutLineSystem, 
               ToolHandlers toolHandlers
            )
        {
            this.slotRaycastSystem = slotRaycastSystem;
            this.obstacleOutLineSystem = obstacleOutLineSystem;
            this.toolHandlers = toolHandlers;
            int size = groundGrid.xAmount * groundGrid.yAmount * groundGrid.zAmount;
            selectedSlotIDs = new List<Vector3Int>(size);
            coveringSlotIDs = new List<Vector3Int>();
            coveringGroundSlotIDs = new List<Vector3Int>();
            SelectedGOs = new List<GameObject>();
            SelectedObstacles = new List<ObstacleFacade>();
            this.groundGrid = groundGrid;
            this.slotsHolder = slotsHolder;

        }

        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly GroundGrid groundGrid;
        private readonly OutLineSystem obstacleOutLineSystem;
        private readonly ToolHandlers toolHandlers;
        private readonly SlotsHolder slotsHolder;

        public Vector3Int StartSlotID { get; private set; }
        public Vector3Int EndSlotID { get; private set; }
        public Vector3 StartSlotPos { get; private set; }


        private List<Vector3Int> selectedSlotIDs;
        public List<GameObject> SelectedGOs { get; private set; }
        public List<ObstacleFacade> SelectedObstacles { get; private set; }

        public List<Vector3Int> coveringSlotIDs { get; private set; }
        public List<Vector3Int> coveringGroundSlotIDs { get; private set; }

        public void ClearSelections()
        {
            RemoveOutlines();
            selectedSlotIDs.Clear();
            SelectedGOs.Clear();
            SelectedObstacles.Clear();
            coveringSlotIDs.Clear();
            coveringGroundSlotIDs.Clear();
        }

        public void SetDefaultSelection(Vector3Int slotID, GameObject item, ObstacleFacade obstacleFacade)
        {
            selectedSlotIDs.Add(slotID);
            SelectedGOs.Add(item);
            SelectedObstacles.Add(obstacleFacade);
            AddOutlines();
        }


        public void Tick()
        {
            if (toolHandlers.CurrentToolType != ToolType.BoxSelection)
                return;
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (!slotRaycastSystem.IsPlaceableIDInRnage)
                    return;

                StartSlotID = slotRaycastSystem.CurrentGroundSlotID;
                StartSlotPos = slotRaycastSystem.CurrentGroundSlotPos;
            }
            if (Input.GetMouseButton(0))
            {
                EndSlotID = slotRaycastSystem.CurrentGroundSlotID;
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnMouseButtonUp();
            }

        }


        private void OnMouseButtonUp()
        {
            ClearSelections();

            var selecteVector = EndSlotID - StartSlotID;

            for (int i = StartSlotID.x; i != EndSlotID.x + Mathf.CeilToInt(Mathf.Sign(selecteVector.x)); i += Mathf.CeilToInt(Mathf.Sign(selecteVector.x)))
            {
                for (int j = slotRaycastSystem.GroundY; j < groundGrid.yAmount; j++)
                {
                    for (int k = StartSlotID.z; k != EndSlotID.z + Mathf.CeilToInt(Mathf.Sign(selecteVector.z)); k += Mathf.CeilToInt(Mathf.Sign(selecteVector.z)))
                    {
                        var slotID = new Vector3Int(i, j, k);

                        var go = slotsHolder.TryGetItem(slotID);
                        if (go != null)
                        {
                            selectedSlotIDs.Add(slotID);
                            SelectedGOs.Add(go);
                            SelectedObstacles.Add(go.GetComponent<ObstacleFacade>());
                        }

                        coveringSlotIDs.Add(slotID);

                        if (j == 0)
                        {
                            coveringGroundSlotIDs.Add(slotID);
                        }
                    }
                }
            }

            AddOutlines();

        }


        private void AddOutlines()
        {
            for (int i = 0; i < SelectedGOs.Count; i++)
            {
                obstacleOutLineSystem.SetOutlines(SelectedGOs[i]);
            }
        }

        private void RemoveOutlines()
        {
            obstacleOutLineSystem.RemoveAllOutlines();
        }

    }
}