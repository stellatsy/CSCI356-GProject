using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class HideWhenBackpackOpen : MonoBehaviour
{
    CanvasGroup group;

    void Awake() { group = GetComponent<CanvasGroup>(); }

    void Update()
    {
        bool hide = BackpackToggle.IsOpen || SettingsMenu.IsOpen;
        group.alpha = hide ? 0f : 1f;
    }
}