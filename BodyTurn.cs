using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTurn : MonoBehaviour
{

    public float speedH = 2.0f;
    [HideInInspector]
    public float bodyYaw = 0.0f;

    // Update is called once per frame
    void Update()
    {
        //get player rotation from mouse
        bodyYaw += speedH * Input.GetAxis("Mouse X");

        transform.eulerAngles = new Vector3(0, bodyYaw, 0);

        //freeze body rotation when holding down middle mouse button
        if (Input.GetKey(KeyCode.Mouse2))
        {
            speedH = 0;
        }
        else
        {
            speedH = 2;
        }
    }
}