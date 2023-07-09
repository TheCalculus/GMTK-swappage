using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float speedmoveMultiplier;

    [SerializeField]
    private float[] cameraSizes;
    private int currentSizeIndex;

    private void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector2 movement =
            new Vector2(inputX, inputY).normalized
            * movementSpeed
            * (Input.GetKey(KeyCode.LeftShift) ? speedmoveMultiplier : 1)
            * Time.deltaTime;
        transform.Translate(movement, Space.World);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ChangeCameraSize();
        }
    }

    private void ChangeCameraSize()
    {
        currentSizeIndex = (currentSizeIndex + 1) % cameraSizes.Length;
        Camera.main.orthographicSize = cameraSizes[currentSizeIndex];
    }
}
