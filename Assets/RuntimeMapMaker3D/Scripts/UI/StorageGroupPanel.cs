﻿using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

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


    void Start()
    {

        backBtn.onClick.AddListener(() => { SceneManager.LoadScene(0); boxSelectionSystem.ClearSelections(); });
        saveBtn.onClick.AddListener(() => { saveMapSystem.SaveMap(); boxSelectionSystem.ClearSelections(); });
        loadBtn.onClick.AddListener(() => { saveMapSystem.LoadMap(); boxSelectionSystem.ClearSelections(); });
        resetBtn.onClick.AddListener(() => {
            uIManager.Pop(PopType.ConfirmPop);
            boxSelectionSystem.ClearSelections();
        });

        //var undoLongPress =
        //    undoBtn.OnPointerDownAsObservable()
        //    .SelectMany(_ => Observable.Interval(TimeSpan.FromSeconds(0.2f)).StartWith(0))
        //    .TakeUntil(undoBtn.OnPointerUpAsObservable())
        //    .RepeatUntilDestroy(undoBtn);

        //undoLongPress.Subscribe((x) => { undoRedoSystem.Undo(); boxSelectionSystem.ClearSelections(); });

        //var redoLongPress =
        //  redoBtn.OnPointerDownAsObservable()
        //  .SelectMany(_ => Observable.Interval(TimeSpan.FromSeconds(0.2f)).StartWith(0))
        //  .TakeUntil(redoBtn.OnPointerUpAsObservable())
        //  .RepeatUntilDestroy(redoBtn);

        //redoLongPress.Subscribe((x) => { undoRedoSystem.Redo(); boxSelectionSystem.ClearSelections(); });

        undoBtn.onClick.AddListener(()=> { undoRedoSystem.Undo(); boxSelectionSystem.ClearSelections(); });
        redoBtn.onClick.AddListener(()=> { undoRedoSystem.Redo(); boxSelectionSystem.ClearSelections(); });
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
