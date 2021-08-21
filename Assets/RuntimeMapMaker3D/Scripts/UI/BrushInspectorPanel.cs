using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using System;
using UnityEngine.UI;

namespace RMM3D
{
    public class BrushInspectorPanel : MonoBehaviour, IInitializable
    {
        [Inject]
        public void Construct(ToolHandlers toolHandlers, GroundGrid groundGrid)
        {
            this.toolHandlers = toolHandlers;
            this.groundGrid = groundGrid;
        }

        [SerializeField] private TMP_InputField inputX;
        [SerializeField] private TMP_InputField inputY;
        [SerializeField] private TMP_InputField inputZ;

        [SerializeField] private Button xIncBtn;
        [SerializeField] private Button xDecBtn;
        [SerializeField] private Button yIncBtn;
        [SerializeField] private Button yDecBtn;
        [SerializeField] private Button zIncBtn;
        [SerializeField] private Button zDecBtn;

        [SerializeField] private Button rotateXaxisBtn;
        [SerializeField] private Button rotateYaxisBtn;
        [SerializeField] private Button rotateZaxisBtn;
        [SerializeField] private Button resetBtn;

        [SerializeField] private Slider xSlider;
        [SerializeField] private Slider ySlider;
        [SerializeField] private Slider zSlider;


        private ToolHandlers toolHandlers;
        private GroundGrid groundGrid;

        public void Initialize()
        {
            inputX.onValueChanged.AddListener(v => { toolHandlers.BrushScale = toolHandlers.BrushScale.WithX(Int32.Parse(v)); });
            inputY.onValueChanged.AddListener(v => { toolHandlers.BrushScale = toolHandlers.BrushScale.WithY(Int32.Parse(v)); });
            inputZ.onValueChanged.AddListener(v => { toolHandlers.BrushScale = toolHandlers.BrushScale.WithZ(Int32.Parse(v)); });

            xIncBtn.onClick.AddListener(()=> { toolHandlers.BrushScale = toolHandlers.BrushScale.WithIncX(1); });
            xDecBtn.onClick.AddListener(()=> { toolHandlers.BrushScale = toolHandlers.BrushScale.WithIncX(-1); });
            yIncBtn.onClick.AddListener(()=> { toolHandlers.BrushScale = toolHandlers.BrushScale.WithIncY(1); });
            yDecBtn.onClick.AddListener(()=> { toolHandlers.BrushScale = toolHandlers.BrushScale.WithIncY(-1); });
            zIncBtn.onClick.AddListener(()=> { toolHandlers.BrushScale = toolHandlers.BrushScale.WithIncZ(1); });
            zDecBtn.onClick.AddListener(()=> { toolHandlers.BrushScale = toolHandlers.BrushScale.WithIncZ(-1); });

            toolHandlers.onHandlerScaleChangeEvent.AddListener(v => {
                inputX.SetTextWithoutNotify(v.x.ToString());
                inputY.SetTextWithoutNotify(v.y.ToString());
                inputZ.SetTextWithoutNotify(v.z.ToString());

                xSlider.SetValueWithoutNotify(v.x);
                ySlider.SetValueWithoutNotify(v.y);
                zSlider.SetValueWithoutNotify(v.z);
            });


            rotateXaxisBtn.onClick.AddListener(() => {
                    Vector3 targetVector = new Vector3(toolHandlers.BrushScale.x, toolHandlers.BrushScale.z, toolHandlers.BrushScale.y);

                toolHandlers.BrushScale = targetVector;
                });
            rotateYaxisBtn.onClick.AddListener(() => {
                Vector3 targetVector = new Vector3(toolHandlers.BrushScale.z, toolHandlers.BrushScale.y, toolHandlers.BrushScale.x);

                toolHandlers.BrushScale = targetVector;
            });
            rotateZaxisBtn.onClick.AddListener(() => {
                Vector3 targetVector = new Vector3(toolHandlers.BrushScale.y, toolHandlers.BrushScale.x, toolHandlers.BrushScale.z);

                toolHandlers.BrushScale = targetVector;
            });

            resetBtn.onClick.AddListener(() => {
                toolHandlers.BrushScale = Vector3.one;
            });

            xSlider.minValue = 1;
            xSlider.maxValue = groundGrid.xAmount / 2;
            ySlider.minValue = 1;
            ySlider.maxValue = groundGrid.yAmount;
            zSlider.minValue = 1;
            zSlider.maxValue = groundGrid.zAmount / 2;

            xSlider.onValueChanged.AddListener((v)=> {
                toolHandlers.BrushScale = toolHandlers.BrushScale.WithX(v); 
            });
            ySlider.onValueChanged.AddListener((v)=> {
                toolHandlers.BrushScale = toolHandlers.BrushScale.WithY(v);
            });
            zSlider.onValueChanged.AddListener((v)=> { 
                toolHandlers.BrushScale = toolHandlers.BrushScale.WithZ(v);
            });



        }

    }
}