using UnityEngine;

public class Door : Interactable
{
    public Color openColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private bool isOpen = false;
    private SpriteRenderer sr;
    private Color closedColor;
    private Collider2D[] solidColliders;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        closedColor = sr.color;

        // Collect every collider on this door that is NOT a trigger
        var all = GetComponents<Collider2D>();
        var list = new System.Collections.Generic.List<Collider2D>();
        foreach (var c in all)
        {
            if (!c.isTrigger) list.Add(c);
        }
        solidColliders = list.ToArray();
    }

    public override void Interact()
    {
        isOpen = !isOpen;

        sr.color = isOpen ? openColor : closedColor;
        foreach (var c in solidColliders) c.enabled = !isOpen;

        Debug.Log(objectName + ": " + (isOpen ? "opened" : "closed"));
    }
}