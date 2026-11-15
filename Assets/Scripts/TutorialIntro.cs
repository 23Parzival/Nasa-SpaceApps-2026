using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialIntro : MonoBehaviour
{
    void Start()
    {
        GameState.uiOpen = true;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.eKey.wasPressedThisFrame || kb.escapeKey.wasPressedThisFrame)
        {
            GameState.uiOpen = false;
            gameObject.SetActive(false);
        } 

    }
}