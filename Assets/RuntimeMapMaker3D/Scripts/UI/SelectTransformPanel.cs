// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace RMM3D
{
    public class SelectTransformPanel : MonoBehaviour, IInitializable
    {
        [Inject]
        public void Construct(SelectionSystem selectionSystem)
        {
            this.selectionSystem = selectionSystem;
        }

        [SerializeField] private Toggle positionToggle;
        [SerializeField] private Toggle rotateToggle;
        [SerializeField] private Toggle scaleToggle;

        private SelectionSystem selectionSystem;

        public void Initialize()
        {
            positionToggle.onValueChanged.AddListener((x) => { selectionSystem.SelectionType = SelectionType.Position; });
            rotateToggle.onValueChanged.AddListener((x) => { selectionSystem.SelectionType = SelectionType.Rotation; });
            scaleToggle.onValueChanged.AddListener((x) => { selectionSystem.SelectionType = SelectionType.Scale; });
        }


    }
}