using UnityEngine;

public class CameraManager : MonoBehaviour
{

    //Speed
    float arrowMouseSpeed = 2.0f;
    float translateSpeed = 3.0f;

    //Move Parameters
    float mouseX;
    float mouseY;
    Quaternion localRotation;
    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    void Start()
    {
        //local rotation
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    void Update()
    {
        MoveCamera();
        RotateCamera(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), arrowMouseSpeed);
    }

    void MoveCamera()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(translateSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(new Vector3(-translateSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, 0, -translateSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Translate(new Vector3(0, 0, translateSpeed * Time.deltaTime));
        }
    }

    void RotateCamera(float horizontal, float verticle, float moveSpeed)
    {
        mouseX = horizontal;
        mouseY = -verticle;

        rotY += mouseX * moveSpeed;
        rotX += mouseY * moveSpeed;

        localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }
}