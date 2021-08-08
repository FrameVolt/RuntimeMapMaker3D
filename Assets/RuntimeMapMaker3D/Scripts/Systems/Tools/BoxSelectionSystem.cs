using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
public class BoxSelectionSystem : ITickable
{
    [Inject]
    public BoxSelectionSystem(SlotRaycastSystem slotRaycastSystem,
           SlotsHolder groundSlotsHolder,
           ToolGroupPanel toolGroupPanel,
           GroundGrid groundGrid,
            OutLineSystem obstacleOutLineSystem
        )
    {
        this.slotRaycastSystem = slotRaycastSystem;
        this.toolGroupPanel = toolGroupPanel;
        this.obstacleOutLineSystem = obstacleOutLineSystem;

        int size = groundGrid.xAmount * groundGrid.yAmount * groundGrid.zAmount;
        selectedSlotIDs = new List<Vector3Int>(size);
        coveringSlotIDs = new List<Vector3Int>();
        coveringGroundSlotIDs = new List<Vector3Int>();
        SelectedGOs = new List<GameObject>();
        SelectedObstacles = new List<ObstacleFacade>();
        this.groundGrid = groundGrid;

        this.slotMap = groundSlotsHolder.slotMap;
    }

    private SlotRaycastSystem slotRaycastSystem;
    private ToolGroupPanel toolGroupPanel;
    private GroundGrid groundGrid;
    private SoltMap slotMap;
    private OutLineSystem obstacleOutLineSystem;

    private Vector3Int startSlot;
    private Vector3Int endSlot;

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
        if (toolGroupPanel.ToolTypeRP.Value != ToolType.BoxSelection)
            return;
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!slotRaycastSystem.IsPlaceableIDInRnage)
                return;

            startSlot = slotRaycastSystem.CurrentInRangeGroundSlotID;
        }
        if (Input.GetMouseButtonUp(0))
        {
            endSlot = slotRaycastSystem.CurrentInRangeGroundSlotID;
            OnMouseButtonUp();
        }

    }


    private void OnMouseButtonUp()
    {
        ClearSelections();

        var selecteVector = endSlot - startSlot;

        for (int i = startSlot.x; i != endSlot.x + Mathf.CeilToInt(Mathf.Sign(selecteVector.x)); i += Mathf.CeilToInt(Mathf.Sign(selecteVector.x)))
        {
            for (int j = slotRaycastSystem.GroundY.Value; j < groundGrid.yAmount; j++)
            {
                for (int k = startSlot.z; k != endSlot.z + Mathf.CeilToInt(Mathf.Sign(selecteVector.z)); k += Mathf.CeilToInt(Mathf.Sign(selecteVector.z)))
                {
                    var slotID = new Vector3Int(i,j,k);
                    
                    var go = slotMap.TryGetItem(slotID);
                    if (go != null)
                    {
                        selectedSlotIDs.Add(slotID);
                        SelectedGOs.Add(go);
                        SelectedObstacles.Add(go.GetComponent<ObstacleFacade>());
                    }

                    coveringSlotIDs.Add(slotID);

                    if(j == 0)
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
