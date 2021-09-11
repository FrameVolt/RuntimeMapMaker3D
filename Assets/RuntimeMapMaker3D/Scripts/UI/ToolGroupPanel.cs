// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using HSVPicker;

namespace RMM3D
{
    public class ToolGroupPanel : UIPanelBase, IInitializable
    {
        [Inject]
        public void Construct(
            RotateObstacleSystem rotateObstacleSystem, 
            ToolHandlers toolHandlers, 
            ColorPicker colorPicker)
        {
            this.rotateObstacleSystem = rotateObstacleSystem;
            this.toolHandlers = toolHandlers;
            this.colorPicker = colorPicker;
        }

        private RotateObstacleSystem rotateObstacleSystem;
        private ToolHandlers toolHandlers;
        private ColorPicker colorPicker;

        [SerializeField] private Toggle selectionBtn;
        [SerializeField] private Toggle placementBtn;
        [SerializeField] private Toggle boxSelectionBtn;
        [SerializeField] private Toggle moveBtn;
        [SerializeField] private Toggle eraseBtn;
        [SerializeField] private Toggle rotateBtn;
        [SerializeField] private Toggle colorBrushBtn;
        [SerializeField] private TMP_Text rotateText;

        [SerializeField] private Image brushColorImage;
        [SerializeField] private GameObject brushPanel;
        [SerializeField] private GameObject colorPickerPanel;
        [SerializeField] private GameObject transformInspectorPanel;
        public void Initialize()
        {
            selectionBtn.onValueChanged.AddListener((x) => { 
                toolHandlers.CurrentToolType = ToolType.Selection;
                transformInspectorPanel.SetActive(x);
                brushPanel.SetActive(!x);
            });
            placementBtn.onValueChanged.AddListener((x) => { 
                toolHandlers.CurrentToolType = ToolType.Placement;
            });
            boxSelectionBtn.onValueChanged.AddListener((x) => { toolHandlers.CurrentToolType = ToolType.BoxSelection;  });
            moveBtn.onValueChanged.AddListener((x) => { toolHandlers.CurrentToolType = ToolType.Move; });
            eraseBtn.onValueChanged.AddListener((x) => { toolHandlers.CurrentToolType = ToolType.Erase; });
            rotateBtn.onValueChanged.AddListener((x) =>
            {
                toolHandlers.CurrentToolType = ToolType.Rotate;

                rotateObstacleSystem.ChangeAxis();

                switch (rotateObstacleSystem.CurrentAxis)
                {
                    case Axis.X:
                        rotateText.text = "X";
                        break;
                    case Axis.Y:
                        rotateText.text = "Y";
                        break;
                    case Axis.Z:
                        rotateText.text = "Z";
                        break;
                }
            });
            colorBrushBtn.onValueChanged.AddListener((x) => { 
                toolHandlers.CurrentToolType = ToolType.ColorBrush; 
                colorPickerPanel.SetActive(x);
            });

            colorPicker.onValueChanged.AddListener((c) => { brushColorImage.color = c; });

            colorPickerPanel.SetActive(false);
            transformInspectorPanel.SetActive(false);
            brushColorImage.color = colorPicker.CurrentColor;
        }



        public void ChangeToolType(ToolType toolType)
        {
            switch (toolType)
            {
                case ToolType.Placement:
                    placementBtn.isOn = true;
                    break;
                case ToolType.BoxSelection:
                    boxSelectionBtn.isOn = true;
                    break;
                case ToolType.Move:
                    moveBtn.isOn = true;
                    break;
                case ToolType.Erase:
                    eraseBtn.isOn = true;
                    break;
                case ToolType.Rotate:
                    rotateBtn.isOn = true;
                    break;
                default:
                    break;
            }
        }

    }
    
}