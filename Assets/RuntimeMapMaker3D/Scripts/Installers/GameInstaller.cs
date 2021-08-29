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
        SettingsInstaller.GameSettings settings;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SlotRaycastSystem>().AsSingle().NonLazy();
            Container.Bind<SlotsHolder>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ToolHandlers>().AsSingle();
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
            Container.BindInterfacesAndSelfTo<AssetBundleSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ColorBrushSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SoltMap>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SelectionSystem>().AsSingle().NonLazy();
            

            Container.Bind<MemoryPoolSettings>().FromInstance(settings.defaultPoolSettings);

            BindFactorys();

            GameSignalsInstaller.Install(Container);
            InitExecutionOrder();
        }


        private void BindFactorys()
        {
            Container.BindFactory<Vector3Int, ObstacleModel, Vector3, Color, ObstacleFacade, ObstacleFacade.Factory>().FromFactory<ObstacleFactory>();



            Container.BindFactory<ConfirmPop, ConfirmPop.Factory>()
                .FromPoolableMemoryPool<ConfirmPop, ConfirmPopPool>(x => x.WithInitialSize(1).FromComponentInNewPrefab(settings.confirmPopPrefab)).NonLazy();

            Container.BindFactory<ObstacleBtn, ObstacleBtn.Factory>().FromComponentInNewPrefab(settings.obstacleBtnPrototype);

        }

        private void InitExecutionOrder()
        {
            Container.BindExecutionOrder<PlacementHandler>(-10);
            Container.BindExecutionOrder<BoxSelectionHandler>(-10);
            Container.BindExecutionOrder<MoveHandler>(-10);
            Container.BindExecutionOrder<EraseHandler>(-10);
            Container.BindExecutionOrder<RotateHandler>(-10);
            Container.BindExecutionOrder<BrushHandler>(-10);
            Container.BindExecutionOrder<SelectionSystem>(-10);
            Container.BindExecutionOrder<RuntimeTransformHandleIntegrate>(0);
            


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