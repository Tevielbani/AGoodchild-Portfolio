using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public GameObject player;

    public float yaw = 0.0f;
    public float pitch = 0.0f;

    // Update is called once per frame
    void Update()
    {
        BodyTurn bodyTurn = player.GetComponent<BodyTurn>();
        
        //get input for camera from mouse
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        //prevent camera from going too high or too low
        if (pitch >= 90)
        {
            pitch = 90;
        }
        else if (pitch <= -90)
        {
            pitch = -90;
        }
        //reset camera position relative to player
        if (Input.GetKeyUp(KeyCode.Mouse2))
        {
            yaw = bodyTurn.bodyYaw;
        }
    }
}