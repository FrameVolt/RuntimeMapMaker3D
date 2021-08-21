using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace RMM3D
{
    public class ToolHandlers : IInitializable
    {
        public ToolHandlers(GroundGrid groundGrid, SoltMap slotMap)
        {
            this.groundGrid = groundGrid;
            this.slotMap = slotMap;
        }
        private readonly GroundGrid groundGrid;
        private readonly SoltMap slotMap;

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

                OnChangeCurrentToolType.Invoke(value);
            }
        }

        public ChangeToolTypeEvent OnChangeCurrentToolType = new ChangeToolTypeEvent();
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
                brushScale = temp;
                onHandlerScaleChangeEvent.Invoke(temp);
            }
        }

        public Vector3Int BrushOddScaleInt
        {
            get {
                var oddScale = OddScale(new Vector3Int(Mathf.RoundToInt(BrushScale.x), Mathf.RoundToInt(BrushScale.y), Mathf.RoundToInt(BrushScale.z)));
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

                        //TODO limit  solt in range

                        SlotsInBrush.Add(targetSlotID);

                        var go = slotMap.TryGetItem(targetSlotID);
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
    }
    public enum ToolType
    {
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