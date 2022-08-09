using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCamera : MonoBehaviour
{
    Transform target;
    Rigidbody rb;
    float startY;

    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        List<GameObject> list = new List<GameObject>();
        scene.GetRootGameObjects(list);
        foreach (GameObject targetObject in list)
            if (targetObject.tag == "Player")
            {
                target = targetObject.transform;
                rb = targetObject.GetComponent<Rigidbody>();
            }
        if (target == null)
        {
            Debug.LogWarning("Игрок на сцене не найден");
            this.enabled = false;
        }
        startY = transform.position.y;
    }

    void Update()
    {
        Vector3 position = transform.position;
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.y = target.rotation.eulerAngles.y;
        position.x = target.position.x;
        position.y = startY + rb.velocity.magnitude * 3.6f;
        position.z = target.position.z;
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
