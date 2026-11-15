using UnityEngine;

public class CubeSatLauncher : Interactable
{
    public override void Interact()
    {
        //TEMPORARY: launching counts as completing the active mission until the mission sim exists.
        if (MissionManager.HasActiveMission)
        {
            Debug.Log("launched, mission complete: " + MissionManager.ActiveMission.title);
            MissionManager.CompleteActiveMission();
        }
        else
        {
            Debug.Log("no active mission, pick one at the Command Desk first");
        }
    }
}
