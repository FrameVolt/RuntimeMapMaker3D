// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using System;
using UnityEngine;
using Zenject;

namespace RMM3D
{
    [CreateAssetMenu(menuName = "GameSettingsInstaller")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        public GameSettings gameSettings;

        public override void InstallBindings()
        {
            Container.BindInstance<SettingsInstaller.GameSettings>(gameSettings).IfNotBound();
            Container.BindInstance(gameSettings.groundGrid).IfNotBound();
            Container.BindInstance(gameSettings.freeFlyCamSetting).IfNotBound();
            Container.BindInstance(gameSettings.confirmPopPrefab).IfNotBound();
            Container.BindInstance(gameSettings.UndoRedoSystemSettings).IfNotBound();
            Container.BindInstance(gameSettings.groundGridSystemSettings).IfNotBound();
            Container.BindInstance(gameSettings.obstacleCreatorData).IfNotBound();

        }


        [Serializable]
        public class GameSettings
        {
            public GroundGrid groundGrid;
            public ObstacleModel[] obstacleDatas;
            public GameObject obstacleBtnPrototype;
            public FreeFlyCamSetting freeFlyCamSetting;
            public GameObject confirmPopPrefab;
            public MemoryPoolSettings defaultPoolSettings;
            public UndoRedoSystem.Settings UndoRedoSystemSettings;
            public GroundGridSystem.Settings groundGridSystemSettings;
            public ObstacleCreatorData obstacleCreatorData;
        }

    }
}