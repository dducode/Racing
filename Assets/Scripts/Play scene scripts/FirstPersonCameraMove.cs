using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraMove : MonoBehaviour
{
    Camera _camera;
    [Range(1, 30)]
    public float sensitivity;
    Vector3 cameraAngles;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        cameraAngles = transform.localEulerAngles;
    }

    void LateUpdate()
    {
        if (_camera.enabled == true)
        {
            float rotateY = Input.GetAxis("Mouse X") * sensitivity;
            cameraAngles.y += rotateY;
            transform.localEulerAngles = cameraAngles;
            if (transform.localEulerAngles.y < 270f && transform.localEulerAngles.y > 135f)
            {
                cameraAngles.y = 270f;
                transform.localEulerAngles = cameraAngles;
            }
            else if (transform.localEulerAngles.y > 90f && transform.localEulerAngles.y < 135f)
            {
                cameraAngles.y = 90f;
                transform.localEulerAngles = cameraAngles;
            }
        }
    }
}
