using HSVPicker;
using System.Collections;
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
            SlotsHolder slotsHolder,
            UndoRedoSystem undoRedoSystem
            )
        {
            this.toolHandlers = toolHandlers;
            this.colorPicker = colorPicker;
            this.slotRaycastSystem = slotRaycastSystem;
            this.slotsHolder = slotsHolder;
            this.undoRedoSystem = undoRedoSystem;
        }
        private readonly ToolHandlers toolHandlers;
        private readonly ColorPicker colorPicker;
        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly SlotsHolder slotsHolder;
        private readonly UndoRedoSystem undoRedoSystem;
        private Vector3Int currentHitID;
        private ObstacleFacade currentObstacle;

        public void Tick()
        {
            if (toolHandlers.CurrentToolType != ToolType.ColorBrush)
                return;
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButton(0))
            {
                if (currentHitID == slotRaycastSystem.CurrentSoltID)
                    return;

                if (!slotRaycastSystem.IsPlaceableIDInRnage)
                    return;

                currentHitID = slotRaycastSystem.CurrentSoltID;
                currentObstacle = slotRaycastSystem.CurrentObstacle;

                if (currentObstacle != null)
                {
                    currentObstacle.SetColor(colorPicker.CurrentColor);
                }

            }

            if (Input.GetMouseButton(0))
            {
                undoRedoSystem.AppendStatus();
            }
        }
    }
}