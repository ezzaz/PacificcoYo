using Ditzelgames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(WaterFloat))]
public class WaterBoat : MonoBehaviour
{
    //visible Properties
    public Transform Motor;
    public float SteerPower = 500f;
    public float Power = 5f;
    public float MaxSpeed = 10f;
    public float Drag = 0.1f;

    //used Components
    protected Rigidbody Rigidbody;
    protected Quaternion StartRotation;
    protected ParticleSystem ParticleSystem;
    protected Camera Camera;

    //internal Properties
    protected Vector3 CamVel;
    private Vector2 moveInput;
    private PlayerInput playerInput;

    // Unity Message | 0 references
    void Start()
    {

        playerInput = new PlayerInput();
    }

    public void Awake()
    {
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
        Rigidbody = GetComponent<Rigidbody>();
        StartRotation = Motor.localRotation;
        Camera = Camera.main;
    }
    private void FixedUpdate()
    {
        FishingMinigame fishing = GetComponent<FishingMinigame>();
        if (fishing != null && fishing.IsFishingActive())
        {
            moveInput = Vector2.zero;
        }
        Moverse();
    }
    public void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();
  
    }
    public void Moverse()
    {
        var forceDirection = transform.forward;
        var steer = 0;


        if (moveInput.x > 0)
            steer = 1;

        else if (moveInput.x < 0)
            steer = -1;

        Rigidbody.AddForceAtPosition(steer * transform.right * SteerPower / 100f, Motor.position);

        //compute vectors
        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);
        var targetVel = Vector3.zero;

        if (moveInput.y > 0)
        {
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody,forward * MaxSpeed,Power);
        }
        else if (moveInput.y < 0)
        {
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody,forward * -MaxSpeed,Power);
        }

        //Motor Animation // Particle system
        Motor.SetPositionAndRotation(Motor.position, transform.rotation * StartRotation * Quaternion.Euler(0, 30f * steer, 0));
        if (ParticleSystem != null)
        {
            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
                ParticleSystem.Play();
            else
                ParticleSystem.Pause();
        }

        //moving forward
        var movingForward = Vector3.Cross(transform.forward, Rigidbody.linearVelocity).y < 0;

        //move in direction
        Rigidbody.linearVelocity = Quaternion.AngleAxis(Vector3.SignedAngle(Rigidbody.linearVelocity, (movingForward ? 1f : 0f) * transform.forward, Vector3.up) * Drag, Vector3.up) * Rigidbody.linearVelocity;


    }

}