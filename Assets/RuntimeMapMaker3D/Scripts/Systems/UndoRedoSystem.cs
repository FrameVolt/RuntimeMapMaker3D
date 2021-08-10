using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Zenject;

namespace RMM3D
{
    public class UndoRedoSystem : IInitializable
    {
        public UndoRedoSystem(UndoRedoSystem.Settings settings,
            SlotsHolder groundSlotsHolder
            )
        {
            _undoList = new LinkedList<Solt[,,]>();
            _redoList = new LinkedList<Solt[,,]>();

            this.max = settings.maxItems;
            this.groundSlotsHolder = groundSlotsHolder;
        }


        public event Action OnAppend = () => { };

        private readonly SlotsHolder groundSlotsHolder;
        private readonly int max;

        private LinkedList<Solt[,,]> _undoList;
        private LinkedList<Solt[,,]> _redoList;

        private bool sign = true;

        public void Initialize()
        {
            AppendStatus();
        }

        public void Clear()
        {
            _undoList.Clear();
            _redoList.Clear();
        }

        public int GetUndoVisualCount()
        {
            int result;
            if (sign) {
                result = _undoList.Count - 1;
            }
            else
            {
                result = _undoList.Count;
            }

            return result;
        }
        public int GetRedoVisualCount()
        {
            int result;
            if (sign)
            {
                result = _redoList.Count;
            }
            else
            {
                result = _redoList.Count - 1;
            }

            return result;
        }

        public void AppendStatus()
        {


            var newSolts = SoltMap.Copy(groundSlotsHolder.slotMap.Solts);

            //if (_undoList.Count > 0 && _undoList.Last.Value == str)
            //{
            //    return;
            //}

            if (sign == false)
            {
                sign = true;
                //AppendStatus();

                Solt[,,] slots = _redoList.Last.Value;
                _undoList.AddLast(slots);
                _redoList.RemoveLast();
            }

            if (_undoList.Count > max)
            {
                _undoList.RemoveFirst();
            }
            
            _undoList.AddLast(newSolts);
            _redoList.Clear();
            OnAppend.Invoke();
        }


        public void Undo()
        {
            if (CanPerformUndo())
            {
                Solt[,,] slots = _undoList.Last.Value;

                if (sign == true)
                {
                    sign = false;
                    _redoList.AddLast(slots);
                    _undoList.RemoveLast();
                    slots = _undoList.Last.Value;
                }

                var newSolts = SoltMap.Copy(slots);

                groundSlotsHolder.ResetSoltMap();
                groundSlotsHolder.SetSoltMap(newSolts);

                _redoList.AddLast(slots);
                _undoList.RemoveLast();

            }
            else
            {
                Debug.Log("Undo list is empty.");
            }

        }

        public void Redo()
        {
            if (CanPerformRedo())
            {
                Solt[,,] slots = _redoList.Last.Value;

                if (sign == false)
                {
                    sign = true;
                    _undoList.AddLast(slots);
                    _redoList.RemoveLast();
                    slots = _redoList.Last.Value;
                }

                var newSolts = SoltMap.Copy(slots);
                groundSlotsHolder.ResetSoltMap();
                groundSlotsHolder.SetSoltMap(newSolts);

                _undoList.AddLast(slots);
                _redoList.RemoveLast();

            }
            else
            {
                Debug.Log("Redo list is empty.");
            }

        }

        public bool CanPerformUndo()
        {
            return _undoList.Count != 0;
        }

        public bool CanPerformRedo()
        {
            return _redoList.Count != 0;
        }



        [System.Serializable]
        public class Settings
        {
            public int maxItems;
        }

    }
}