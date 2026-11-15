using UnityEngine;
using UnityEngine.InputSystem;

public class FrontWindow : Interactable
{
    public GameObject popup;

    public override void Interact()
    {
        SetOpen(!popup.activeSelf);
    }

    void Update()
    {
        if (popup.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SetOpen(false);
    }

    void SetOpen(bool open)
    {
        popup.SetActive(open);
        GameState.uiOpen = open;
    }
}