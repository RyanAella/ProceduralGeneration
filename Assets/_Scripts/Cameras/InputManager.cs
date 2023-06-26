/*
* Copyright (c) mmq
*/

using System;
using UnityEngine;

namespace _Scripts.Cameras
{
    public class InputManager : MonoBehaviour
    {
        #region Variables
        
        private InputMaster _inputMaster;
        private InputMaster.GeneralActions _generalActions;

        private GameManager _gameManager;
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            _inputMaster = new InputMaster();
            _generalActions = _inputMaster.General;

            _gameManager = gameObject.GetComponent<GameManager>();
            
            _generalActions.ChangeView.performed += ctx => _gameManager.ChangeView();
        }
        
        private void OnEnable()
        {
            _generalActions.Enable();
        }

        private void OnDisable()
        {
            _generalActions.Disable();
        }

        #endregion
    }
}
