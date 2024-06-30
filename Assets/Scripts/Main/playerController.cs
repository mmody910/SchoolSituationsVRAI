
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] Transform resetTransform;
    [SerializeField] GameObject player;
    [SerializeField] Camera playerHead;
    private void Start()
    {
        Invoke("resetPosition", 0.1f);
        //resetPosition();
    }
    public void resetPosition()
    {
        var rotationAngleY = resetTransform.rotation.eulerAngles.y - playerHead.transform.rotation.eulerAngles.y;
        player.transform.Rotate(0,rotationAngleY,0);
        var distanceDiff = resetTransform.position - playerHead.transform.position;
        player.transform.position += distanceDiff;
    }
}
