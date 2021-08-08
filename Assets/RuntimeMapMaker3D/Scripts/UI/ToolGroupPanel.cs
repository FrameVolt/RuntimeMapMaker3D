using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;
using TMPro;

public class ToolGroupPanel : UIPanelBase
{
    [Inject]
    public void Construct(RotateObstacleSystem rotateObstacleSystem)
    {
        this.rotateObstacleSystem = rotateObstacleSystem;
    }


    private RotateObstacleSystem rotateObstacleSystem;
    [SerializeField] private Toggle baseSelectionBtn;
    [SerializeField] private Toggle boxSelectionBtn;
    [SerializeField] private Toggle moveBtn;
    [SerializeField] private Toggle eraseBtn;
    [SerializeField] private Toggle rotateBtn;
    [SerializeField] private TMP_Text rotateText;

    [SerializeField] private RectTransform baseSelectionGroup;
    [SerializeField] private RectTransform boxSelectionGroup;
    [SerializeField] private RectTransform moveGroup;
    [SerializeField] private RectTransform eraseGroup;
    [SerializeField] private RectTransform rotateGroup;



    public ReactiveProperty<ToolType> ToolTypeRP = new ReactiveProperty<ToolType>();


    void Awake()
    {
        baseSelectionBtn.onValueChanged.AddListener((x) => { ToolTypeRP.Value = ToolType.BaseSelection; MoveDown(baseSelectionGroup, x); });
        boxSelectionBtn.onValueChanged.AddListener((x) => { ToolTypeRP.Value = ToolType.BoxSelection; MoveDown(boxSelectionGroup, x); });
        moveBtn.onValueChanged.AddListener((x) => { ToolTypeRP.Value = ToolType.Move; MoveDown(moveGroup, x); });
        eraseBtn.onValueChanged.AddListener((x) => { ToolTypeRP.Value = ToolType.Erase; MoveDown(eraseGroup, x); });
        rotateBtn.onValueChanged.AddListener((x) =>
        {
            ToolTypeRP.Value = ToolType.Rotate;

            rotateObstacleSystem.ChangeAxis();

            switch (rotateObstacleSystem.CurrentAxis.Value)
            {
                case RotateObstacleSystem.Axis.X:
                    rotateText.text = "X";
                    break;
                case RotateObstacleSystem.Axis.Y:
                    rotateText.text = "Y";
                    break;
                case RotateObstacleSystem.Axis.Z:
                    rotateText.text = "Z";
                    break;
            }
            MoveDown(rotateGroup, x);
        });

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
            case ToolType.BaseSelection:
                baseSelectionBtn.isOn = true;
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
public enum ToolType
{
    BaseSelection,
    BoxSelection,
    Move,
    Erase,
    Rotate
}