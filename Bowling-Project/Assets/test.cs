using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        ballRigidBody.AddForce(0, 0, 50, ForceMode.Impulse);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
