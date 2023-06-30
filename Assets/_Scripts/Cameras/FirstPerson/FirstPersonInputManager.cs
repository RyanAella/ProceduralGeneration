/*
* Copyright (c) mmq
*/

using _Scripts.Cameras.FirstPerson;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Cameras.FirstPerson
{
    public class FirstPersonInputManager : MonoBehaviour
    {
        #region Variables

        private InputMaster _inputMaster;
        private InputMaster.PlayerActions _playerActions;

        private FirstPersonController _firstPersonController;
        private FirstPersonCamera _firstPersonCamera;


        private GameManager _gameManager;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _inputMaster = new InputMaster();
            _playerActions = _inputMaster.Player;

            _firstPersonController = GetComponent<FirstPersonController>();
            _firstPersonCamera = GetComponent<FirstPersonCamera>();

            _playerActions.Sprint.performed += ctx => _firstPersonController.Sprint(true);
            _playerActions.Sprint.canceled += ctx => _firstPersonController.Sprint(false);
        }

        private void Update()
        {
            _firstPersonController.Move(_playerActions.Move.ReadValue<Vector2>());
        }

        private void LateUpdate()
        {
            _firstPersonCamera.LookAround(_playerActions.Mouse.ReadValue<Vector2>());
        }

        private void OnEnable()
        {
            _playerActions.Enable();
        }

        private void OnDisable()
        {
            _playerActions.Disable();
        }

        #endregion
    }
}