using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{    
    [NonSerialized] public static CameraController Instance;

    public Canvas canvas;
    private Camera cam;
    
    private float mapSize, maxSize;

    private Vector3 minLimit, maxLimit;

    [Header("Speed Settings")]
    public float lerpSpeed = 5;
    public float keyboardZoomSpeed = 3, keyboardMoveSpeed = 5;
    public float mouseZoomSpeed = 5, mouseMoveSpeed = 2, borderMovementSpeed = 5;

    Vector3 newPos;
    float newSize;

    Vector3 mouseMovementStartPosition, mouseMovementCurrentPosition;

    [Header("Border Movement")]
    public bool useBorderMovement = true;
    float borderMovementEffectRange = 0.01f;
    float BorderMovementThickness {
        get { return Screen.height * borderMovementEffectRange; }
    }
    
    [Header("Camera Near")]
    public bool useCameraNearMultiplier;
    public AnimationCurve camNearCurve;
    float CamNearMultiplier { get { return useCameraNearMultiplier ? camNearCurve.Evaluate(newSize/maxSize) : 1; } }
    
    private bool acceptInput = true;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        if (acceptInput)
        {
            MouseInput();
            KeyboardInput();
            ClampSize();
            ClampPosition();
            CalculateLimits();
        }
    }

    public void FixedUpdate()
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newSize, lerpSpeed * Time.fixedDeltaTime);
        cam.transform.position = Vector3.Lerp(cam.transform.position, newPos, lerpSpeed * Time.fixedDeltaTime);
    }

    public void MouseInput()
    {
        if (Input.mouseScrollDelta.y != 0)
            SizeInput(-Input.mouseScrollDelta.y * mouseZoomSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown(2))
            mouseMovementStartPosition = Input.mousePosition;

        if (Input.GetMouseButton(2))
        {
            mouseMovementCurrentPosition = Input.mousePosition;
            Vector2 direction = mouseMovementCurrentPosition - mouseMovementStartPosition;
            mouseMovementStartPosition = mouseMovementCurrentPosition;
            PositionInput(-direction * mouseMoveSpeed * Time.deltaTime);
        }

        if (useBorderMovement)
        {
            if (Input.mousePosition.y >= Screen.height - BorderMovementThickness)
                PositionInput(Vector2.up * borderMovementSpeed * Time.deltaTime);
            if (Input.mousePosition.y <= BorderMovementThickness)
                PositionInput(-Vector2.up * borderMovementSpeed * Time.deltaTime);
            if (Input.mousePosition.x >= Screen.width - BorderMovementThickness)
                PositionInput(Vector2.right * borderMovementSpeed * Time.deltaTime);
            if (Input.mousePosition.x <= BorderMovementThickness)
                PositionInput(-Vector2.right * borderMovementSpeed * Time.deltaTime);
        }
    }

    public void KeyboardInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            PositionInput(Vector2.up * keyboardMoveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            PositionInput(-Vector2.up * keyboardMoveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            PositionInput(Vector2.right * keyboardMoveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            PositionInput(-Vector2.right * keyboardMoveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.KeypadPlus))
        {
            SizeInput(-keyboardZoomSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.KeypadMinus))
        {
            SizeInput(keyboardZoomSpeed * Time.deltaTime);
        }
    }

    public void PositionInput(Vector3 input)
    {
        newPos += (input * CamNearMultiplier);
    }

    public void SizeInput(float input)
    {
        newSize += (input * CamNearMultiplier);
    }

    public void AcceptInput(bool accept)
    {
        acceptInput = accept;
    }

    public void ClampSize()
    {
        newSize = Mathf.Clamp(newSize, 1, maxSize);
    }

    public void ClampPosition()
    {
        newPos = new Vector3(
            Mathf.Clamp(newPos.x, minLimit.x, maxLimit.x),
            Mathf.Clamp(newPos.y, minLimit.y, maxLimit.y),
            Mathf.Clamp(newPos.z, minLimit.z, maxLimit.z)
        );
    }

    public void CalculateLimits()
    {
        minLimit = new Vector3(newSize, newSize, -10);
        maxLimit = new Vector3(maxSize * 2 - newSize, maxSize * 2 - newSize, -10);

        mouseMoveSpeed = newSize * 2;
    }

    public void SetCamera(float width, float height)
    {
        cam = Camera.main;
        mapSize = width;
        maxSize = height / 2f;

        cam.orthographicSize = maxSize;
        cam.transform.position = new Vector3(maxSize, maxSize, -10); ;
        newSize = cam.orthographicSize;
        newPos = cam.transform.position;
        CalculateLimits();

        keyboardZoomSpeed *= maxSize/10f; 
        keyboardMoveSpeed *= maxSize/10f;

        mouseZoomSpeed *= maxSize;
        mouseMoveSpeed = maxSize * 2;

        borderMovementSpeed *= maxSize/10f;
    }
}
