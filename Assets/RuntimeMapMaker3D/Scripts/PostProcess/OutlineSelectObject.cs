// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using UnityEngine;

public class OutlineSelectObject : MonoBehaviour
{
    RaycastHit hit;
    Ray ray;
    public Transform obj;
    int layer;

    void Start() { }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (obj == null)
            {
                obj = hit.transform;
                this.layer = obj.gameObject.layer;
                obj.gameObject.layer = LayerMask.NameToLayer("Outline");
            }
        }
        else
        {
            if (obj != null)
            {
                obj.gameObject.layer = this.layer;
                obj = null;
            }

        }
    }



}
