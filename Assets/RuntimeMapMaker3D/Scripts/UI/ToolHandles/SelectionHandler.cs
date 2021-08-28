using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
namespace RMM3D
{
    public class SelectionHandler : MonoBehaviour, IInitializable, ITickable
    {
        [Inject]
        public void Construct(
            ToolHandlers toolHandlers,
            SlotRaycastSystem slotRaycastSystem,
            PlacementSystem placementSystem
            )
        {
            this.toolHandlers = toolHandlers;
            this.slotRaycastSystem = slotRaycastSystem;
            this.placementSystem = placementSystem;
        }


        private ToolHandlers toolHandlers;
        private SlotRaycastSystem slotRaycastSystem;
        private PlacementSystem placementSystem;

        [SerializeField] private Transform trans;
        [SerializeField] private Transform scaleTrans;


        public void Initialize()
        {
            toolHandlers.OnChangeCurrentToolType.AddListener(toolType =>
            {
                if (toolType == ToolType.Selection)
                {
                    trans.gameObject.SetActive(true);
                }
                else
                {
                    trans.gameObject.SetActive(false);
                }
            });
        }


        public void Tick()
        {

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (toolHandlers.CurrentToolType == ToolType.Selection)
            {
                trans.position = slotRaycastSystem.CurrentSoltIDPos;
            }
        }
    }
}