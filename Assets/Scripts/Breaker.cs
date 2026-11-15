using UnityEngine;

public class Breaker : Interactable
{
    private bool isOn = true;

    public override void Interact()
    {
        isOn = !isOn;
        Debug.Log("Breaker: power is now " + (isOn ? "on" : "off"));
    }
}