/*
* Copyright (c) mmq
*/

using UnityEngine;

namespace _Scripts.Cameras.FirstPerson
{
    public class FirstPersonCamera : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] private float mouseSensitivity = 10f;
        
        // the first person camera
        public new GameObject camera;

        private float _xRotation;
        
        #endregion

        #region Unity Methods

        private void Start()
        {
            camera = transform.Find("fp_camera").gameObject;
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void LookAround(Vector2 input)
        {
            float mouseX = input.x * mouseSensitivity * Time.deltaTime;
            float mouseY = input.y * mouseSensitivity * Time.deltaTime;
            
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -70f, 60f);

            camera.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);

            transform.Rotate(Vector3.up * mouseX);
        }
        
        #endregion
    }
}