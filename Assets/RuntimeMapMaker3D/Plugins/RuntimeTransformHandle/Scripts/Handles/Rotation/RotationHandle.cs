using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class RotationHandle : MonoBehaviour
    {
        protected RuntimeTransformHandle _parentTransformHandle;
        protected List<RotationAxis> _axes;

        private Color rightColor = /*Color.red;*/new Color(0.71f, 0.55f, 0.25f);
        private Color upColor = /*Color.green;*/new Color(0.95f, 0.92f, 0.81f);
        private Color forwardColor = /*Color.blue;*/new Color(0.07f, 0.15f, 0.12f);


        public RotationHandle Initialize(RuntimeTransformHandle p_parentTransformHandle)
        {
            _parentTransformHandle = p_parentTransformHandle;
            transform.SetParent(_parentTransformHandle.transform, false);

            _axes = new List<RotationAxis>();
            
            if (_parentTransformHandle.axes == HandleAxes.X || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<RotationAxis>()
                    .Initialize(_parentTransformHandle, Vector3.right, Vector3.up, rightColor));
            
            if (_parentTransformHandle.axes == HandleAxes.Y || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<RotationAxis>()
                    .Initialize(_parentTransformHandle, Vector3.up, Vector3.right, upColor));

            if (_parentTransformHandle.axes == HandleAxes.Z || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<RotationAxis>()
                    .Initialize(_parentTransformHandle, Vector3.forward, Vector3.up, forwardColor));

            return this;
        }

        public void Destroy()
        {
            foreach (RotationAxis axis in _axes)
                Destroy(axis.gameObject);
            
            Destroy(this);
        }
    }
}