// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace RMM3D
{
    public class ToolHandlers : IInitializable
    {
        public ToolHandlers(GroundGrid groundGrid, SlotsHolder slotsHolder)
        {
            this.groundGrid = groundGrid;
            this.slotsHolder = slotsHolder;
        }
        private readonly GroundGrid groundGrid;
        private readonly SlotsHolder slotsHolder;

        private int maxX_ID;
        private int maxY_ID;
        private int maxZ_ID;

        private ToolType currentToolType;
        public ToolType CurrentToolType
        {
            get
            {
                return currentToolType;
            }
            set
            {
                if (currentToolType == value)
                    return;
                currentToolType = value;

                OnChangeToolType.Invoke(value);
            }
        }

        public ChangeToolTypeEvent OnChangeToolType = new ChangeToolTypeEvent();
        public List<GameObject> SelectedGOs { get; private set; } = new List<GameObject>();
        public List<Vector3Int> SelectedSlotsOfGO { get; private set; } = new List<Vector3Int>();
        public List<ObstacleFacade> SelectedObstacles { get; private set; } = new List<ObstacleFacade>();
        public List<Vector3Int> SlotsInBrushGround { get; private set; } = new List<Vector3Int>();
        public List<Vector3Int> SlotsInBrush { get; private set; } = new List<Vector3Int>();


        private Vector3 brushScale = Vector3.one;
        public Vector3 BrushScale
        {
            get
            {
                return brushScale;
            }
            set
            {
                if (brushScale == value)
                {
                    return;
                }
                var tempX = Mathf.Clamp(value.x, 1, groundGrid.xAmount);
                var tempY = Mathf.Clamp(value.y, 1, groundGrid.yAmount);
                var tempZ = Mathf.Clamp(value.z, 1, groundGrid.zAmount);
                Vector3 temp = new Vector3(tempX, tempY, tempZ);

                temp = new Vector3Int(Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y), Mathf.RoundToInt(temp.z));


                brushScale = temp;
                onHandlerScaleChangeEvent.Invoke(temp);
            }
        }

        public Vector3Int BrushOddScaleInt
        {
            get {
                var oddScale = OddScale(new Vector3Int(Mathf.FloorToInt(BrushScale.x), Mathf.FloorToInt(BrushScale.y), Mathf.FloorToInt(BrushScale.z)));
                return oddScale;
            }
        }
        private Vector3Int OddScale(Vector3Int oragin)
        {
            var oddScale = new Vector3Int(oragin.x * 2 - 1, oragin.y, oragin.z * 2 - 1);
            return oddScale;
        }

        public HandlerScaleChangeEvent onHandlerScaleChangeEvent = new HandlerScaleChangeEvent();

        public void Initialize()
        {
            CurrentToolType = ToolType.Placement;

            maxX_ID = slotsHolder.Slots.GetLength(0) - 1;
            maxY_ID = slotsHolder.Slots.GetLength(1) - 1;
            maxZ_ID = slotsHolder.Slots.GetLength(2) - 1;

        }

        public void CheckSlotsInBrush(Vector3Int centerID, Vector3Int size)
        {
            SlotsInBrush.Clear();
            SelectedGOs.Clear();
            SelectedSlotsOfGO.Clear();
            SelectedObstacles.Clear();
            SlotsInBrushGround.Clear();

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    for (int k = 0; k < size.z; k++)
                    {
                        Vector3Int targetSlotID = centerID + new Vector3Int(Mathf.CeilToInt(i - size.x / 2f), j, Mathf.CeilToInt(k - size.z / 2f));

                        //check solt in range
                        if (!CheckInIDRange(targetSlotID)) {
                            continue;
                        }
                        SlotsInBrush.Add(targetSlotID);

                        var go = slotsHolder.TryGetItem(targetSlotID);
                        if (go != null)
                        {
                            SelectedSlotsOfGO.Add(targetSlotID);
                            SelectedGOs.Add(go);
                            SelectedObstacles.Add(go.GetComponent<ObstacleFacade>());
                        }
                        if (j == 0)
                        {
                            SlotsInBrushGround.Add(targetSlotID);
                        }
                    }
                }
            }
        }

        public bool CheckInIDRange(Vector3Int slotID)
        {
            bool inRange = true;
            if (!IsBetween(slotID.x, 0, maxX_ID) || !IsBetween(slotID.y, 0, maxY_ID) || !IsBetween(slotID.z, 0, maxZ_ID))
                inRange = false;

            return inRange;
        }
        public bool IsBetween(float inputValue, float bound1, float bound2)
        {
            return (inputValue >= Mathf.Min(bound1, bound2) && inputValue <= Mathf.Max(bound1, bound2));
        }
    }
    public enum ToolType
    {
        Selection,
        BoxSelection,
        Placement,
        Move,
        Erase,
        Rotate,
        ColorBrush
    }

    public enum Axis
    {
        X, Y, Z
    }
}