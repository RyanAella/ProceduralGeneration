using UnityEngine;

namespace _Scripts.ml_agents.Agents
{
    public class Movement : MonoBehaviour
    {
        [Header("Configuration")] 
        private Animator _animator;
        public GameObject head;
        [Range(0, 1)] public float walkSpeed;
        [Range(-1, 1)] public float rotation;
        
        [Space(3)] 
        [Range(-9.81f, -30)] public float gravity;
        [Range(0.1f, 1)] public float groundDistance = 0.4f;
        public Transform groundCheck;
        public LayerMask groundLayer;

        [Space(3)] 
        [Range(-1, 1)] public float headRotationX;
        [Range(-1, 1)] public float headRotationY;
        public Transform pointToLookAt;
        private float _pointToLookAtYOffset;

        [Space(10)] 
        [Header("Stamina")] 
        public float staminaReductionMovement = 1.66f;
        private float _staminaReductionHeadRotation;
        private float _staminaReductionRotation;

        [Space(10)] 
        [Header("Multiplier")] 
        [Range(1, 100)]
        public float rotationMultiplier = 180;

        [Range(0.1f, 100)] public float walkSpeedMultiplier = 1;
        [Space(5)] [Range(1, 100)] public float headRotationMultiplier = 1;
        [Space(3)] [Range(0.01f, 1)] public float maxRotationHeadX = 0.01f;
        [Range(0.01f, 1)] public float maxRotationHeadY = 0.01f;
        [Range(0.1f, 2)] public float staminaReductionMultiplier = 1f;

        private Vector3 _velocity;
        private bool _isGrounded;
        private CustomAgent _agent;

        private CharacterController _controller;

        private static readonly int Speed = Animator.StringToHash("speed");

        void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();

            _controller = gameObject.GetComponent<CharacterController>();
            _pointToLookAtYOffset = pointToLookAt.localPosition.y;

            _staminaReductionRotation = staminaReductionMovement / 2;
            _staminaReductionHeadRotation = _staminaReductionRotation * 1.5f;
            _controller.enableOverlapRecovery = false;
        }

        void Awake()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        void Update()
        {
            HandleHeadRotation();
            HandleMovementAndRotation();

            //updating animation after AI input
            _animator.SetFloat(Speed, walkSpeed);
            AddGravity();
        }

        private void AddGravity()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2;
            }

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void HandleMovementAndRotation()
        {
            Vector3 move = transform.forward * walkSpeed;
            _controller.Move(move * (walkSpeedMultiplier * Time.deltaTime));
            _agent.ReduceStamina(walkSpeed * staminaReductionMovement * Time.deltaTime);

            transform.Rotate(Vector3.up * (rotation * 10 * rotationMultiplier * Time.deltaTime));
            _agent.ReduceStamina(Mathf.Abs(rotation) * _staminaReductionRotation * Time.deltaTime);
        }

        private void HandleHeadRotation()
        {
            float moveY = headRotationY * headRotationMultiplier * Time.deltaTime;

            if (CheckInBoundRotationY(moveY))
            {
                pointToLookAt.Translate(0, moveY, 0);
                _agent.ReduceStamina(Mathf.Abs(moveY) * _staminaReductionHeadRotation * Time.deltaTime);
            }

            float moveX = headRotationX * headRotationMultiplier * Time.deltaTime;

            if (CheckInBoundRotationX(moveX))
            {
                pointToLookAt.Translate(moveX, 0, 0);
                _agent.ReduceStamina(Mathf.Abs(moveX) * _staminaReductionHeadRotation * Time.deltaTime);
            }

            head.transform.LookAt(pointToLookAt);
        }

        private bool CheckInBoundRotationY(float moveY)
        {
            float futureY = moveY + pointToLookAt.localPosition.y;
            return futureY >= (-maxRotationHeadY + _pointToLookAtYOffset) &&
                   futureY <= (maxRotationHeadY + _pointToLookAtYOffset);
        }

        private bool CheckInBoundRotationX(float moveX)
        {
            float futureX = moveX + pointToLookAt.localPosition.x;

            return futureX >= -maxRotationHeadX && futureX <= maxRotationHeadX;
        }
    }
}