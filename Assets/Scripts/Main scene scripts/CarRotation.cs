using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRotation : MonoBehaviour
{
    [SerializeField, Range(0, 360)] float rotateDegrees = 90f;
    void Update() => transform.Rotate(0, rotateDegrees * Time.deltaTime, 0);
}
