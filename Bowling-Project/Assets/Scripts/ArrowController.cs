using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private Vector3 _initialScale = new Vector3(1,1,1);
    
    public void ChangeArrowScale(float newZScale)
    {
        // Calculate the new scale factor.
        Vector3 localScale = transform.localScale;
        
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
