using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidBody;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private Transform ballStartPosition;
    [SerializeField] private Transform ballRightBorder;
    [SerializeField] private Transform ballLeftBorder;
    [SerializeField] private Transform floorPosition;
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
    private bool stopBallBtnClicked = false;
    
    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        // Input.GetKeyDown("space") // TODO - remove this line
        // Continue to next ball control state
        if (stopBallBtnClicked && _ballControlState != BallControlState.InMotion)
        {
            if (_ballControlState == BallControlState.ThrowForce)
            {
                ThrowBall();
            }
            
            // Calculate next ball control state
            int nextBallState = ((int)_ballControlState + 1) % Enum.GetValues(typeof(BallControlState)).Length;
            _ballControlState = (BallControlState)nextBallState;
            
            stopBallBtnClicked = false;
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
         || (ballTransform.position.y < floorPosition.position.y && startedMoving && !notifiedStoppedMoving)) {
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

    public void StopButtonClickedListener() {
        stopBallBtnClicked = true;
    }

    private void movePositionAnimation() {
        // Move the ball left and right within the screen boundaries.
        _newPosition.x += movementSpeed * Time.deltaTime;            

        // Check if the ball reaches the floor edges and reverse direction.
        if (_newPosition.x > ballRightBorder.position.x)
        {
            _newPosition.x = ballRightBorder.position.x;
            movementSpeed *= -1;
        } else if (_newPosition.x < ballLeftBorder.position.x) {
            _newPosition.x = ballLeftBorder.position.x;
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
        if (_ballControlState == BallControlState.Rotation)
        {
            ballRigidBody.MoveRotation(Quaternion.Euler(_newRotation));
        }
    }

    private void ThrowBall()
    {
        arrow.gameObject.SetActive(false);
        ballRigidBody.isKinematic = false;

        BowlingPin[] pins = FindObjectsOfType<BowlingPin>();

        foreach (var pin in pins)
        {       
            pin.setPinKinemticFalse();
        }
        
        // Calculate the throw force based on the arrow's local scale on the Z-axis.
        float throwForce = arrow.GetArrowScaleOnZAxis() * startThrowForce;
        Vector3 throwForceVector = arrow.transform.forward * throwForce;
        Debug.Log("throwForceVector: " + throwForceVector);

        ballRigidBody.AddForce(throwForceVector, ForceMode.Impulse);

        // Play sound of rolling ball
        if (GameUIController.Instance.IsAudioEnable())
        {
            audioSource.PlayOneShot(rollingBallAudioClip);
        }
    }

    public void Reset()
    {
        ballRigidBody.isKinematic = true;
        startedMoving = false;
        notifiedStoppedMoving = false;
        stopBallBtnClicked = false;
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


