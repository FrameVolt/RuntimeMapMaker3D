using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using TMPro;

namespace RMM3D
{
    public class StorageGroupPanel : UIPanelBase
    {
        [Inject]
        public void Construct(
           SaveMapSystem saveMapSystem,
           UIManager uIManager,
           UndoRedoSystem undoRedoSystem,
           BoxSelectionSystem boxSelectionSystem
           )
        {
            this.saveMapSystem = saveMapSystem;
            this.uIManager = uIManager;
            this.undoRedoSystem = undoRedoSystem;
            this.boxSelectionSystem = boxSelectionSystem;
        }

        private SaveMapSystem saveMapSystem;
        private UIManager uIManager;
        private UndoRedoSystem undoRedoSystem;
        private BoxSelectionSystem boxSelectionSystem;

        [SerializeField] private Button backBtn;
        [SerializeField] private Button saveBtn;
        [SerializeField] private Button loadBtn;
        [SerializeField] private Button resetBtn;
        [SerializeField] private Button undoBtn;
        [SerializeField] private Button redoBtn;

        [SerializeField] private TMP_Text undoCount;
        [SerializeField] private TMP_Text redoCount;


        void Start()
        {

            backBtn.onClick.AddListener(() => { SceneManager.LoadScene(0); boxSelectionSystem.ClearSelections(); });
            saveBtn.onClick.AddListener(() => { saveMapSystem.SaveMap(); boxSelectionSystem.ClearSelections(); });
            loadBtn.onClick.AddListener(() => { saveMapSystem.LoadMap(); boxSelectionSystem.ClearSelections(); });
            resetBtn.onClick.AddListener(() =>
            {
                uIManager.Pop(PopType.ConfirmPop);
                boxSelectionSystem.ClearSelections();
            });

            undoBtn.onClick.AddListener(() =>
            {
                if (undoRedoSystem.GetUndoVisualCount() <= 0)
                    return;

                undoRedoSystem.Undo();
                boxSelectionSystem.ClearSelections();
                undoCount.text = undoRedoSystem.GetUndoVisualCount().ToString();
                redoCount.text = undoRedoSystem.GetRedoVisualCount().ToString();
            });
            redoBtn.onClick.AddListener(() =>
            {
                if (undoRedoSystem.GetRedoVisualCount() <= 0)
                    return;

                undoRedoSystem.Redo();
                boxSelectionSystem.ClearSelections();
                undoCount.text = undoRedoSystem.GetUndoVisualCount().ToString();
                redoCount.text = undoRedoSystem.GetRedoVisualCount().ToString();
            });

            undoCount.text = undoRedoSystem.GetUndoVisualCount().ToString();
            redoCount.text = undoRedoSystem.GetRedoVisualCount().ToString();

            undoRedoSystem.OnAppend += () =>
            {
                undoCount.text = undoRedoSystem.GetUndoVisualCount().ToString();
                redoCount.text = undoRedoSystem.GetRedoVisualCount().ToString();
            };

        }


        protected override void OnShow()
        {
            base.OnShow();
        }

        protected override void OnHide()
        {
            base.OnHide();
        }

    }
}