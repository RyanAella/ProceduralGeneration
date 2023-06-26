/*
* Copyright (c) mmq
*/

using UnityEngine;

namespace _Scripts.Cameras.FirstPerson
{
    public class FirstPersonController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float walkingSpeed = 6f;
        [SerializeField] private float runningSpeed = 12f;

        [SerializeField] private float gravityValue = -9.81f;

        [SerializeField] private GameObject groundCheck;
        [SerializeField] private int groundDistance;
        [SerializeField] private LayerMask groundMask;
        
        private CharacterController _controller;
        private Vector3 _playerVelocity;

        private bool _isGrounded;

        private float _currentSpeed;
        private Vector3 _direction;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _controller = gameObject.GetComponent<CharacterController>();
        }

        private void Start()
        {
            _currentSpeed = walkingSpeed * 5f;
            _direction = Vector3.zero;
        }

        private void Update()
        {
            // Check the player grounded state
            // _isGrounded = _controller.isGrounded;
            _isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundMask);
        }

        /// <summary>
        /// Moving around.
        /// </summary>
        /// <param name="input">A Vector2 from the Input System</param>
        public void Move(Vector2 input)
        {
            _direction.x = input.x;
            _direction.y = gravityValue;
            _direction.z = input.y;

            _controller.Move(transform.TransformDirection(_direction) * (_currentSpeed * Time.deltaTime));

            // If the player is grounded
            if (_isGrounded && _playerVelocity.y < 0) _playerVelocity.y = gravityValue * Time.deltaTime;
            
            _playerVelocity.y += gravityValue * Time.deltaTime;
            _controller.Move(_playerVelocity * Time.deltaTime);
        }

        /// <summary>
        /// Changing the current speed while sprinting.
        /// </summary>
        /// <param name="isSprinting">true while sprinting</param>
        public void Sprint(bool isSprinting)
        {
            if (isSprinting)
                _currentSpeed = runningSpeed * 5;
            else
                _currentSpeed = walkingSpeed * 5;
        }

        #endregion
    }
}