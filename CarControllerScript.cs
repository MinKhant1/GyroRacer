using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarControllerScript : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;


    [SerializeField] private ArduinoInput arduinoInput;
    [SerializeField] private bool isAccelarting;
    [SerializeField] private bool isDeAccelarting;
    [SerializeField] private float gyroX;

    [SerializeField] float sensitivity = 3000f;  
   [SerializeField] float deadzone = 100f;

    [SerializeField] ParticleSystem leftParticle;
    [SerializeField] ParticleSystem rightParticle;


    private float currentSteeringInput = 0f; 
        // Ignores micro movement
    public float returnSpeed = 2f;          
    public float steerSmoothness = 5f;


    //OdorMeter
    private Rigidbody rb;
    private float lastVelocity;
    private float acceleration;

    private float odorLevel = 0f;
    public float odorIncreaseRate = 5f; 
    public float odorDecayRate = 1f;

    public float currentSpeed;

    [SerializeField] TextMeshProUGUI ordorText;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastVelocity = rb.linearVelocity.magnitude;


    }


    private void FixedUpdate()
    {

        isAccelarting = arduinoInput.IsAccelerating();
        isDeAccelarting = arduinoInput.IsDeAccelerating();



        

        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();



        float currentVelocity = rb.velocity.magnitude;
        acceleration = (currentVelocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = currentVelocity;

        UpdateOdorMeter();


    }

  
    private float accelerationDuration = 0.2f; 
    private float currentAccelTime = 0f;

    private void GetInput()
    {
        gyroX = arduinoInput.GetSteeringInput();
        horizontalInput = gyroX;

        // Handle acceleration
        if (isAccelarting)
        {
            if (currentAccelTime <= 0f) 
            {
                verticalInput = 1;
                leftParticle.Play();
                rightParticle.Play();
            }
            currentAccelTime = accelerationDuration; // Reset timer
        }
        else if (currentAccelTime > 0f) // If still running, reduce timer
        {
            currentAccelTime -= Time.deltaTime;
        }
        else
        {
            verticalInput = 0f; // Stop the particle system if the timer runs out
            leftParticle.Stop();
            rightParticle.Stop();

        }

        // Handle deceleration
        if (isDeAccelarting)
        {
            if (currentAccelTime <= 0f)
            {
                verticalInput = -1;
             
            }
            currentAccelTime = accelerationDuration; // Reset timer
        }
        else if (currentAccelTime > 0f)
        {
            currentAccelTime -= Time.deltaTime;
        }
        else
        {
            verticalInput = 0f;
            leftParticle.Stop();
            rightParticle.Stop();
        }

      
        //Debug.Log("verticalInput: " + verticalInput);
    }


    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        //Debug.Log("Steering Input: " + horizontalInput);

        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }


    private void UpdateOdorMeter()
    {
         currentSpeed = Mathf.RoundToInt(rb.linearVelocity.magnitude)*5;
        ordorText.text = currentSpeed.ToString();
        

        Debug.Log(currentSpeed);

      

      
    }


   


}