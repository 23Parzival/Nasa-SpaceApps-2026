using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string objectName = "Object";

    public virtual void Interact()
    {
        Debug.Log(objectName + ": interacted.");
    }
}