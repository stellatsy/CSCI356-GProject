using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    public TMP_Text label;
    Shooting shooting;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p != null) shooting = p.GetComponentInChildren<Shooting>(true);
    }

    void Update()
    {
        if (shooting == null || shooting.Current == null) { label.text = ""; return; }
        string reserve = shooting.Current.unlimitedReserve ? "∞" : shooting.Reserve.ToString();
        label.text = shooting.Reloading ? "Reloading..." : $"{shooting.CurrentAmmo} / {reserve}";
    }
}