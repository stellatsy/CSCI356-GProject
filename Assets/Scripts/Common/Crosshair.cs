// Crosshair.cs - flashes a colour when the player hits something
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public static Crosshair Instance;
    public Image image;                       // drag the Crosshair's own Image here
    public Color normal = Color.white;
    public Color hitColor = Color.red;
    public float flashTime = 0.12f;
    float timer;

    void Awake() { Instance = this; }
    public void Hit() { timer = flashTime; }

    void Update()
    {
        if (timer > 0f) { timer -= Time.deltaTime; image.color = hitColor; }
        else image.color = normal;
    }
}