using UnityEngine;

public class Door : Interactable
{
    public Color openColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private SpriteRenderer sr;
    private Color closedColor;
    private Collider2D[] solidColliders;
    private int playersInside = 0;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        closedColor = sr.color;

        var list = new System.Collections.Generic.List<Collider2D>();
        foreach (var c in GetComponents<Collider2D>())
        {
            if (!c.isTrigger) list.Add(c);
        }
        solidColliders = list.ToArray();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playersInside++;
        SetOpen(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playersInside--;
        if (playersInside <= 0)
        {
            playersInside = 0;
            SetOpen(false);
        }
    }

    void SetOpen(bool open)
    {
        sr.color = open ? openColor : closedColor;
        foreach (var c in solidColliders) c.enabled = !open;
        Debug.Log(objectName + ": " + (open ? "opened." : "closed."));
    }

    // Pressing E does nothing for doors now
    public override void Interact() { }
}