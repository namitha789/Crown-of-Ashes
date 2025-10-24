using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 50f;
    [SerializeField] private float _edgeScrollSize = 20f;
    [SerializeField] private bool _useEdgeScrolling = true;
    
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 100f;
    
    [Header("Zoom Settings")]
    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private float _minHeight = 10f;
    [SerializeField] private float _maxHeight = 50f;
    
    [Header("Camera Bounds")]
    [SerializeField] private Vector2 _cameraBounds = new Vector2(50f, 50f);
    
    private Transform _cameraTransform;
    private Camera _mainCamera;
    
    private void Awake()
    {
        // Remove camera initialization from here
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Make sure your camera has the 'MainCamera' tag.");
        }
    }
    
    private void Update()
    {
        HandleKeyboardMovement();
        if (_useEdgeScrolling)
        {
            HandleEdgeScrolling();
        }
        HandleRotation();
        HandleZoom();
        ClampPosition();
    }
    
    private void HandleKeyboardMovement()
    {
        Vector3 inputDirection = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            inputDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            inputDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            inputDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            inputDirection += Vector3.right;
        
        Vector3 moveDirection = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        moveDirection.y = 0;
        
        transform.position += moveDirection.normalized * _moveSpeed * Time.deltaTime;
    }
    
    private void HandleEdgeScrolling()
    {
        Vector3 inputDirection = Vector3.zero;
        
        if (Input.mousePosition.x < _edgeScrollSize)
            inputDirection += Vector3.left;
        if (Input.mousePosition.x > Screen.width - _edgeScrollSize)
            inputDirection += Vector3.right;
        if (Input.mousePosition.y < _edgeScrollSize)
            inputDirection += Vector3.back;
        if (Input.mousePosition.y > Screen.height - _edgeScrollSize)
            inputDirection += Vector3.forward;
        
        Vector3 moveDirection = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        moveDirection.y = 0;
        
        transform.position += moveDirection.normalized * _moveSpeed * Time.deltaTime;
    }
    
    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, -_rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        }
    }
    
    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            Vector3 pos = _cameraTransform.position;
            pos.y = Mathf.Clamp(pos.y - scrollInput * _zoomSpeed, _minHeight, _maxHeight);
            _cameraTransform.position = pos;
        }
    }
    
    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -_cameraBounds.x, _cameraBounds.x);
        pos.z = Mathf.Clamp(pos.z, -_cameraBounds.y, _cameraBounds.y);
        transform.position = pos;
    }
}