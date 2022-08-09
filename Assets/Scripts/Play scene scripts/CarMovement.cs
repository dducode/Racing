using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class CarMovement : MonoBehaviour
{
    public Vector3 massCenter; 
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public CarFeature carFeature;  
    public Automatic automatic;  
    public Camera thirdPersonCamera;
    public Lights lights;
    Rigidbody rb;    
    Vector3 startPos;
    Quaternion startQuat;
    bool lightsTumbler;
    public float speed { get { return rb.velocity.magnitude * 3.6f; } }
    public int engineSpeed { get; private set; }
    public int currentTransmission { get; private set; }
    public float currentEngineSpeed { get; private set; }
    bool isAutomatic = true;

    GameSettings gameSettings;

    void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.buildIndex == 1)
        {
            gameObject.GetComponent<CarMovement>().enabled = false;
            thirdPersonCamera.enabled = false;
            return;
        }

        if (GameManager.gameManager != null)
            gameSettings = GameManager.gameManager.gameSettings;
        else
        {
            GameSettings _gameSettings = new GameSettings();
            _gameSettings.braking = KeyCode.Space;
            _gameSettings.lightsTumbler = KeyCode.LeftShift;
            gameSettings = _gameSettings;
        }
        rb = GetComponent<Rigidbody>();
        currentTransmission = 1;
    }

    void Start()
    {
        startPos = transform.position;
        startQuat = transform.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "finish")
        {
            transform.position = startPos;
            transform.rotation = startQuat;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            currentTransmission = 1;
            BroadcastMessages.SendMessage(Messages.RELOAD_TRACK);
        }
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        for (int i = 0; i < collider.transform.childCount; i++)
        {
            Transform visualWheel = collider.transform.GetChild(i);

            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);

            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(gameSettings.lightsTumbler))
            lightsTumbler = !lightsTumbler;

        ApplyCamerasAndLightsEnabled();
        
        BroadcastMessages<float, int, float>.SendMessage(
            Messages.UPDATE_VIEW, speed, currentTransmission, currentEngineSpeed
            );
    }

    void ApplyCamerasAndLightsEnabled()
    {
        Vector3 move = transform.InverseTransformDirection(rb.velocity);
        bool forward = !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow);
        bool back = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        lights.backLight.SetActive(forward || Input.GetKey(gameSettings.braking));
        lights.rearSideLight.SetActive(back || Input.GetKey(gameSettings.braking));
        lights.reverseLight.SetActive(move.z < 0 && back);
        lights.frontLight.SetActive(lightsTumbler);
    }

    float MotorTorque()
    {
        float meanRPM = 0f;
        int wheelCount = 0;
        for (int i = 0; i < axleInfos.Count; i++)
        {
            if (axleInfos[i].motor)
            {
                meanRPM += (axleInfos[i].leftWheel.rpm + axleInfos[i].rightWheel.rpm);
                wheelCount += 2;
            }
        }
        meanRPM /= wheelCount;
        if (currentTransmission > 0)
            currentEngineSpeed = meanRPM * carFeature.transmission[currentTransmission - 1];
        else
            currentEngineSpeed = -meanRPM * carFeature.transmission[currentTransmission];
        currentEngineSpeed = Mathf.Clamp(currentEngineSpeed, carFeature.minEngineSpeed, carFeature.maxEngineSpeed);
        float rotationForce = 0f;
        if (currentEngineSpeed < carFeature.minEngineSpeed)
            rotationForce = Mathf.InverseLerp(0, carFeature.minEngineSpeed, currentEngineSpeed);
        else if (currentEngineSpeed > carFeature.maxEffectiveEngSpeed)
            rotationForce = 1 - Mathf.InverseLerp(carFeature.maxEffectiveEngSpeed, carFeature.maxEngineSpeed, currentEngineSpeed);
        else
            rotationForce = 1f;
        return rotationForce;
    }

    void GearChange()
    {
        if (isAutomatic)
        {
            WheelHit leftWheelHit = new WheelHit();
            WheelHit rightWheelHit = new WheelHit();
            bool isGround = false;
            for (int i = 0; i < axleInfos.Count; i++)
                if (axleInfos[i].motor)
                {
                    isGround = axleInfos[i].leftWheel.GetGroundHit(out leftWheelHit) &&
                                axleInfos[i].rightWheel.GetGroundHit(out rightWheelHit);
                    break;
                }
            if (!isGround)
                return;

            bool positiveSlip = leftWheelHit.forwardSlip < 0.1f && rightWheelHit.forwardSlip < 0.1f;
            bool negativeSlip = leftWheelHit.forwardSlip > -0.1f && rightWheelHit.forwardSlip > -0.1f;

            if (currentEngineSpeed >= automatic.maxEngineSpeed && positiveSlip)
                currentTransmission++;

            if (currentEngineSpeed <= automatic.minEngineSpeed && negativeSlip && currentTransmission > 1)
                currentTransmission--;
            
            Vector3 inverseVelocity = transform.InverseTransformDirection(rb.velocity);
            if (inverseVelocity.z < -0.1f && Input.GetKey(KeyCode.S))
                currentTransmission = 0;
            else if (currentTransmission == 0)
                currentTransmission = 1;

            currentTransmission = Mathf.Clamp(currentTransmission, 0, carFeature.transmission.Count);
        }
    }

    public void FixedUpdate()
    {
        rb.centerOfMass = massCenter;
        Vector3 moveForward = transform.InverseTransformDirection(rb.velocity);
        float scalar = Mathf.Abs(moveForward.z) / 100;
        rb.AddForce(Vector3.down * scalar, ForceMode.Acceleration);
        rb.angularDrag = scalar;

        float rotationForce = MotorTorque();
        float motor = carFeature.maxMotorTorque * Input.GetAxis("Vertical") * rotationForce;
        float steering = carFeature.maxSteeringAngle * Input.GetAxis("Horizontal");
        bool brake = Input.GetKey(gameSettings.braking);        

        float dot = Vector3.Dot(transform.forward, rb.velocity);
        dot = Mathf.Clamp(dot, 0, Mathf.Infinity);
        steering /= Mathf.Sqrt(dot / 5 + 1);

        GearChange();

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            if (axleInfo.braking)
            {
                if (brake)
                {
                    axleInfo.leftWheel.brakeTorque = carFeature.maxBrakingTorque;
                    axleInfo.rightWheel.brakeTorque = carFeature.maxBrakingTorque;
                }
                else
                {
                    axleInfo.leftWheel.brakeTorque = 0f;
                    axleInfo.rightWheel.brakeTorque = 0f;
                }
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
    public bool braking;
}

[System.Serializable]
public class CarFeature
{
    [Tooltip("Мощность двигателя")]
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    [Tooltip("Максимальный поворот руля")]
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    [Tooltip("Мощность торможения")]
    public float maxBrakingTorque;
    public float maxEffectiveEngSpeed;
    [Tooltip("Количество оборотов двигателя на холостом ходу")]
    public float minEngineSpeed;
    public float maxEngineSpeed;
    public List<float> transmission;
}

[System.Serializable]
public class Automatic
{
    public float minEngineSpeed;
    public float maxEngineSpeed;
}

[System.Serializable]
public class Lights
{
    public GameObject backLight;
    public GameObject rearSideLight;
    public GameObject frontLight;
    public GameObject reverseLight;
}
