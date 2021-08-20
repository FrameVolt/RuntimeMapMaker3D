using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using System;
using UnityEngine.UI;

namespace RMM3D
{
    public class PlacementInspectorPanel : MonoBehaviour, IInitializable
    {
        [Inject]
        public void Construct(PlacementSystem placementSystem)
        {
            this.placementSystem = placementSystem;
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

        private PlacementSystem placementSystem;


        public void Initialize()
        {
            //placementSystem.HandlerScale = 

            inputX.onValueChanged.AddListener(v => { placementSystem.HandlerScale = placementSystem.HandlerScale.WithX(Int32.Parse(v)); });
            inputY.onValueChanged.AddListener(v => { placementSystem.HandlerScale = placementSystem.HandlerScale.WithY(Int32.Parse(v)); });
            inputZ.onValueChanged.AddListener(v => { placementSystem.HandlerScale = placementSystem.HandlerScale.WithZ(Int32.Parse(v)); });

            xIncBtn.onClick.AddListener(()=> { placementSystem.HandlerScale = placementSystem.HandlerScale.WithIncX(1); });
            xDecBtn.onClick.AddListener(()=> { placementSystem.HandlerScale = placementSystem.HandlerScale.WithIncX(-1); });
            yIncBtn.onClick.AddListener(()=> { placementSystem.HandlerScale = placementSystem.HandlerScale.WithIncY(1); });
            yDecBtn.onClick.AddListener(()=> { placementSystem.HandlerScale = placementSystem.HandlerScale.WithIncY(-1); });
            zIncBtn.onClick.AddListener(()=> { placementSystem.HandlerScale = placementSystem.HandlerScale.WithIncZ(1); });
            zDecBtn.onClick.AddListener(()=> { placementSystem.HandlerScale = placementSystem.HandlerScale.WithIncZ(-1); });

            placementSystem.onHandlerScaleChangeEvent.AddListener(v => {
                inputX.SetTextWithoutNotify(v.x.ToString());
                inputY.SetTextWithoutNotify(v.y.ToString());
                inputZ.SetTextWithoutNotify(v.z.ToString());


            });

        }

    }
}