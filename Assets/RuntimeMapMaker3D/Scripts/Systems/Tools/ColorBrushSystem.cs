// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using HSVPicker;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace RMM3D
{
    public class ColorBrushSystem : ITickable
    {
        public ColorBrushSystem(
            ToolHandlers toolHandlers, 
            ColorPicker colorPicker,
            SlotRaycastSystem slotRaycastSystem,
            UndoRedoSystem undoRedoSystem
            )
        {
            this.toolHandlers = toolHandlers;
            this.colorPicker = colorPicker;
            this.slotRaycastSystem = slotRaycastSystem;
            this.undoRedoSystem = undoRedoSystem;
        }
        private readonly ToolHandlers toolHandlers;
        private readonly ColorPicker colorPicker;
        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly UndoRedoSystem undoRedoSystem;
        private Vector3Int currentHitID;

        public void Tick()
        {
            if (toolHandlers.CurrentToolType != ToolType.ColorBrush)
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

                ColorBrush(currentHitID, toolHandlers.BrushOddScaleInt);

            }

            if (Input.GetMouseButtonUp(0))
            {
                undoRedoSystem.AppendStatus();
            }
        }

        private void ColorBrush(Vector3Int centerID, Vector3Int size)
        {
            toolHandlers.CheckSlotsInBrush(centerID, size);
            List<ObstacleFacade> selectedObstacles = toolHandlers.SelectedObstacles;
            foreach (var obstacle in selectedObstacles)
            {
                obstacle.SetColor(colorPicker.CurrentColor);
            }
        }

    }
}