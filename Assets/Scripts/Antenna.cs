using UnityEngine;

public class Antenna : Interactable
{
    public override void Interact()
    {
        Debug.Log("Antenna: transmitting signal");
    }
}