using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    private Camera camera;
    public float damping;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 cursor = Input.mousePosition;
        cursor = Camera.main.ScreenToWorldPoint(cursor);
        cursor.z = 0;
        Vector3 targetDirection = cursor - transform.position;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        Vector2 movement = new Vector2(inputX, inputY).normalized * movementSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        Vector3 targetCameraPosition = new Vector3(
            transform.position.x,
            transform.position.y,
            camera.transform.position.z
        );
        camera.transform.position = Vector3.Lerp(
            camera.transform.position,
            targetCameraPosition,
            damping * Time.deltaTime
        );

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
