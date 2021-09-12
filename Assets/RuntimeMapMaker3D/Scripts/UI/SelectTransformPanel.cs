// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using System;

namespace RMM3D
{
    public class SelectTransformPanel : MonoBehaviour, IInitializable, ITickable
    {
        [Inject]
        public void Construct(SelectionSystem selectionSystem)
        {
            this.selectionSystem = selectionSystem;
        }

        [SerializeField] private Toggle positionToggle;
        [SerializeField] private Toggle rotateToggle;
        [SerializeField] private Toggle scaleToggle;

        [SerializeField] private TMP_InputField positionX;
        [SerializeField] private TMP_InputField positionY;
        [SerializeField] private TMP_InputField positionZ;
        [SerializeField] private TMP_InputField rotateX;
        [SerializeField] private TMP_InputField rotateY;
        [SerializeField] private TMP_InputField rotateZ;
        [SerializeField] private TMP_InputField scaleX;
        [SerializeField] private TMP_InputField scaleY;
        [SerializeField] private TMP_InputField scaleZ;


        private SelectionSystem selectionSystem;

        public void Initialize()
        {
            positionToggle.onValueChanged.AddListener((x) => { selectionSystem.SelectionType = SelectionType.Position; });
            rotateToggle.onValueChanged.AddListener((x) => { selectionSystem.SelectionType = SelectionType.Rotation; });
            scaleToggle.onValueChanged.AddListener((x) => { selectionSystem.SelectionType = SelectionType.Scale; });


            positionX.onValueChanged.AddListener(x => {
                if (selectionSystem.SelectedGO == null) return;
                float v = Convert.ToSingle(x);
                selectionSystem.SelectedGO.transform.localPosition = selectionSystem.SelectedGO.transform.localPosition.WithX(v); 
            });
            positionY.onValueChanged.AddListener(x => {
                if (selectionSystem.SelectedGO == null) return;
                float v = Convert.ToSingle(x);
                selectionSystem.SelectedGO.transform.localPosition = selectionSystem.SelectedGO.transform.localPosition.WithY(v);
            });
            positionZ.onValueChanged.AddListener(x => {
                if (selectionSystem.SelectedGO == null) return;
                float v = Convert.ToSingle(x);
                selectionSystem.SelectedGO.transform.localPosition = selectionSystem.SelectedGO.transform.localPosition.WithZ(v);
            });

            rotateX.onValueChanged.AddListener(x => {
                if (selectionSystem.SelectedGO == null) return;
                float v = Convert.ToSingle(x);
                selectionSystem.SelectedGO.transform.localEulerAngles = selectionSystem.SelectedGO.transform.localEulerAngles.WithX(v);
            });
            rotateY.onValueChanged.AddListener(x => {
                if (selectionSystem.SelectedGO == null) return;
                float v = Convert.ToSingle(x);
                selectionSystem.SelectedGO.transform.localEulerAngles = selectionSystem.SelectedGO.transform.localEulerAngles.WithY(v);
            });
            rotateZ.onValueChanged.AddListener(x => {
                if (selectionSystem.SelectedGO == null) return;
                float v = Convert.ToSingle(x);
                selectionSystem.SelectedGO.transform.localEulerAngles = selectionSystem.SelectedGO.transform.localEulerAngles.WithZ(v);
            });

            scaleX.onValueChanged.AddListener(x => {
                if (selectionSystem.SelectedGO == null) return;
                float v = Convert.ToSingle(x);
                selectionSystem.SelectedGO.transform.localScale = selectionSystem.SelectedGO.transform.localScale.WithX(v);
            });
            scaleY.onValueChanged.AddListener(x => {
                if (selectionSystem.SelectedGO == null) return;
                float v = Convert.ToSingle(x);
                selectionSystem.SelectedGO.transform.localScale = selectionSystem.SelectedGO.transform.localScale.WithY(v);
            });
            scaleZ.onValueChanged.AddListener(x => {
                if (selectionSystem.SelectedGO == null) return;
                float v = Convert.ToSingle(x);
                selectionSystem.SelectedGO.transform.localScale = selectionSystem.SelectedGO.transform.localScale.WithZ(v);
            });
        }

        public void Tick()
        {
            if(selectionSystem.SelectedGO == null)
            {
                return;
            }

            positionX.SetTextWithoutNotify(selectionSystem.SelectedGO.transform.localPosition.x.ToString());
            positionY.SetTextWithoutNotify(selectionSystem.SelectedGO.transform.localPosition.y.ToString());
            positionZ.SetTextWithoutNotify(selectionSystem.SelectedGO.transform.localPosition.z.ToString());
            rotateX.SetTextWithoutNotify(selectionSystem.SelectedGO.transform.localEulerAngles.x.ToString());
            rotateY.SetTextWithoutNotify(selectionSystem.SelectedGO.transform.localEulerAngles.y.ToString());
            rotateZ.SetTextWithoutNotify(selectionSystem.SelectedGO.transform.localEulerAngles.z.ToString());
            scaleX.SetTextWithoutNotify(selectionSystem.SelectedGO.transform.localScale.x.ToString());
            scaleY.SetTextWithoutNotify(selectionSystem.SelectedGO.transform.localScale.y.ToString());
            scaleZ.SetTextWithoutNotify(selectionSystem.SelectedGO.transform.localScale.z.ToString());

        }
    }
}