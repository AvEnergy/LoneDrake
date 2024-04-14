using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertmin, lockVertmax;
    [SerializeField] bool invertY;

    float rotX;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        //Get input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        //Invert?
        if (invertY)
        {
            rotX += mouseY;
        }
        else
        {
            rotX -= mouseY;
        }

        //Clamp the rotX on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertmin, lockVertmax);

        //Rotate cam on the x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //Rotate the player on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}