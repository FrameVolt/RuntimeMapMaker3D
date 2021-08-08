using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FreeFlyCamSystem : ICameraSystem
{
    public FreeFlyCamSystem(FreeFlyCamSetting camSetting)
    {
        this.camSetting = camSetting;
        this.camTrans = Camera.main.transform;
    }

    private readonly FreeFlyCamSetting camSetting;

    private Transform camTrans;

    public void EnterState()
    {
        
        camSetting.pitch = camTrans.localEulerAngles.x;
        camSetting.yaw = camTrans.localEulerAngles.y;
    }

    public void ExitState()
    {
        
    }

    public void UpdateState()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            var scroll = Input.mouseScrollDelta.y;
            var adjustedScroll = scroll * camSetting.scrollSensitivity;
            camTrans.position += Time.deltaTime * adjustedScroll * camTrans.forward;
        }


        if (!Input.GetMouseButton(1))
        {
            return;
        }

        camSetting.yaw += camSetting.turnSpeedH * Input.GetAxisRaw("Mouse X");
        camSetting.pitch -= camSetting.turnSpeedV * Input.GetAxisRaw("Mouse Y");


        var shiftSpeed = Input.GetKey(KeyCode.LeftShift) ? camSetting.shiftSpeedMax : 1;


        camTrans.eulerAngles = new Vector3(camSetting.pitch, camSetting.yaw, 0.0f);


        camSetting.inputH = Input.GetAxisRaw("Horizontal");
        camSetting.inputV = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (camTrans.forward * camSetting.inputV + camSetting.inputH * camTrans.right).normalized;

        if (Input.GetKey(KeyCode.Q))
        {
            moveDirection.y = -1.0f;
        }

        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space))
        {
            moveDirection.y = 1.0f;
        }


        Vector3 desiredPosition = camTrans.position + moveDirection * shiftSpeed * camSetting.movementSpeed * Time.deltaTime;
        float clampedY = Mathf.Clamp(desiredPosition.y, 0.1f, 100);
        desiredPosition = new Vector3(desiredPosition.x, clampedY, desiredPosition.z);

        Vector3 smoothedPosition = Vector3.Lerp(camTrans.position, desiredPosition, camSetting.smoothSpeed);
        camTrans.position = smoothedPosition;
    }
}
