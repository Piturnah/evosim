using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    float panSpeed = 5;
    [SerializeField]float zoomSpeed = 200;

    private void Update()
    {
        Move();
    }
    void Move()
    {
        //Control the zoom level of the camera via orthographic size
        float zoom = Input.GetAxisRaw("Mouse ScrollWheel") * -1;
        float zoomAmount = Camera.main.orthographicSize + zoom * zoomSpeed * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(zoomAmount, 1, 20);

        //Control pan of camera via transform position
        Vector2 moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        transform.Translate(moveDirection * panSpeed * Camera.main.orthographicSize * Time.deltaTime);

        if (Mathf.Abs(transform.position.x) > 60 + Mathf.Abs(transform.localScale.x))
        {
            transform.position = new Vector3(transform.position.x * -1, transform.position.y, -10);
        }
        if (Mathf.Abs(transform.position.y) > 60 + Mathf.Abs(transform.localScale.y))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y * -1, -10);
        }
    }
}
