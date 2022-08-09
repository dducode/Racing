using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraMove : MonoBehaviour
{
    public Transform target;
    Camera _camera;
    [Range(1, 30)]
    public float sensitivity;
    public float startSensitive;
    private float rotY;
    private float startY;
    private Vector3 offset;
    Quaternion tempRotation;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        rotY = transform.eulerAngles.y;
        startY = rotY;
        offset = target.position - transform.position;
        tempRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    void OnEnable() => BroadcastMessages.AddListener(Messages.RELOAD_TRACK, ToStartPosition);
    void OnDisable() => BroadcastMessages.RemoveListener(Messages.RELOAD_TRACK, ToStartPosition);

    void ToStartPosition() => rotY = startY;

    void Update()
    {
        if (_camera.enabled == true)
        {
            rotY += Mathf.Abs(Input.GetAxis("Mouse X")) > startSensitive ? Input.GetAxis("Mouse X") * sensitivity : 0;
            Quaternion targetRotation = Quaternion.Euler(0, rotY, 0);
            Quaternion rotation = Quaternion.Slerp(tempRotation, targetRotation, Time.deltaTime * 15f);
            transform.position = target.position - (rotation * offset);
            transform.LookAt(target);
            tempRotation = rotation;
        }
    }
}
