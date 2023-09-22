using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidBody;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private Transform ballStartPosition;
    [SerializeField] private Transform ballFinishPosition;
    [SerializeField] private ArrowController arrow;
    
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float startThrowForce;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rollingBallAudioClip;

    private BallControlState _ballControlState;

    private Vector3 _newRotation;
    private Vector3 _newPosition;
    private const float MinRotationDegrees = -90f;
    private const float MaxRotationDegrees = 90f;

    private bool notifiedReachedToFinishLine = false;
    
    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        // Continue to next ball control state
        if (Input.GetKeyDown(KeyCode.Space) && _ballControlState != BallControlState.InMotion)
        {
            if (_ballControlState == BallControlState.ThrowForce)
            {
                ThrowBall();
            }
            
            // Calculate next ball control state
            Debug.Log("Last control state: " + _ballControlState);
            
            int nextBallState = ((int)_ballControlState + 1) % Enum.GetValues(typeof(BallControlState)).Length;
            _ballControlState = (BallControlState)nextBallState;
            
            Debug.Log("Current control state: " + _ballControlState);
        }

        switch (_ballControlState)
        {
            case BallControlState.Position:
            {
                float horizontalInput = Input.GetAxis("Horizontal");
                float horizontalMovementDelta = horizontalInput * movementSpeed * Time.deltaTime;
                _newPosition += ballTransform.TransformDirection(Vector3.right) * horizontalMovementDelta;
                break;
            }
            case BallControlState.Rotation:
            {
                float rotationInput = Input.GetAxis("Horizontal");
                float newYRotation = Mathf.Clamp(_newRotation.y + (rotationInput * rotationSpeed * Time.deltaTime),
                    MinRotationDegrees, MaxRotationDegrees);
                _newRotation = Vector3.up * newYRotation;
                ballTransform.rotation = Quaternion.Euler(_newRotation);
                break;
            }
            case BallControlState.ThrowForce:
            {
                float verticalInput = Input.GetAxis("Vertical");
                arrow.ChangeArrowScale(verticalInput);
                break;
            }
        }
        
        // If ball reached to the end of the lane
        if (transform.position.z >= ballFinishPosition.position.z && !notifiedReachedToFinishLine)
        {
            notifiedReachedToFinishLine = true;
            GameEvents.Instance.TriggerBallReachedFinishEvent();
        }
        
        // Reset for debug
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }
    
    void FixedUpdate()
    {
        if (_ballControlState == BallControlState.Position)
        {
            ballRigidBody.MovePosition(_newPosition);
        }
    }

    private void ThrowBall()
    {
        arrow.gameObject.SetActive(false);
        ballRigidBody.isKinematic = false;
        
        // Calculate the throw force based on the arrow's local scale on the Z-axis.
        float throwForce = arrow.GetArrowScaleOnZAxis() * startThrowForce;
        Vector3 throwForceVector = arrow.transform.forward * throwForce;
        Debug.Log("throwForceVector: " + throwForceVector);

        ballRigidBody.AddForce(throwForceVector, ForceMode.Impulse);
        
        // Play sound of rolling ball   
        audioSource.PlayOneShot(rollingBallAudioClip);
    }

    public void Reset()
    {
        ballRigidBody.isKinematic = true;
        _ballControlState = BallControlState.Position;

        // Reset ball position and rotation
        ballTransform.position = ballStartPosition.position;
        ballTransform.rotation = ballStartPosition.rotation;
        _newPosition = ballTransform.position;
        _newRotation = ballTransform.rotation.eulerAngles;
        
        // Reset arrow
        arrow.gameObject.SetActive(true);
        arrow.ResetArrowScale();
    }

    public void StopSound()
    {
        audioSource.Stop();
    }
}


