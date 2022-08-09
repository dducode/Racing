using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CarData", menuName = "Car Data", order = 51)]
public class CarData : ScriptableObject
{
    public enum Transmission
    {
        Full, Forward, Back
    }

    [SerializeField] string _carName;
    [SerializeField] int _maxMotorTorque;
    [SerializeField] int _maxBrakingTorque;
    [SerializeField] int _mass;
    [SerializeField] Transmission _transmission;
    [SerializeField] GameObject _car;

    public string carName { get { return _carName; } }
    public int maxMotorTorque { get { return _maxMotorTorque; } }
    public int maxBrakingTorque { get { return _maxBrakingTorque; } }
    public int mass { get { return _mass; } }
    public Transmission transmission { get { return _transmission; } }
    public GameObject car { get { return _car; } }
}
