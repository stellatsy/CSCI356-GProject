using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRaycastDebug : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.F1)) return;
        var data = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        Debug.Log("UI under mouse: " +
            (results.Count == 0 ? "NOTHING" : string.Join("  >  ", results.ConvertAll(r => r.gameObject.name))));
    }
}