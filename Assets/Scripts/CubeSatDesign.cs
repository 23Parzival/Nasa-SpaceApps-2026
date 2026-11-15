using UnityEngine;

public class CubeSatDesign : Interactable
{
    public override void Interact()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector2 playerPosition = player != null ? (Vector2)player.transform.position : (Vector2)transform.position;
        SceneFlow.OpenDesigner(playerPosition);
    }
}
