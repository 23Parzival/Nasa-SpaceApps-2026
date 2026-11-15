using UnityEngine;

public class CubeSatPickup : Interactable
{
    private bool pickedUp = false;

    public override void Interact()
    {
        if (pickedUp)
        {
            Debug.Log("CubeSat Pickup: already picked up");
            return;
        }
        pickedUp = true;
        Debug.Log("CubeSat Pickup: cubeSat collected");
    }
}