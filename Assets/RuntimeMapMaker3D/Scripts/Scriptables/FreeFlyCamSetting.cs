using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "FreeFlyCamSetting", menuName = "Game/FreeFlyCamSetting")]
public class FreeFlyCamSetting : ScriptableObject
{
    public bool negateScrollDirection = false;
    public float movementSpeed = 30.0f;
    public float turnSpeed = 3.0f;
    public Vector3 cameraSpawn;
    public bool cursorVisible = false;
    public bool lockCursor = true;
    public float shiftSpeedMax = 5;
    public float smoothSpeed = 0.125f;
    public float turnSpeedH = 1;
    public float turnSpeedV = 1;
    public float scrollSensitivity = 50f;
    public float minCameraDistance = 5;
    public float maxCameraDistance = 40;
    public float pitch = 0.0f;
    public float yaw = 0.0f;
    public float inputH = 0.0f;
    public float inputV = 0.0f;
}
