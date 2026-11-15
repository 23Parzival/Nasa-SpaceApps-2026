using UnityEngine;

public class WaterSystem : Interactable
{
    private bool isRunning = false;

    public override void Interact()
    {
        isRunning = !isRunning;
        Debug.Log("Water System: pumps " + (isRunning ? "running" : "stopped"));
    }
}