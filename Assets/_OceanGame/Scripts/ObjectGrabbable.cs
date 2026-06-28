using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    // Respawn state
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    private void Start()
    {
        // Capture initial position on Start as well, in case position was adjusted after Awake (e.g. by instantiation offsets)
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        if (objectRigidbody != null)
        {
            objectRigidbody.useGravity = false;
        }
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        if (objectRigidbody != null)
        {
            objectRigidbody.useGravity = true;
        }
    }

    private void Update()
    {
        // If object falls off the boat/dock/beach into the water (Y < -5.0f), respawn it
        if (transform.position.y < -5.0f)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (objectGrabPointTransform != null)
        {
            Drop();
        }

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        if (objectRigidbody != null)
        {
            objectRigidbody.linearVelocity = Vector3.zero;
            objectRigidbody.angularVelocity = Vector3.zero;
            objectRigidbody.isKinematic = false;
        }
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null && objectRigidbody != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);
        }
    }
}