using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject head;
    [Range(0, 1)] public float walkSpeed;
    [Range(-1, 1)] public float rotation;

    [Space(3)]
    [Range(-1, 1)] public float headRotationX = 0;
    [Range(-1, 1)] public float headRotationY = 0;
    

    [Space(10)]
    [Header("Multiplier")]
    [Range(1, 100)] public float rotationMultiplier = 180;
    [Range(0.1f, 10)] public float walkSpeedMultiplier = 1;
    [Space(5)]
    [Range(1, 100)] public float headRotationMultiplier = 1;
    [Space(3)]
    [Range(0.1f, 0.99f)] public float maxRotationHeadX = 0.3f;
    [Range(0.1f, 0.99f)] public float maxRotationHeadY = 0.5f;



    CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = transform.forward * walkSpeed;
        controller.Move(move * walkSpeedMultiplier * Time.deltaTime);
        transform.Rotate(Vector3.up * rotation * 10 * rotationMultiplier * Time.deltaTime);

        float rotationY = headRotationY * headRotationMultiplier * 50 * Time.deltaTime;

        if (CheckInBoundRotationY(Vector3.up * rotationY))
        {
            head.transform.Rotate(Vector3.up * rotationY);
        }

        float rotationX = headRotationX * headRotationMultiplier * 50 * Time.deltaTime;

        if (CheckInBoundRotationX(Vector3.right * rotationX))
        {
            head.transform.Rotate(Vector3.right * rotationX);
        }
    }

    private bool CheckInBoundRotationY(Vector3 toCheckRotation)
    {
        Quaternion quaternion = Quaternion.Euler(toCheckRotation.x, toCheckRotation.y, toCheckRotation.z);
        Quaternion localRotation = head.transform.localRotation;
        localRotation *= quaternion;
        return (localRotation.y > -maxRotationHeadY && localRotation.y < maxRotationHeadY);
    }

    private bool CheckInBoundRotationX(Vector3 toCheckRotation)
    {
        Quaternion quaternion = Quaternion.Euler(toCheckRotation.x, toCheckRotation.y, toCheckRotation.z);
        Quaternion localRotation = head.transform.localRotation;
        localRotation *= quaternion;
        return (localRotation.x > -maxRotationHeadX && localRotation.x < maxRotationHeadX);
    }
}
