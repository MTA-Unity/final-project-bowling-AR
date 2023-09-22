using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingPin : MonoBehaviour
{
    private Vector3 currentVector;
    private int pinNumber;
    private Rigidbody rb;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip StrikeAudioClip;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        string[] strlist = gameObject.name.Split("Bowling-Pin", 2);
        pinNumber = int.Parse(strlist[1]);
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
        // Play sound of colliding
        audioSource.PlayOneShot(StrikeAudioClip);
    }
}
