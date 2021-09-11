// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using UnityEngine.UI;
namespace RMM3D
{
    public class BottomGroupPanel : MonoBehaviour
    {
        [SerializeField] private Button arrowBtn;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private Image arrowImage;
        [SerializeField] private Sprite up;
        [SerializeField] private Sprite down;


        private bool expended = false;

        void Start()
        {
            arrowBtn.onClick.AddListener(OnArrowClick);
        }

        void OnArrowClick()
        {
            if(expended == false)
            {
                panelRect.sizeDelta = new Vector2(0, 800);
                arrowImage.sprite = down;
            }
            else
            {
                panelRect.sizeDelta = new Vector2(0, 100);
                arrowImage.sprite = up;
            }
            expended = !expended;
        }
    }
}