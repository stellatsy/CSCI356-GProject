using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ControlsHint : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.H;
    CanvasGroup group;

    void Awake() { group = GetComponent<CanvasGroup>(); }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            group.alpha = group.alpha > 0.5f ? 0f : 1f;
    }
}