using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_cam_controller : MonoBehaviour
{
	[SerializeField]
	private Camera camera;
	[SerializeField]
	private float acceleration = 50; // movespeed
	[SerializeField]
	private float lookSensitivity = 1; // mousesensitivity
	[SerializeField]
	private float dampingCoefficient = 5; // how quickly you break to a halt after you stop your input
	[SerializeField]
	private bool focusOnEnable = true; // lock cursor
									   
	[SerializeField]
	private float flyMod = 25f; // geschwindigkeit Up & Down Movement

	private Vector3 velocity; // current velocity
	[SerializeField]
	private Vector3 keyInput = default; // keyboard Input
	private float rotation;

	static bool Focused
	{
		get => Cursor.lockState == CursorLockMode.Locked;
		set
		{
			Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = value == false;
		}
	}

	void OnEnable()
	{
		Cursor.lockState = CursorLockMode.Locked;

		if (focusOnEnable) Focused = true;
	}

	void OnDisable() => Focused = false;

    private void Update()
    {
		if (Focused) velocity += GetAccelerationVector() * Time.deltaTime;

		velocity = Vector3.Lerp(velocity, Vector3.zero, dampingCoefficient * Time.deltaTime);
		transform.position += velocity * Time.deltaTime;
	}

	//Input Mouse für Camera bewegung
	public void lookArround(Vector2 input)
    {
		float mouseX = input.x;
		float mouseY = input.y;

		//Blickwinkel Up/Down auf 90° setzten
		rotation -= (mouseY * Time.deltaTime) * lookSensitivity;
		rotation = Mathf.Clamp(rotation, -90f, 90f);

		//Camera Transfrom
		camera.transform.localRotation = Quaternion.Euler(rotation, 0, 0);

		//Blickwinkel left/right
		transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * lookSensitivity);
	}

	//Input Tastatur für Camera Bewegung
	public void moveController(Vector2 input)
    {
		keyInput.x = input.x;
		keyInput.z = input.y;
    }

    public void UpAndDown(float mod)
    {
		keyInput.y = flyMod * mod;
    }  

	public void UpAndDownDone()
    {
		keyInput.y = 0;
    }


    Vector3 GetAccelerationVector()
	{
		Vector3 dir = transform.TransformVector(keyInput.normalized);

		return dir * acceleration; ;
	}

}
