using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GroundGridData", menuName = "Game/GroundGridData")]
public class GroundGrid : ScriptableObject
{
    public int xAmount;
    public int zAmount;
    public int yAmount;

    public float size;
}
