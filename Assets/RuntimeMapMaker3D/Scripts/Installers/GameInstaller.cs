using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace RMM3D
{
    public class GameInstaller : MonoInstaller
    {
        [Inject]
        GameSettingsInstaller.GameSettings settings;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SlotRaycastSystem>().AsSingle().NonLazy();
            Container.Bind<SlotsHolder>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlacementSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<RotateObstacleSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SaveMapSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EraseSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FreeFlyCamSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FollowCamSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CameraManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UndoRedoSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BoxSelectionSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MoveToolSystem>().AsSingle().NonLazy();


            Container.Bind<MemoryPoolSettings>().FromInstance(settings.defaultPoolSettings);

            BindFactorys();

            GameSignalsInstaller.Install(Container);
            InitExecutionOrder();
        }


        private void BindFactorys()
        {
            Container.BindFactory<Vector3Int, ObstacleModel, ObstacleFacade, ObstacleFacade.Factory>().FromFactory<ObstacleFactory>();



            Container.BindFactory<ConfirmPop, ConfirmPop.Factory>()
                .FromPoolableMemoryPool<ConfirmPop, ConfirmPopPool>(x => x.WithInitialSize(1).FromComponentInNewPrefab(settings.confirmPopPrefab)).NonLazy();

            Container.BindFactory<ObstacleBtn, ObstacleBtn.Factory>().FromComponentInNewPrefab(settings.obstacleBtnPrototype);

        }

        private void InitExecutionOrder()
        {
            //Container.BindExecutionOrder<UIManager>(-10);
            //Container.BindExecutionOrder<ConfirmPop>(-10);
        }


        public class ConfirmPopPool : MonoPoolableMemoryPool<IMemoryPool, ConfirmPop>
        {
        }
    }

    public class ObstacleInstaller : Installer<ObstacleInstaller>
    {
        public override void InstallBindings()
        {

        }
    }
}