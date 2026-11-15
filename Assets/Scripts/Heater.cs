using UnityEngine;

public class Heater : Interactable
{
    private bool isOn = false;

    public override void Interact()
    {
        isOn = !isOn;
        Debug.Log("Heater: " + (isOn ? "on" : "off"));
    }
}