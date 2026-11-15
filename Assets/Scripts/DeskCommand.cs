using UnityEngine;

//mission terminal
public class DeskCommand : Interactable
{
    [SerializeField] private MissionTerminal terminal;

    public override void Interact()
    {
        if (terminal != null)
        {
            terminal.Open();
        }
    }
}
