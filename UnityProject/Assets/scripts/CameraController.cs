using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertmin, lockVertmax;
    [SerializeField] bool invertY;
    [SerializeField] ParticleSystem runningParticles;

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
        if(Input.GetButton("Sprint") && !gameManager.instance.isPaused)
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, 70f, 5f * Time.deltaTime);
            runningParticles.Play();
        }
        else
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, 50f, 5f * Time.deltaTime);
            runningParticles.Stop();
        }

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