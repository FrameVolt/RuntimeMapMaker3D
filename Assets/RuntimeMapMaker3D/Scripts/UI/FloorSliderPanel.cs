using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using TMPro;

namespace RMM3D
{
    public class FloorSliderPanel : MonoBehaviour, IInitializable
    {

        [Inject]
        public void Contrust(GroundGrid groundGrid, SlotRaycastSystem slotRaycastSystem, BoxSelectionSystem boxSelectionSystem)
        {
            this.groundGrid = groundGrid;
            this.slotRaycastSystem = slotRaycastSystem;
            this.boxSelectionSystem = boxSelectionSystem;
        }


        [SerializeField] private Slider slider;
        [SerializeField] private Button upBtn;
        [SerializeField] private Button downBtn;
        [SerializeField] private TMP_InputField inputField;
        private GroundGrid groundGrid;
        private SlotRaycastSystem slotRaycastSystem;
        private BoxSelectionSystem boxSelectionSystem;
        public void Initialize()
        {
            slider.maxValue = groundGrid.yAmount - 1;
            upBtn.onClick.AddListener(() =>
            {
                slider.value++;
            });

            downBtn.onClick.AddListener(() =>
            {
                slider.value--;
            });

            slider.onValueChanged.AddListener((v) =>
            {
                inputField.text = v.ToString();
            });

            inputField.onValueChanged.AddListener((s) =>
            {
                if (s == "")
                {
                    inputField.SetTextWithoutNotify("0");
                    slider.SetValueWithoutNotify(0);
                    inputField.stringPosition = 1;
                    slotRaycastSystem.SetGroundY(0);
                    boxSelectionSystem.ClearSelections();

                    return;
                }

                var v = Convert.ToInt32(s);
                slider.SetValueWithoutNotify(v);
                slotRaycastSystem.SetGroundY(v);
                boxSelectionSystem.ClearSelections();
            });

        }


    }
}