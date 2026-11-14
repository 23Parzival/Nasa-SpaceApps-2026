using UnityEngine;

/// <summary>
/// Fits the Components / Preview panels (and the square inside Preview) to the
/// orthographic camera's view, whatever the Game view aspect ratio is.
/// The right part of the Preview panel is reserved for a column of stat labels.
/// Assumes the built-in 1x1 unit Square sprite, so scale == size in world units.
/// </summary>
[ExecuteAlways]
public class CameraFitLayout : MonoBehaviour
{
    [Header("Scene objects")]
    public Camera cam;                       // leave empty to use Camera.main
    public Transform componentsPanel;        // "Components Section"
    public Transform previewPanel;           // "Preview Section"
    public Transform previewOuterSquare;     // "OuterSquare" (border color)
    public Transform previewInnerSquare;     // "InnerSquare" (fill color)

    [Header("Panel labels (RectTransforms on the overlay Canvas)")]
    public RectTransform componentsLabel;    // "ComponentsLbl"
    public RectTransform previewLabel;       // "PreviewLbl"
    public float labelInsetPixels = 12f;     // distance below the panel's top edge

    [Header("Stats labels (stacked top to bottom, right side of Preview)")]
    public RectTransform[] statsLabels;      // mass, volume, energy produced, energy consumed
    [Range(0f, 0.45f)] public float statsColumnShare = 0.28f; // share of Preview width
    public float statsRowHeightPixels = 36f;
    public float statsRowSpacingPixels = 10f;
    public float statsColumnPaddingPixels = 10f;

    [Header("Layout (world units)")]
    [Range(0.2f, 0.5f)] public float componentsWidthShare = 0.35f;
    public float margin = 0.3f;              // space between panels and screen edge
    public float gap = 0.2f;                 // space between the two panels
    [Range(0.3f, 0.9f)] public float squareFill = 0.65f;  // square size vs. its area
    public float squareBorder = 0.1f;        // outline thickness

    void Update()
    {
        Layout();
    }

    void Layout()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null || !cam.orthographic) return;

        float viewH = cam.orthographicSize * 2f;
        float viewW = viewH * cam.aspect;
        Vector3 c = cam.transform.position;

        float left   = c.x - viewW / 2f + margin;
        float right  = c.x + viewW / 2f - margin;
        float top    = c.y + viewH / 2f - margin;
        float bottom = c.y - viewH / 2f + margin;

        float usableW = (right - left) - gap;
        float compW = usableW * componentsWidthShare;
        float prevW = usableW - compW;
        float panelH = top - bottom;
        float panelY = (top + bottom) / 2f;

        float compX = left + compW / 2f;
        float prevX = left + compW + gap + prevW / 2f;

        SetRect(componentsPanel, compX, panelY, compW, panelH);
        SetRect(previewPanel, prevX, panelY, prevW, panelH);

        // The square lives in the left part of Preview; the stats column takes the right part.
        bool hasStats = statsLabels != null && statsLabels.Length > 0;
        float prevLeft = prevX - prevW / 2f;
        float statsW = hasStats ? prevW * statsColumnShare : 0f;
        float squareAreaW = prevW - statsW;
        float squareX = prevLeft + squareAreaW / 2f;

        float sq = Mathf.Min(squareAreaW, panelH) * squareFill;
        SetRect(previewOuterSquare, squareX, panelY, sq, sq);
        SetRect(previewInnerSquare, squareX, panelY, sq - 2f * squareBorder, sq - 2f * squareBorder);

        PlaceTopLabel(componentsLabel, compX, top);
        PlaceTopLabel(previewLabel, prevX, top);

        if (hasStats)
            PlaceStats(prevLeft + squareAreaW, prevLeft + prevW, panelY);
    }

    static void SetRect(Transform t, float x, float y, float w, float h)
    {
        if (t == null) return;
        t.position = new Vector3(x, y, t.position.z);
        t.localScale = new Vector3(w, h, t.localScale.z);
    }

    static float CanvasScale(RectTransform r)
    {
        var canvas = r.GetComponentInParent<Canvas>();
        return canvas != null ? Mathf.Max(0.01f, canvas.scaleFactor) : 1f;
    }

    void PlaceTopLabel(RectTransform label, float worldX, float worldTop)
    {
        if (label == null) return;
        float s = CanvasScale(label);
        Vector3 screen = cam.WorldToScreenPoint(new Vector3(worldX, worldTop, 0f));
        label.position = new Vector3(
            screen.x,
            screen.y - labelInsetPixels - label.rect.height * s / 2f,
            0f);
    }

    void PlaceStats(float worldLeft, float worldRight, float worldCenterY)
    {
        Vector3 sl = cam.WorldToScreenPoint(new Vector3(worldLeft, worldCenterY, 0f));
        Vector3 sr = cam.WorldToScreenPoint(new Vector3(worldRight, worldCenterY, 0f));

        float widthPx = Mathf.Max(10f, (sr.x - sl.x) - 2f * statsColumnPaddingPixels);
        float centerX = (sl.x + sr.x) / 2f;

        int n = statsLabels.Length;
        float totalH = n * statsRowHeightPixels + (n - 1) * statsRowSpacingPixels;
        float topY = sl.y + totalH / 2f;

        for (int i = 0; i < n; i++)
        {
            var label = statsLabels[i];
            if (label == null) continue;

            float s = CanvasScale(label);
            label.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widthPx / s);
            label.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, statsRowHeightPixels / s);

            float y = topY - statsRowHeightPixels / 2f - i * (statsRowHeightPixels + statsRowSpacingPixels);
            label.position = new Vector3(centerX, y, 0f);
        }
    }
}
