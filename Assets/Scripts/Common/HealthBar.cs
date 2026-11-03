using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health target;      // leave empty: finds the parent's Health, or the Player's
    public Image fill;         // the red Image set to "Filled"
    public bool faceCamera = true;
    float targetFill = 1f;

    public TMP_Text label;          // optional: shows "70 / 100"
    public bool useGradient;        // optional: green -> yellow -> red
    public Gradient colorByHealth;


    void Start()
    {
        if (target == null) target = GetComponentInParent<Health>();
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.GetComponent<Health>();
        }
        if (target == null)
        {
            Debug.LogWarning("HealthBar: no Health found for " + name, this);
            enabled = false;
            return;
        }

        target.OnChanged += UpdateBar;
        target.OnDied += Hide;
        fill.fillAmount = 1f;
        UpdateBar(target.current, target.maxHealth);
    }

    void Update()
    {
        fill.fillAmount = Mathf.Lerp(fill.fillAmount, targetFill, 8f * Time.deltaTime);
        if (useGradient) fill.color = colorByHealth.Evaluate(fill.fillAmount);
    }

    void LateUpdate()
    {
        if (faceCamera && Camera.main != null)
            transform.forward = Camera.main.transform.forward;
    }

    void UpdateBar(float cur, float max)
    {
        targetFill = cur / max;
        if (label != null) label.text = Mathf.CeilToInt(cur) + " / " + Mathf.CeilToInt(max);
    }
    void Hide() { gameObject.SetActive(false); }

    void OnDestroy()
    {
        if (target == null) return;
        target.OnChanged -= UpdateBar;
        target.OnDied -= Hide;
    }
}