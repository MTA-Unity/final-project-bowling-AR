using UnityEngine;
using UnityEngine.Serialization;

public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidBody;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private Transform ballStartPosition;
    private Transform _arrow;
    
    private float _horizontalMovementSpeed = 4f;
    private bool _ballIsMoving;
    private BallControlState _ballControlState;
    
    private Vector3 _newPosition;
    private Vector3 _newRotation;
    
    private void Start()
    {
        ballTransform = ballStartPosition;
        _newPosition = ballTransform.position;
        _newRotation = ballTransform.rotation.eulerAngles;

        _ballControlState = BallControlState.Position;
    }
    
    private void Update()
    {
        // Continue to next ball control state
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_ballControlState == BallControlState.ThrowForce)
            {
                ThrowBall();
            }
            else
            {
                // Change to next ball control state
                _ballControlState = (BallControlState)((int)_ballControlState + 1);
            }
        }
        
        if (_ballControlState == BallControlState.Position)
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            var horizontalMovementDelta = horizontalInput * _horizontalMovementSpeed * Time.deltaTime;
            _newPosition = _newPosition + ballTransform.TransformDirection(Vector3.right) * horizontalMovementDelta;
        }
        

        else if (_ballControlState == BallControlState.Rotation)
        {
            
        }



        // var rotationInput = Input.GetAxis("Mouse X");
        // var rotationDelta = rotationInput * _rotationSpeed;
        // _newRotation += Vector3.up * rotationDelta;
    }

    private void ThrowBall()
    {
        throw new System.NotImplementedException();
    }

    void FixedUpdate()
    {
        ballRigidBody.MovePosition(_newPosition);
       // ballRigidBody.MoveRotation(Quaternion.Euler(_newRotation));
    }

}

public enum BallControlState
{
    Position,
    Rotation,
    ThrowForce
}
