using RuntimeHandle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;

namespace RMM3D
{
    public class SelectionSystem : ITickable, IInitializable
    {
        [Inject]
        public SelectionSystem(
            ToolHandlers toolHandlers, 
            OutLineSystem obstacleOutLineSystem, 
            SlotsHolder slotsHolder,
            SlotRaycastSystem slotRaycastSystem,
            RuntimeTransformHandle runtimeTransformHandle
        )
        {
            this.toolHandlers = toolHandlers;
            this.obstacleOutLineSystem = obstacleOutLineSystem;
            this.slotsHolder = slotsHolder;
            this.slotRaycastSystem = slotRaycastSystem;
            this.runtimeTransformHandle = runtimeTransformHandle;
        }

        private readonly ToolHandlers toolHandlers;
        private readonly OutLineSystem obstacleOutLineSystem;
        private readonly SlotsHolder slotsHolder;
        private readonly SlotRaycastSystem slotRaycastSystem;
        private readonly RuntimeTransformHandle runtimeTransformHandle;

        private SelectionType selectionType;
        public SelectionType SelectionType
        {
            get
            {
                return selectionType;
            }
            set
            {
                if (selectionType == value)
                    return;
                selectionType = value;
                OnSelectionTypeChange.Invoke(value);
            }
        }
        public OnSelectionTypeChange OnSelectionTypeChange = new OnSelectionTypeChange();

        public GameObject SelectedGO { get; private set; }
        public void Initialize()
        {
            OnSelectionTypeChange.AddListener((x) =>
            {
                switch (x)
                {
                    case SelectionType.Position:
                        runtimeTransformHandle.type = HandleType.POSITION;
                        break;
                    case SelectionType.Rotation:
                        runtimeTransformHandle.type = HandleType.ROTATION;
                        break;
                    case SelectionType.Scale:
                        runtimeTransformHandle.type = HandleType.SCALE;
                        break;
                    default:
                        break;
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
                OnMouseButtonUp();
            }
        }

        private void OnMouseButtonUp()
        {
            ClearSelections();
            var slotID = slotRaycastSystem.CurrentObstacleSlotID;
            var go = slotsHolder.TryGetItem(slotID);
            if (go != null)
            {
                SelectedGO = go;
                runtimeTransformHandle.Target = go.transform;
            }
        }

        public void ClearSelections()
        {
            RemoveOutlines();
        }

        private void AddOutlines()
        {
            obstacleOutLineSystem.SetOutlines(SelectedGO);
        }

        private void RemoveOutlines()
        {
            obstacleOutLineSystem.RemoveAllOutlines();
        }


    }

    public enum SelectionType
    {
        Position,
        Rotation,
        Scale
    }
    [System.Serializable]
    public class OnSelectionTypeChange : UnityEvent<SelectionType>
    {

    }

}