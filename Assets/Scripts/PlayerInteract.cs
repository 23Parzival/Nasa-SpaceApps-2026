using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private Interactable current;

    void Update()
    {
        if (PauseMenu.IsPaused || GameState.minigameOpen) return;

        if (current != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            current.Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var i = other.GetComponentInParent<Interactable>();
        if (i != null)
        {
            current = i;
            Debug.Log("Press E: " + i.objectName);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var i = other.GetComponentInParent<Interactable>();
        if (i != null && i == current) current = null;
    }
}