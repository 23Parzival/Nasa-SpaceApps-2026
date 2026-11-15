using UnityEngine;

/// <summary>
/// Draws a blueprint-style grid (thin minor lines plus thicker major lines)
/// exactly over a target square, such as "InnerSquare".
/// The grid is a procedurally generated 1x1 unit sprite, so it scales with the
/// square and needs no image assets. Put this on an empty GameObject.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class BlueprintGrid : MonoBehaviour
{
    [Header("Target")]
    public SpriteRenderer target;            // drag "InnerSquare" here

    [Header("Grid")]
    [Min(2)] public int divisions = 12;      // cells per side
    [Min(1)] public int majorEvery = 4;      // every Nth line is a major line
    [Min(8)] public int cellPixels = 64;     // texture resolution per cell
    [Min(1)] public int minorWidthPixels = 2;
    [Min(1)] public int majorWidthPixels = 4;

    [Header("Colors")]
    public Color minorColor = new Color(0.55f, 0.75f, 1f, 0.22f);
    public Color majorColor = new Color(0.65f, 0.82f, 1f, 0.50f);

    SpriteRenderer sr;
    Texture2D tex;
    Sprite sprite;
    bool dirty = true;

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        dirty = true;
    }

    void OnDisable()
    {
        if (sr != null) sr.sprite = null;
        Free();
    }

    void OnValidate()
    {
        dirty = true;
    }

    void LateUpdate()
    {
        if (target == null) return;
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        if (dirty || sprite == null)
        {
            Build();
            dirty = false;
        }

        // Follow the target exactly. LateUpdate runs after the layout script's Update.
        Transform tt = target.transform;
        transform.position = new Vector3(tt.position.x, tt.position.y, tt.position.z);
        transform.localScale = tt.localScale;

        sr.sharedMaterial = target.sharedMaterial;
        sr.sortingLayerID = target.sortingLayerID;
        sr.sortingOrder = target.sortingOrder + 1;   // just above the fill
    }

    void Build()
    {
        Free();

        int size = divisions * cellPixels;
        var px = new Color32[size * size];   // starts fully transparent

        Color32 minor = minorColor;
        Color32 major = majorColor;

        // Interior lines only; the outer border is already drawn by the outline square.
        for (int i = 1; i < divisions; i++)
        {
            bool isMajor = i % majorEvery == 0;
            int w = isMajor ? majorWidthPixels : minorWidthPixels;
            Color32 col = isMajor ? major : minor;
            int center = i * cellPixels;
            int start = center - w / 2;

            for (int d = 0; d < w; d++)
            {
                int line = start + d;
                if (line < 0 || line >= size) continue;

                for (int t = 0; t < size; t++)
                {
                    px[line * size + t] = col;   // horizontal line
                    px[t * size + line] = col;   // vertical line
                }
            }
        }

        tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            hideFlags = HideFlags.HideAndDontSave
        };
        tex.SetPixels32(px);
        tex.Apply();

        // pixelsPerUnit == size makes the sprite exactly 1x1 world unit,
        // so scaling it to the square's scale fits it perfectly.
        sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        sprite.hideFlags = HideFlags.HideAndDontSave;
        sr.sprite = sprite;
    }

    void Free()
    {
        DestroyObj(sprite);
        DestroyObj(tex);
        sprite = null;
        tex = null;
    }

    static void DestroyObj(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o);
        else DestroyImmediate(o);
    }
}
