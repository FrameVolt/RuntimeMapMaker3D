using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace RMM3D
{
    public class ToolHandlers: IInitializable
    {


        private ToolType currentToolType;
        public ToolType CurrentToolType
        {
            get
            {
                return currentToolType;
            }
            set
            {
                if (currentToolType == value)
                    return;
                currentToolType = value;

                OnChangeCurrentToolType.Invoke(value);
            }
        }

        public ChangeToolTypeEvent OnChangeCurrentToolType = new ChangeToolTypeEvent();

        public void Initialize()
        {
            CurrentToolType = ToolType.Placement;
        }
    }

    public enum ToolType
    {
        BoxSelection,
        Placement,
        Move,
        Erase,
        Rotate,
        ColorBrush
    }

    public enum Axis
    {
        X, Y, Z
    }
}