// PlayerDeath.cs
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    void Start() { GetComponent<Health>().OnDied += Die; }

    void Die()
    {
        GetComponent<FPSInput>().enabled = false;
        GetComponentInChildren<MouseLook>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Player died"); // the Game Over panel comes later
    }
}