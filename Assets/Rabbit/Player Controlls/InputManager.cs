using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Playercontrolls playerInput;
    private Playercontrolls.PlayerActions playerControlls;
    private Playercontrolls.MenuActions menuControlls;

    private player_cam_controller camController;
    // Start is called before the first frame update

    private void Awake()
    {
        playerInput = new Playercontrolls();
        playerControlls = playerInput.Player;

        camController = GetComponent<player_cam_controller>();

        playerControlls.Up.performed += ctx => camController.UpAndDown(1f);
        playerControlls.Down.performed += ctx => camController.UpAndDown(-1f);

        playerControlls.Up.canceled += ctx => camController.UpAndDownDone();
        playerControlls.Down.canceled += ctx => camController.UpAndDownDone();
        
    }
    private void FixedUpdate()
    {
        camController.moveController(playerControlls.Move.ReadValue<Vector2>());

    }

    private void LateUpdate()
    {
        camController.lookArround(playerControlls.Mouse.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        playerControlls.Enable();
    }

    private void OnDisable()
    {
        playerControlls.Disable();
    }
}
