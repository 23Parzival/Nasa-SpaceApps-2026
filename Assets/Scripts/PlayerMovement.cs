using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;

    public SpriteRenderer spriteRenderer;

    public Sprite left;

    public Sprite right;

    public Sprite down;

    public Sprite up;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        //stand still while a terminal or minigame is open
        if (GameState.minigameOpen)
        {
            input = Vector2.zero;
            return;
        }

        float x = 0f, y = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)
        {
            x -= 1f;
            spriteRenderer.sprite = left;
        }
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed)
        {
            x += 1f;
            spriteRenderer.sprite = right;
        }
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)
        {
            y -= 1f;
            spriteRenderer.sprite = down;
        }
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)
        {
            y += 1f;
            spriteRenderer.sprite = up;
        }

        input = new Vector2(x, y).normalized;
    }

    // FixedUpdate runs on the physics step, which keeps wall collisions reliable
    void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}