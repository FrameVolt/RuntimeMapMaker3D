// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using Zenject;

namespace RMM3D
{
    public class GameSignalsInstaller : Installer<GameSignalsInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<ChangeCameraStateSignal>();
        }
    }
}