// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using Zenject;

namespace RMM3D
{
    public class UIManager : MonoBehaviour, IInitializable
    {
        [Inject]
        public void Construct(ConfirmPop.Factory confirmPopFactory)
        {
            this.confirmPopFactory = confirmPopFactory;

        }

        public RectTransform canvasTrans;
        public RectTransform CanvasTrans
        {
            get
            {
                if (canvasTrans == null)
                {
                    canvasTrans = GetComponent<RectTransform>();
                }
                return canvasTrans;
            }
        }

        private ConfirmPop.Factory confirmPopFactory;
        public void Initialize()
        {

        }


        public void Show(string panelName)
        {

        }

        public void Pop(string popName)
        {
            var popGo = confirmPopFactory.Create();

            var rt = popGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.SetParent(CanvasTrans, false);

        }

    }

    public class PopType
    {
        public const string ConfirmPop = "ConfirmPop";

    }
}