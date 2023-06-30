/*
* Copyright (c) mmq
*/

using UnityEngine;

namespace _Scripts.Cameras.FlyCam
{
    public class FlyCam : MonoBehaviour
    {
        # region Variables

        [SerializeField] private new Camera camera;
        [SerializeField] private float speed = 50;
        [SerializeField] private float mouseSensitivity = 10;

        // how quickly you break to a halt after you stop your input
        [SerializeField] private float dampingCoefficient = 5;

        [SerializeField] private bool focusOnEnable = true; // lock cursor

        [SerializeField] private float upDownSpeed = 25f;
        [SerializeField] private Vector3 keyInput; // keyboard Input

        private Vector3 _velocity; // current velocity
        private float _rotation;

        #endregion
        
        #region Unity Methods

        private void Update()
        {
            if (Focused)
            {
                _velocity += transform.TransformVector(keyInput.normalized) * (speed * Time.deltaTime);
            }

            _velocity = Vector3.Lerp(_velocity, Vector3.zero, dampingCoefficient * Time.deltaTime);
            transform.position += _velocity * Time.deltaTime;
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;

            if (focusOnEnable) Focused = true;
        }

        private void OnDisable()
        {
            Focused = false;
        }
        
        private static bool Focused
        {
            get => Cursor.lockState == CursorLockMode.Locked;
            set
            {
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = value == false;
            }
        }

        /// <summary>
        /// Looking around by moving the mouse.
        /// </summary>
        /// <param name="input">A Vector2 from the Input System</param>
        public void LookAround(Vector2 input)
        {
            var mouseX = input.x * mouseSensitivity * Time.deltaTime;
            var mouseY = input.y * mouseSensitivity * Time.deltaTime;

            _rotation -= mouseY;
            _rotation = Mathf.Clamp(_rotation, -70f, 60f);

            camera.transform.localRotation = Quaternion.Euler(_rotation, 0, 0);

            transform.Rotate(Vector3.up * mouseX);
        }

        /// <summary>
        /// Moving around.
        /// </summary>
        /// <param name="input">A Vector2 from the Input System</param>
        public void Move(Vector2 input)
        {
            keyInput.x = input.x;
            keyInput.z = input.y;
        }

        /// <summary>
        /// Moving upwards or downwards.
        /// </summary>
        /// <param name="modifier">Tells the direction</param>
        public void UpAndDown(float modifier)
        {
            keyInput.y = upDownSpeed * modifier;
        }

        /// <summary>
        /// Reset the upwards / downwards movement.
        /// </summary>
        public void UpAndDownDone()
        {
            keyInput.y = 0;
        }
        
        #endregion
    }
}