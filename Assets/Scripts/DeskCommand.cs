using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialPopup : MonoBehaviour
{
     void Start()
    {
        GameState.uiOpen = true;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            GameState.uiOpen = false;
            gameObject.SetActive(false);
        }
    }
}