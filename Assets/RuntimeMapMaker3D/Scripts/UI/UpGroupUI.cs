// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RMM3D
{
    public class UpGroupUI : UIPanelBase
    {
        [Inject]
        public void Construct(CameraManager cameraManager)
        {
            this.cameraManager = cameraManager;
        }

        private CameraManager cameraManager;

        [SerializeField] private Button playBtn;

        [SerializeField] private RectTransform leftGroupTrans;
        [SerializeField] private RectTransform toolGroupTrans;
        [SerializeField] private RectTransform obstacleBtnsGroupTrans;

        private Mode mode = Mode.EditMode;

        void Start()
        {
            playBtn.onClick.AddListener(OnPlayClick);
        }

        protected override void OnShow()
        {
            base.OnShow();
        }

        protected override void OnHide()
        {
            base.OnHide();
        }


        private void OnPlayClick()
        {
            if (mode == Mode.EditMode)
            {
                mode = Mode.EditorPlayMode;
                cameraManager.ChangeState(CameraState.FreeFly);
            }
            else
            {
                mode = Mode.EditMode;
                cameraManager.ChangeState(CameraState.Follow);
            }

        }

        enum Mode
        {
            EditorPlayMode,
            EditMode
        }

    }
}