using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] private Toggle placementBtn;
        [SerializeField] private Toggle boxSelectionBtn;
        [SerializeField] private Toggle moveBtn;
        [SerializeField] private Toggle eraseBtn;
        [SerializeField] private Toggle rotateBtn;
        [SerializeField] private Toggle colorBrushBtn;
        [SerializeField] private TMP_Text rotateText;

        [SerializeField] private RectTransform placementGroup;
        [SerializeField] private RectTransform boxSelectionGroup;
        [SerializeField] private RectTransform moveGroup;
        [SerializeField] private RectTransform eraseGroup;
        [SerializeField] private RectTransform rotateGroup;
        [SerializeField] private RectTransform ColorBrushGroup;

        [SerializeField] private Image brushColorImage;
        [SerializeField] private GameObject colorPickerPanel;
        public void Initialize()
        {
            placementBtn.onValueChanged.AddListener((x) => { 
                toolHandlers.CurrentToolType = ToolType.Placement; 
                MoveDown(placementGroup, x); 
            });
            boxSelectionBtn.onValueChanged.AddListener((x) => { toolHandlers.CurrentToolType = ToolType.BoxSelection; MoveDown(boxSelectionGroup, x); });
            moveBtn.onValueChanged.AddListener((x) => { toolHandlers.CurrentToolType = ToolType.Move; MoveDown(moveGroup, x); });
            eraseBtn.onValueChanged.AddListener((x) => { toolHandlers.CurrentToolType = ToolType.Erase; MoveDown(eraseGroup, x); });
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
                MoveDown(rotateGroup, x);
            });
            colorBrushBtn.onValueChanged.AddListener((x) => { 
                toolHandlers.CurrentToolType = ToolType.ColorBrush; 
                MoveDown(ColorBrushGroup, x);
                colorPickerPanel.SetActive(x);
            });

            colorPicker.onValueChanged.AddListener((c) => { brushColorImage.color = c; });

            colorPickerPanel.SetActive(false);

            brushColorImage.color = colorPicker.CurrentColor;
        }

        public void MoveDown(RectTransform rectTransform, bool isOn)
        {
            if (isOn)
            {

            }
            else
            {

            }

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