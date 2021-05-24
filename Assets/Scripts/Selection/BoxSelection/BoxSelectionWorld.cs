using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(AudioSource))]
public class BoxSelectionWorld : MonoBehaviour
{
    public TextMeshProUGUI sizeText;
    public AudioClip selectionSound;
    public Color boxColor = new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f);
    [Range(0, 255)]
    public byte dragStartDistance = 2;

    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    Camera cam;

    Vector3 startPosition;
    Vector3 lastSelectedPosition;
    Vector3 currentPosition;

    /// <summary>
    /// > tile selection sets the pitch to max level of 3
    /// </summary>
    int maxSelectionSizeForPitch = 1000;
    float SelectionSize
    {
        get
        {
            return Mathf.Abs((Width) * (Height));
        }
    }
    float Width
    {
        get
        {
            return currentPosition.x - startPosition.x;
        }
    }
    float Height
    {
        get
        {
            return currentPosition.y - startPosition.y;
        }
    }
    float AbsWidth { get { return Mathf.Abs(Width); } }
    float AbsHeight { get { return Mathf.Abs(Height); } }


    void Start()
    {
        cam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = boxColor;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = selectionSound;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            currentPosition = cam.ScreenToWorldPoint(Input.mousePosition);

            if (AbsWidth >= dragStartDistance || AbsHeight >= dragStartDistance) // Vector3.Distance(startPosition, currentPosition) >= dragStartDistance)
            {
                if (currentPosition.x - startPosition.x >= 0)
                {
                    startPosition.x = Mathf.FloorToInt(startPosition.x);
                    currentPosition.x = Mathf.CeilToInt(currentPosition.x);
                }
                else
                {
                    startPosition.x = Mathf.CeilToInt(startPosition.x);
                    currentPosition.x = Mathf.FloorToInt(currentPosition.x);
                }
                if (currentPosition.y - startPosition.y >= 0)
                {
                    startPosition.y = Mathf.FloorToInt(startPosition.y);
                    currentPosition.y = Mathf.CeilToInt(currentPosition.y);
                }
                else
                {
                    startPosition.y = Mathf.CeilToInt(startPosition.y);
                    currentPosition.y = Mathf.FloorToInt(currentPosition.y);
                }
                DrawBox();

                if (lastSelectedPosition != currentPosition)
                {
                    lastSelectedPosition = currentPosition;
                    PlaySelectionSound();
                }
            } else {
                OffscreenBox();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OffscreenBox();
        }
    }

    void DrawBox()
    {
        transform.localScale = new Vector2(AbsWidth, AbsHeight);
        transform.position = startPosition + new Vector3(Width / 2, Height / 2, -startPosition.z);

        UpdateSizeTextPosition(transform.position);
        UpdateSizeText();
    }

    void OffscreenBox()
    {
        transform.position = new Vector3(-10000, -10000, 0);
        UpdateSizeTextPosition(transform.position);
    }

    void PlaySelectionSound()
    {
        if (!audioSource.isPlaying)
        {
            SetPitchRelativeToSelectionSize(SelectionSize);
            audioSource.Play();
        }
    }

    void SetPitchRelativeToSelectionSize(float size)
    {
        float relative = Mathf.Min(size / (float)maxSelectionSizeForPitch, 1f);
        audioSource.pitch = 2 + relative;
    }

    void UpdateSizeText()
    {
        sizeText.text = $"{AbsWidth}x{AbsHeight}";
        sizeText.rectTransform.sizeDelta = new Vector2(0.5f + AbsWidth / 2, 0.5f + AbsHeight / 2);
    }

    void UpdateSizeTextPosition(Vector3 position)
    {
        sizeText.transform.position = position;
    }
}
