/*
* Copyright (c) mmq
*/

using UnityEngine;

namespace _Scripts.Cameras.FlyCam
{
    public class FlyCamInputManager : MonoBehaviour
    {
        #region Variables
        
        private FlyCam _cam;
        private InputMaster.FlyCamActions _flyCamActions;

        private GameManager _gameManager;
        private InputMaster _inputMaster;

        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            _inputMaster = new InputMaster();
            _flyCamActions = _inputMaster.FlyCam;

            _cam = GetComponent<Cameras.FlyCam.FlyCam>();
            _gameManager = GameManager.Instance;

            _flyCamActions.Up.performed += ctx => _cam.UpAndDown(1f);
            _flyCamActions.Up.canceled += ctx => _cam.UpAndDownDone();

            _flyCamActions.Down.performed += ctx => _cam.UpAndDown(-1f);
            _flyCamActions.Down.canceled += ctx => _cam.UpAndDownDone();
        }

        private void FixedUpdate()
        {
            _cam.Move(_flyCamActions.Move.ReadValue<Vector2>());
        }

        private void LateUpdate()
        {
            _cam.LookAround(_flyCamActions.Mouse.ReadValue<Vector2>());
        }

        private void OnEnable()
        {
            _flyCamActions.Enable();
        }

        private void OnDisable()
        {
            _flyCamActions.Disable();
        }
        
        #endregion
    }
}