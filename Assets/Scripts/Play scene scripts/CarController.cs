using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(AudioListener))]
public class CarController : MonoBehaviour
{
    [SerializeField] Vector3 massCenter; 
    [SerializeField] Lights lights;
    Rigidbody rb;
    Vector3 startPos;
    Quaternion startQuat;
    GameSettings gameSettings;
    bool lightsTumbler;

    void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.buildIndex is 1)
        {
            GetComponent<CarMovement>().enabled = false;
            GetComponent<CarController>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            return;
        }
        gameSettings = GameManager.gameManager.gameSettings;
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        startPos = transform.position;
        startQuat = transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(gameSettings.lightsTumbler))
            lightsTumbler = !lightsTumbler;

        ApplyCamerasAndLightsEnabled();
    }
    void FixedUpdate()
    {
        rb.centerOfMass = massCenter;
        Vector3 moveForward = transform.InverseTransformDirection(rb.velocity);
        float scalar = Mathf.Abs(moveForward.z) / 100;
        rb.AddForce(Vector3.down * scalar, ForceMode.Acceleration);
        rb.angularDrag = scalar;
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

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "finish")
        {
            transform.position = startPos;
            transform.rotation = startQuat;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            BroadcastMessages.SendMessage(Messages.RELOAD_TRACK);
        }
    }
}
