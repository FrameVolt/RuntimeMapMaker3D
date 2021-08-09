using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace RMM3D
{
    //// Include this just to ensure BindSignal with an object mapping works
    //public class PlayerDiedSignalObserver
    //{
    //    public void OnPlayerDied()
    //    {
    //        Debug.Log("Fired PlayerDiedSignal");
    //    }
    //}

    public class GameSignalsInstaller : Installer<GameSignalsInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<ChangeCameraStateSignal>();

            // Include these just to ensure BindSignal works
            //Container.BindSignal<EditorPlayModeSignal>().ToMethod<PlayerDiedSignalObserver>(x => x.OnPlayerDied).FromNew();
            //Container.BindSignal<EditModeSignal>().ToMethod(() => Debug.Log("Fired EnemyKilledSignal"));
        }
    }
}