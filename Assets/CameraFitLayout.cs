using UnityEngine;

/// <summary>
/// Fits the Components / Preview panels (and the square inside Preview) to the
/// orthographic camera's view, whatever the Game view aspect ratio is.
/// Assumes the built-in 1x1 unit Square sprite, so scale == size in world units.
/// Also keeps the two UI labels (overlay Canvas) sitting inside their panels.
/// </summary>
[ExecuteAlways]
public class CameraFitLayout : MonoBehaviour
{
    [Header("Scene objects")]
    public Camera cam;                       // leave empty to use Camera.main
    public Transform componentsPanel;        // "Components Section"
    public Transform previewPanel;           // "Preview Section"
    public Transform previewOuterSquare;     // "Square"     (border color)
    public Transform previewInnerSquare;     // "Square (1)" (fill color)

    [Header("Labels (RectTransforms on the overlay Canvas)")]
    public RectTransform componentsLabel;    // "ComponentsLbl"
    public RectTransform previewLabel;       // "PreviewLbl"
    public float labelInsetPixels = 12f;     // distance below the panel's top edge

    [Header("Layout (world units)")]
    [Range(0.2f, 0.5f)] public float componentsWidthShare = 0.35f;
    public float margin = 0.3f;              // space between panels and screen edge
    public float gap = 0.2f;                 // space between the two panels
    [Range(0.3f, 0.9f)] public float squareFill = 0.65f;  // square size vs. preview panel
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

        float sq = Mathf.Min(prevW, panelH) * squareFill;
        SetRect(previewOuterSquare, prevX, panelY, sq, sq);
        SetRect(previewInnerSquare, prevX, panelY, sq - 2f * squareBorder, sq - 2f * squareBorder);

        PlaceLabel(componentsLabel, compX, top);
        PlaceLabel(previewLabel, prevX, top);
    }

    static void SetRect(Transform t, float x, float y, float w, float h)
    {
        if (t == null) return;
        t.position = new Vector3(x, y, t.position.z);
        t.localScale = new Vector3(w, h, t.localScale.z);
    }

    void PlaceLabel(RectTransform label, float worldX, float worldTop)
    {
        if (label == null) return;
        Vector3 screen = cam.WorldToScreenPoint(new Vector3(worldX, worldTop, 0f));
        label.position = new Vector3(
            screen.x,
            screen.y - labelInsetPixels - label.rect.height / 2f,
            0f);
    }
}
