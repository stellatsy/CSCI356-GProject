using UnityEngine;

public class BackpackToggle : MonoBehaviour
{
    public static bool IsOpen;
    public GameObject backpack;          // drag "Inv" here

    MonoBehaviour[] playerScripts;
    Health playerHealth;

    void Start()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerScripts = new MonoBehaviour[]
            {
                player.GetComponent<FPSInput>(),
                player.GetComponentInChildren<MouseLook>(true),
                player.GetComponentInChildren<Shooting>(true)
            };
            playerHealth = player.GetComponent<Health>();
        }
        SetOpen(false);
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.IsDead)
        {
            if (IsOpen) CloseBecauseDead();
            return;                              // no Tab while dead
        }
        if (Input.GetKeyDown(KeyCode.Tab)) SetOpen(!IsOpen);
    }

    void SetOpen(bool open)
    {
        IsOpen = open;
        backpack.SetActive(open);
        Time.timeScale = open ? 0f : 1f;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;

        if (playerScripts != null)
            foreach (var s in playerScripts)
                if (s != null) s.enabled = !open;
    }

    void CloseBecauseDead()
    {
        IsOpen = false;
        backpack.SetActive(false);
        Time.timeScale = 1f;                     // let the death sequence play
        // the player's scripts stay disabled while dead
    }

    void OnDestroy() { Time.timeScale = 1f; IsOpen = false; }
}