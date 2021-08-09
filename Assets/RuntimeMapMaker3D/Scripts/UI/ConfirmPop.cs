using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RMM3D
{
    public class ConfirmPop : UIPopBase, IPoolable<IMemoryPool>
    {

        [Inject]
        public void Construct(SaveMapSystem saveMapSystem, UIManager uIManager)
        {
            this.saveMapSystem = saveMapSystem;
            this.uIManager = uIManager;
        }

        private SaveMapSystem saveMapSystem;
        private UIManager uIManager;
        [SerializeField] private Button confirmBtn;
        [SerializeField] private Button cancelBtn;

        IMemoryPool _pool;

        private void Awake()
        {
            confirmBtn.onClick.AddListener(() =>
            {
                saveMapSystem.ResetMap();
                _pool.Despawn(this);
            }
            );
            cancelBtn.onClick.AddListener(() =>
                _pool.Despawn(this)
            );

            var rt = this.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.SetParent(uIManager.CanvasTrans, false);
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(IMemoryPool pool)
        {

            _pool = pool;
        }

        public class Factory : PlaceholderFactory<ConfirmPop>
        {

        }
    }
}