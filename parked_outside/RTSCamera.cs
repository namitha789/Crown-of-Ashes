 using UnityEngine;
   using Cinemachine;

   public class RTSCamera : MonoBehaviour
   {
       [SerializeField] private CinemachineVirtualCamera _virtualCamera;
       [SerializeField] private float _moveSpeed = 50f;
       [SerializeField] private float _rotationSpeed = 100f;
       [SerializeField] private float _zoomSpeed = 10f;
       [SerializeField] private float _minHeight = 10f;
       [SerializeField] private float _maxHeight = 50f;
       [SerializeField] private Vector2 _cameraBounds = new Vector2(100f, 100f);
       
       private Transform _cameraTransform;
       
       private void Awake()
       {
           _cameraTransform = _virtualCamera.transform;
       }
       
       private void Update()
       {
           HandleMovement();
           HandleRotation();
           HandleZoom();
           ClampPosition();
       }
       
       private void HandleMovement()
       {
           Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
           Vector3 moveDirection = _cameraTransform.forward * inputDirection.z + _cameraTransform.right * inputDirection.x;
           moveDirection.y = 0;
           
           transform.position += moveDirection * _moveSpeed * Time.deltaTime;
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