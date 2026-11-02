using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 1.5f;
    public float minimumVert = -45f;
    public float maximumVert = 45f;

    private float verticalRotation = 0f;
    private Transform player;

    void Start()
    {
        player = transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        player.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minimumVert, maximumVert);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}