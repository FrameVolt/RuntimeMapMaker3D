using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraManager : IInitializable, ITickable
{

    public CameraManager(SignalBus signalBus, FollowCamSystem followCamSystem, FreeFlyCamSystem freeFlyCamSystem)
    {
        this.signalBus = signalBus;
        this.followCamSystem = followCamSystem;
        this.freeFlyCamSystem = freeFlyCamSystem;

        cameraSystems = new List<ICameraSystem>
        {
            followCamSystem,
            freeFlyCamSystem
        };
    }

    private readonly SignalBus signalBus;
    private readonly FollowCamSystem followCamSystem;
    private readonly FreeFlyCamSystem freeFlyCamSystem;

    List<ICameraSystem> cameraSystems;
    ICameraSystem currentSystem;
    CameraState currentState;


    public void Initialize()
    {
        //signalBus.Subscribe<ChangeCameraStateSignal>(OnChangeCameraState);
        


        ChangeState(CameraState.Follow);

    }


    public void ChangeState(CameraState state)
    {
        if (currentState == state)
        {
            // Already in state
            return;
        }

        currentState = state;
        signalBus.Fire<ChangeCameraStateSignal>(new ChangeCameraStateSignal() { cameraState = state });

        if (currentSystem != null)
        {
            currentSystem.ExitState();
            currentSystem = null;
        }

        currentSystem = cameraSystems[(int)state];
        currentSystem.EnterState();
    }
    public void Tick()
    {
        if(currentSystem != null)
        currentSystem.UpdateState();
    }




}


public interface ICameraSystem
{
    void EnterState();
    void ExitState();
    void UpdateState();
}

public enum CameraState
{
    FreeFly,
    Follow
}
