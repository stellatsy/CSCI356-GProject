using InventoryFramework;
using UnityEngine;

// TEST ONLY: press F8 to give yourself potions (Editor only)
public class PotionTester : MonoBehaviour
{
    public Item potion;      // drag the Potion item asset here
    public int amount = 1;   // potions per key press
    public KeyCode key = KeyCode.F8;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(key) && potion != null && WeaponProgress.Instance != null)
            for (int i = 0; i < amount; i++) WeaponProgress.Instance.Give(potion);
#endif
    }
}