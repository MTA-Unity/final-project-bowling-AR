using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingPin : MonoBehaviour
{
    private Vector3 currentVector;
    public int pinNumber = 0;
    private Rigidbody rb;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip StrikeAudioClip;
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pinNumber = GetPinNumber();
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    int GetPinNumber() {
        string[] strlist = gameObject.name.Split("Bowling-Pin", 2);
        return int.Parse(strlist[1]);
    }

    public void Reset() {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        transform.rotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    void Update()
    {
        currentVector = transform.forward;
        
        Vector3 v3Velocity = rb.velocity; 

        float distanceFromForward = Vector3.Distance(currentVector, new Vector3(0,1,0));
        float velocityDistanceFromZero = Vector3.Distance(v3Velocity, new Vector3(0,0,0));

        if (distanceFromForward < 0.001) {
            GameEvents.Instance.TriggerPinSteadyEvent(pinNumber);
        } 
        else if (velocityDistanceFromZero > 0.2) {
            GameEvents.Instance.TriggerPinSwingingEvent(pinNumber);
        } else {
            GameEvents.Instance.TriggerPinFallenEvent(pinNumber);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag != "Floor") {
            // Play sound of colliding
            audioSource.PlayOneShot(StrikeAudioClip);
        }
    }
}
