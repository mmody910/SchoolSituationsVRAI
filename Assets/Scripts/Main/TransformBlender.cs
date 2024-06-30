using UnityEngine;
public class TransformBlender : MonoBehaviour
{
    public Transform transform1;
    public Transform transform2;
    void Update()
    {
        // Interpolate between the two transforms with a 50/50 ratio
        transform.position = Vector3.Lerp(transform1.position, transform2.position, 0.5f);
/*        transform.rotation = Quaternion.Slerp(transform1.rotation, transform2.rotation, 0.5f);
        transform.localScale = Vector3.Lerp(transform1.localScale, transform2.localScale, 0.5f);*/
    }
}