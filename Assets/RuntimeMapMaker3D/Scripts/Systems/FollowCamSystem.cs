// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;

namespace RMM3D
{
    public class FollowCamSystem : ICameraSystem
    {

        public float smoothSpeed = 0.125f;
        public Vector3 offset;

        private Transform camTrans;
        private Transform target;

        public void EnterState()
        {

        }
        public void UpdateState()
        {
            if (target == null)
                return;

            Vector3 desiredPosition = target.TransformPoint(offset);
            Vector3 smoothedPosition = Vector3.Lerp(camTrans.position, desiredPosition, smoothSpeed);
            camTrans.position = smoothedPosition;

            camTrans.LookAt(target);
        }
        public void ExitState()
        {

        }


    }
}