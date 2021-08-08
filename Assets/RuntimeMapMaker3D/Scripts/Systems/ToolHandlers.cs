using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class ToolHandlers : MonoBehaviour, IInitializable, ITickable
{

    [Inject]
    public void Construct(SlotRaycastSystem slotRaycastSystem, 
            ToolGroupPanel toolGroupPanel,
            RotateObstacleSystem rotateObstacleSystem,
            MoveToolSystem moveToolSystem
        )
    {
        this.slotRaycastSystem = slotRaycastSystem;
        this.toolGroupPanel = toolGroupPanel;
        this.rotateObstacleSystem = rotateObstacleSystem;
        this.moveToolSystem = moveToolSystem;
    }


    public Vector3 BoxSelectionTransSize { get; private set; }

    [SerializeField] private Transform baseTrans;
    [SerializeField] private Transform boxSelectionTrans;
    [SerializeField] private Transform rotateHandlerTrans;
    [SerializeField] private Material rotateHandlerMat;
    private SlotRaycastSystem slotRaycastSystem;
    private ToolGroupPanel toolGroupPanel;
    private RotateObstacleSystem rotateObstacleSystem;
    private MoveToolSystem moveToolSystem;

    public Vector3Int StartSlotID { get; private set; }
    private Vector3 startSlotPos;
    public Vector3Int EndSlotID { get; private set; }

    public void Initialize()
    {
        toolGroupPanel.ToolTypeRP.Subscribe(toolType =>
        {
            if (toolType == ToolType.Rotate)
            {
                baseTrans.gameObject.SetActive(false);
                rotateHandlerTrans.gameObject.SetActive(true);
            }
            else if (toolType == ToolType.BoxSelection)
            {
                StartSlotID = Vector3Int.zero;
                EndSlotID = Vector3Int.zero;
                boxSelectionTrans.localScale = Vector3.zero;
                boxSelectionTrans.gameObject.SetActive(true);
                baseTrans.gameObject.SetActive(false);
                rotateHandlerTrans.gameObject.SetActive(false);
            }
            else if (toolType == ToolType.Move)
            {
                baseTrans.gameObject.SetActive(true);
                rotateHandlerTrans.gameObject.SetActive(false);
                boxSelectionTrans.gameObject.SetActive(false);
            }
            else if (toolType == ToolType.Erase)
            {
                baseTrans.gameObject.SetActive(true);
                rotateHandlerTrans.gameObject.SetActive(false);
                boxSelectionTrans.gameObject.SetActive(false);
            }
            else
            {
                baseTrans.gameObject.SetActive(true);
                rotateHandlerTrans.gameObject.SetActive(false);

            }
        });

        rotateObstacleSystem.CurrentAxis.Subscribe(axis => {
            switch (axis)
            {
                case RotateObstacleSystem.Axis.X:
                    rotateHandlerTrans.eulerAngles = new Vector3(60, 0, 90);
                    break;
                case RotateObstacleSystem.Axis.Y:
                    rotateHandlerTrans.eulerAngles = Vector3.zero;
                    break;
                case RotateObstacleSystem.Axis.Z:
                    rotateHandlerTrans.eulerAngles = new Vector3(90,0,0);
                    break;
                default:
                    break;
            }

        });

        boxSelectionTrans.localScale = Vector3.zero;
    }

    public void Tick()
    {

        if (EventSystem.current.IsPointerOverGameObject())
            return;



        if (toolGroupPanel.ToolTypeRP.Value == ToolType.BaseSelection)
        {
            Vector3 temp;
            if (Input.GetMouseButton(0))
            {
                temp = slotRaycastSystem.CurrentInRangeSlotPos;
            }
            else
            {
                temp = slotRaycastSystem.PlaceableSlotPos;
            }

            var pos = new Vector3(temp.x, temp.y, temp.z);
            baseTrans.position = pos;

        }
        else if (toolGroupPanel.ToolTypeRP.Value == ToolType.Move)
        {
            if (moveToolSystem.CurrentGrabedObstacle)
            {
                baseTrans.position = moveToolSystem.CurrentGrabedObstacle.transform.position;
            }
            else
            {
                var pos = slotRaycastSystem.CurrentInRangeSlotPos;
                baseTrans.position = pos;
            }
            
        }
        else if (toolGroupPanel.ToolTypeRP.Value == ToolType.Rotate)
        {
            RotateHandlerColor();
            Vector3 temp = slotRaycastSystem.CurrentInRangeSlotPos;

            var pos = new Vector3(temp.x, temp.y, temp.z);
            rotateHandlerTrans.position = pos;
        }
        else if (toolGroupPanel.ToolTypeRP.Value == ToolType.BoxSelection)
        {
            BoxSelection();
        }
        else
        {
            var pos = slotRaycastSystem.CurrentInRangeSlotPos;
            baseTrans.position = pos;
        }
        
    }

    private void RotateHandlerColor()
    {
        if(slotRaycastSystem.CurrentObstacle != null)
        {
            rotateHandlerMat.color = Color.white;
        }
        else
        {
            rotateHandlerMat.color = new Color(1,1,1,0.3f);
        }
    }


    private void BoxSelection()
    {

        if (Input.GetMouseButtonDown(0))
        {
            StartSlotID = slotRaycastSystem.CurrentInRangeGroundSlotID;
            startSlotPos = slotRaycastSystem.CurrentInRangeGroundSlotPos;

        }

        if (Input.GetMouseButton(0))
        {
            EndSlotID = slotRaycastSystem.CurrentInRangeGroundSlotID;

            var selecteVector = EndSlotID - StartSlotID;

            boxSelectionTrans.position = startSlotPos - new Vector3(Mathf.Sign(selecteVector.x) * 0.5f, 0.5f + slotRaycastSystem.GroundY.Value, Mathf.Sign(selecteVector.z) * 0.5f);
            boxSelectionTrans.position = new Vector3(boxSelectionTrans.position.x, slotRaycastSystem.GroundY.Value - 0.45f, boxSelectionTrans.position.z);
            float y = 0;
            Vector3 scale = Vector3.zero;

            //180:(1,1),  90:(-1,1),  0:(-1,-1),  270:(1,-1)
            if ((Mathf.Sign(selecteVector.x) > 0 && Mathf.Sign(selecteVector.z) > 0))
            {
                y = 180;
                scale = new Vector3(Mathf.Abs(selecteVector.x) + Mathf.Sign(selecteVector.x), 0.1f, Mathf.Abs(selecteVector.z) + Mathf.Sign(selecteVector.z));
            }
            else if ((Mathf.Sign(selecteVector.x) > 0 && Mathf.Sign(selecteVector.z) < 0))
            {
                y = 270;
                scale = new Vector3(Mathf.Abs(selecteVector.z) - Mathf.Sign(selecteVector.z), 0.1f, Mathf.Abs(selecteVector.x) + Mathf.Sign(selecteVector.x));
            }
            else if ((Mathf.Sign(selecteVector.x) < 0 && Mathf.Sign(selecteVector.z) > 0))
            {
                y = 90;
                scale = new Vector3(Mathf.Abs(selecteVector.z) - Mathf.Sign(selecteVector.x), 0.1f, Mathf.Abs(selecteVector.x) + Mathf.Sign(selecteVector.z));
            }
            else if ((Mathf.Sign(selecteVector.x) < 0 && Mathf.Sign(selecteVector.z) < 0))
            {
                y = 0;
                scale = new Vector3(Mathf.Abs(selecteVector.x) - Mathf.Sign(selecteVector.x), 0.1f, Mathf.Abs(selecteVector.z) - Mathf.Sign(selecteVector.z));
            }
            boxSelectionTrans.eulerAngles = new Vector3(0,y,0);
            boxSelectionTrans.localScale = scale;
            BoxSelectionTransSize = scale;

        }
        if (Input.GetMouseButtonUp(0))
        {
            

        }

    }


}
