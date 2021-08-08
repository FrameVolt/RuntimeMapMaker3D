using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UniRx;

public class RotateObstacleSystem : ITickable
{
    [Inject]
    public RotateObstacleSystem(
        SlotRaycastSystem slotRaycastSystem, 
        SlotsHolder groundSlotsHolder, 
        ArrangeObstacleBtnsPanel arrangeObstacleBtnSystem,
        ToolGroupPanel toolGroupPanel,
        UndoRedoSystem undoRedoSystem)
    {
        this.slotRaycastSystem = slotRaycastSystem;
        this.groundSlotsHolder = groundSlotsHolder;
        this.arrangeObstacleBtnSystem = arrangeObstacleBtnSystem;
        this.toolGroupPanel = toolGroupPanel;
        this.undoRedoSystem = undoRedoSystem;
    }


    private SlotRaycastSystem slotRaycastSystem;
    private SlotsHolder groundSlotsHolder;
    private ArrangeObstacleBtnsPanel arrangeObstacleBtnSystem;
    private ToolGroupPanel toolGroupPanel;
    private UndoRedoSystem undoRedoSystem;

    public ReactiveProperty<Axis> CurrentAxis { get; private set; } = new ReactiveProperty<Axis>();

    public void Tick()
    {
        if (toolGroupPanel.ToolTypeRP.Value != ToolType.Rotate)
            return;
        if (arrangeObstacleBtnSystem.CurrentObstacleData == null)
            return;
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (!slotRaycastSystem.IsPlaceableIDInRnage)
            return;


        if (Input.GetMouseButtonDown(0))
        {
            var currentHitID = slotRaycastSystem.CurrentSoltID;

            var slot = groundSlotsHolder.slotMap.Solts[currentHitID.x, currentHitID.y, currentHitID.z];

            if (slot.item != null)
            {
                var trans = slot.item.transform;

                switch (CurrentAxis.Value)
                {
                    case Axis.X:
                        trans.Rotate(new Vector3(-90, 0, 0),Space.World);
                        break;
                    case Axis.Y:
                        trans.Rotate(new Vector3(0, 90, 0), Space.World);
                        break;
                    case Axis.Z:
                        trans.Rotate(new Vector3(0, 0, 90), Space.World);
                        break;
                    default:
                        break;
                }

                slot.rotation = trans.eulerAngles;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            undoRedoSystem.AppendStatus();
        }
    }

    public void ChangeAxis()
    {
        var array = Enum.GetValues(typeof(Axis));
        var length = array.Length;
        int index = (int)CurrentAxis.Value;
        index++;
        if (index >= length)
            index = 0;
        CurrentAxis.Value = (Axis)index;
    }

    public enum Axis
    {
        X,Y,Z
    }

}
