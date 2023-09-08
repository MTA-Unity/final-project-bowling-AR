using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidBody;
    private Transform _arrow;
    private bool _ballIsMoving;
    [SerializeField] private Transform _ballStartPosition;

}
