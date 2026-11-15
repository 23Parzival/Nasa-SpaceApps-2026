using UnityEngine;
using UnityEngine.InputSystem;

//goes on the "Back to ship" button in the designer scene.
//the button's OnClick calls ReturnToShip, and Escape does the same.
public class DesignerBackButton : MonoBehaviour
{
    public void ReturnToShip()
    {
        SceneFlow.ReturnToShip();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ReturnToShip();
        }
    }
}
