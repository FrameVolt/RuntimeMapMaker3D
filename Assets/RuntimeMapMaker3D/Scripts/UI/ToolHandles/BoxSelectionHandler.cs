using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
namespace RMM3D
{
    public class BoxSelectionHandler : MonoBehaviour, IInitializable, ITickable
    {
        [Inject]
        public void Construct(
          ToolHandlers toolHandlers,
          SlotRaycastSystem slotRaycastSystem,
          BoxSelectionSystem boxSelectionSystem)
        {
            this.toolHandlers = toolHandlers;
            this.slotRaycastSystem = slotRaycastSystem;
            this.boxSelectionSystem = boxSelectionSystem;
        }

        private ToolHandlers toolHandlers;
        private SlotRaycastSystem slotRaycastSystem;
        private BoxSelectionSystem boxSelectionSystem;

        [SerializeField] private Transform boxSelectionTrans;


        public void Initialize()
        {
            toolHandlers.OnChangeCurrentToolType.AddListener(toolType =>
            {
                if (toolType == ToolType.BoxSelection)
                {
                    boxSelectionTrans.localScale = Vector3.zero;
                    boxSelectionTrans.gameObject.SetActive(true);
                }
                else if (toolType != ToolType.Placement && toolType != ToolType.Move && toolType != ToolType.Erase)
                {
                    boxSelectionTrans.gameObject.SetActive(false);
                }
            });
            boxSelectionTrans.localScale = Vector3.zero;
        }

        public void Tick()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (toolHandlers.CurrentToolType != ToolType.BoxSelection)
                return;

            if (Input.GetMouseButton(0))
            {
                var selecteVector = boxSelectionSystem.EndSlotID - boxSelectionSystem.StartSlotID;

                boxSelectionTrans.position = boxSelectionSystem.StartSlotPos - new Vector3(Mathf.Sign(selecteVector.x) * 0.5f, 0.5f + slotRaycastSystem.GroundY, Mathf.Sign(selecteVector.z) * 0.5f);
                boxSelectionTrans.position = new Vector3(boxSelectionTrans.position.x, slotRaycastSystem.GroundY - 0.45f, boxSelectionTrans.position.z);
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
                boxSelectionTrans.eulerAngles = new Vector3(0, y, 0);
                boxSelectionTrans.localScale = scale;

            }
            if (Input.GetMouseButtonUp(0))
            {


            }

        }
    }
}