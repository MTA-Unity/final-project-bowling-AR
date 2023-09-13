using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private float arrowScaleSpeed;
    
    private const float MinScale = 0.1f; // Minimum scale along the Z-axis
    private const float MaxScale = 2.0f; // Maximum scale along the Z-axis

    private Vector3 _initialScale;

    private void Start()
    {
        // Store the initial scale of the arrow.
        _initialScale = transform.localScale;
    }
    
    public void ChangeArrowScale(float verticalInput)
    {
        // Calculate the new scale factor.
        Vector3 localScale = transform.localScale;
        
        float newZScale = Mathf.Clamp(localScale.z +
                                    (verticalInput * arrowScaleSpeed * Time.deltaTime), MinScale, MaxScale); 
        var newLocalScale = new Vector3(localScale.x, localScale.y, newZScale);
        transform.localScale = newLocalScale;
    }

    public float GetArrowScaleOnZAxis()
    {
        return transform.localScale.z;
    }

    public void ResetArrowScale()
    {
        transform.localScale = _initialScale;
    }
}
