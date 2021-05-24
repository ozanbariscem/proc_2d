using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class BoxSelectionScreen : MonoBehaviour
{
    public Canvas canvas;
    public string canvasName = "Canvas";
    RectTransform rect;
    Image img;
    Vector3 startPosition;
    Vector3 currentPosition;

    public Color boxColor;

    /// <summary>
    /// Relative to canvas width 1 is 100%
    /// </summary>
    [Range(0f, 1f)] 
    public float dragStartDistance = 0.001f;
    float DragStartDistance
    {
        get
        {
            return Screen.width * dragStartDistance;
        }
    }

    void Start()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        img.color = boxColor;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            currentPosition = Input.mousePosition;
            if (Vector3.Distance(startPosition, currentPosition) > DragStartDistance)
            {
                DrawBox();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OffscreenBox();
        }
    }

    void DrawBox()
    {
        float width = ((currentPosition.x - DragStartDistance) - startPosition.x);
        float height = ((currentPosition.y - (DragStartDistance / canvas.scaleFactor)) - startPosition.y);

        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(height / canvas.scaleFactor));
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Abs(width / canvas.scaleFactor));
        rect.position = startPosition + new Vector3(width / 2, height / 2, 0);
    }

    void OffscreenBox()
    {
        rect.anchoredPosition = new Vector2(-10000, -10000);
    }
}
