using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickupFeed : MonoBehaviour
{
    static PickupFeed instance;
    public float showTime = 3f;
    public int maxLines = 4;

    TextMeshProUGUI label;
    readonly List<(string text, float expire)> lines = new List<(string text, float expire)>();

    void Awake() { instance = this; label = GetComponent<TextMeshProUGUI>(); }
    void OnDestroy() { if (instance == this) instance = null; }

    public static void Show(string msg) { if (instance != null) instance.Add(msg); }

    void Add(string msg)
    {
        lines.Add((msg, Time.unscaledTime + showTime));
        if (lines.Count > maxLines) lines.RemoveAt(0);
    }

    void Update()
    {
        lines.RemoveAll(l => Time.unscaledTime > l.expire);
        label.text = string.Join("\n", lines.ConvertAll(l => l.text));
    }
}