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
    [SerializeField] private float arrowScaleSpeed;
    [SerializeField] private float startThrowForce;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rollingBallAudioClip;

    private BallControlState _ballControlState;

    private Vector3 _newRotation;
    private Vector3 _newPosition;
    private const float MinRotationDegrees = -90f;
    private const float MaxRotationDegrees = 90f;
    private const float MinArrowScale = 1.0f; // Minimum scale along the Z-axis
    private const float MaxArrowScale = 3.0f; // Maximum scale along the Z-axis
    private bool notifiedReachedToFinishLine = false;
    private bool notifiedStoppedMoving = false;
    private bool startedMoving = false;
    
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
            int nextBallState = ((int)_ballControlState + 1) % Enum.GetValues(typeof(BallControlState)).Length;
            _ballControlState = (BallControlState)nextBallState;
            
            Debug.Log("Current control state: " + _ballControlState);
        }

        switch (_ballControlState)
        {
            case BallControlState.Position:
            {
                movePositionAnimation();
                break;
            }
            case BallControlState.Rotation:
            {
                moveRotationAnimation();
                break;
            }
            case BallControlState.ThrowForce:
            {
                moveForceAnimation();
                break;
            }
        }
        
        // If ball reached to the end of the lane
        if (transform.position.z >= ballFinishPosition.position.z && !notifiedReachedToFinishLine)
        {
            notifiedReachedToFinishLine = true;
            GameEvents.Instance.TriggerBallReachedFinishEvent();
        }

        if (_ballControlState == BallControlState.InMotion && ballRigidBody.velocity.magnitude > 0) {
            startedMoving = true;
        }

        if ((ballRigidBody.velocity.magnitude == 0 && startedMoving && !notifiedStoppedMoving)
         || (ballTransform.position.y < 0 && startedMoving && !notifiedStoppedMoving)) {
            startedMoving = false;
            notifiedStoppedMoving = true;
            GameEvents.Instance.TriggerBallReachedFinishEvent();
        }
        
        // Reset for debug
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    private void movePositionAnimation() {
        // Move the ball left and right within the screen boundaries.
        _newPosition.x += movementSpeed * Time.deltaTime;            

        // Check if the ball reaches the floor edges and reverse direction.
        if (_newPosition.x > 4)
        {
            _newPosition.x = 4;
            movementSpeed *= -1;
        } else if (_newPosition.x < -4) {
            _newPosition.x = -4;
            movementSpeed *= -1;
        } 
    }

    private void moveRotationAnimation() {
        // Rotate the ball left and right within the screen boundaries.
        float newYRotation = _newRotation.y + (rotationSpeed * Time.deltaTime);        

        // Check if the ball reaches the angle edges and reverse direction.
        if (newYRotation > MaxRotationDegrees)
        {
            newYRotation = MaxRotationDegrees;
            rotationSpeed *= -1;
        } else if (newYRotation < MinRotationDegrees) {
            newYRotation = MinRotationDegrees;
            rotationSpeed *= -1;
        } 
        _newRotation = Vector3.up * newYRotation;
        ballTransform.rotation = Quaternion.Euler(_newRotation);
    }

    private void moveForceAnimation() {
        float newZScale = arrow.GetArrowScaleOnZAxis() + (arrowScaleSpeed * Time.deltaTime); 

        if (newZScale > MaxArrowScale) {
            newZScale = MaxArrowScale;
            arrowScaleSpeed *= -1;
        } else if (newZScale < MinArrowScale) {
            newZScale = MinArrowScale;
            arrowScaleSpeed *= -1;
        }

        arrow.ChangeArrowScale(newZScale);   
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
        startedMoving = false;
        notifiedStoppedMoving = false;
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


