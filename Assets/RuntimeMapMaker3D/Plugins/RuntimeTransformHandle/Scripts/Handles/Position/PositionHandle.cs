using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class PositionHandle : MonoBehaviour
    {
        protected RuntimeTransformHandle _parentTransformHandle;
        protected List<PositionAxis> _axes;
        protected List<PositionPlane> _planes;

        private Color rightColor = /*Color.red;*/new Color(0.71f, 0.55f, 0.25f);
        private Color upColor = /*Color.green;*/new Color(0.95f, 0.92f, 0.81f);
        private Color forwardColor = /*Color.blue;*/new Color(0.07f, 0.15f, 0.12f);
        private Color rightUpPlaneColor = /*new Color(0, 0, 1, .2f);*/new Color(0.07f, 0.15f, 0.12f,0.5f);
        private Color upForwardPlaneColor = /*new Color(1, 0, 0, .2f);*/new Color(0.71f, 0.55f, 0.25f,0.5f);
        private Color rightForwardPlaneColor = /*new Color(0, 1, 0, .2f);*/new Color(0.95f, 0.92f, 0.81f,0.5f);


        public PositionHandle Initialize(RuntimeTransformHandle p_runtimeHandle)
        {
            _parentTransformHandle = p_runtimeHandle;
            transform.SetParent(_parentTransformHandle.transform, false);

            _axes = new List<PositionAxis>();

            if (_parentTransformHandle.axes == HandleAxes.X || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<PositionAxis>()
                    .Initialize(_parentTransformHandle, Vector3.right, -Vector3.forward, rightColor));
            
            if (_parentTransformHandle.axes == HandleAxes.Y || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<PositionAxis>()
                    .Initialize(_parentTransformHandle, Vector3.up, Vector3.forward, upColor));

            if (_parentTransformHandle.axes == HandleAxes.Z || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<PositionAxis>()
                    .Initialize(_parentTransformHandle, Vector3.forward, Vector3.right, forwardColor));

            _planes = new List<PositionPlane>();
            
            if (_parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XYZ)
                _planes.Add(new GameObject().AddComponent<PositionPlane>()
                    .Initialize(_parentTransformHandle, Vector3.right, Vector3.up, -Vector3.forward, rightUpPlaneColor));

            if (_parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _planes.Add(new GameObject().AddComponent<PositionPlane>()
                    .Initialize(_parentTransformHandle, Vector3.up, Vector3.forward, Vector3.right, upForwardPlaneColor));

            if (_parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _planes.Add(new GameObject().AddComponent<PositionPlane>()
                    .Initialize(_parentTransformHandle, Vector3.right, Vector3.forward, Vector3.up, rightForwardPlaneColor));

            return this;
        }

        public void Destroy()
        {
            foreach (PositionAxis axis in _axes)
                Destroy(axis.gameObject);
            
            foreach (PositionPlane plane in _planes)
                Destroy(plane.gameObject);
            
            Destroy(this);
        }
    }
}