using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform[] povs;
    [SerializeField] float speed;

    private int index = 1;
    private Vector3 target;
    public Transform plane;

    private void LateUpdate()
    {
        target = povs[index].position;
        if (Input.GetKeyDown(KeyCode.Alpha1)) index = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) index = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) index = 2;

        else if (Input.GetKeyDown(KeyCode.Alpha4)) index = 3;
        if (index == 2)
        {
            transform.rotation = plane.transform.rotation;
            transform.position = target;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
            transform.forward = povs[index].forward;
        }
        
    }

}
