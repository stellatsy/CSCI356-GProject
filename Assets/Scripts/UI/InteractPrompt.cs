using TMPro;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    static InteractPrompt instance;
    TMP_Text label;
    Object owner;

    void Awake()
    {
        instance = this;
        label = GetComponent<TMP_Text>();
        label.text = "";
    }

    public static void Show(Object who, string text)
    {
        if (instance == null) return;
        instance.owner = who;
        instance.label.text = text;
    }

    public static void Hide(Object who)
    {
        if (instance == null || instance.owner != who) return;
        instance.owner = null;
        instance.label.text = "";
    }
}