using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;
        Vector3 midpoint = (player1.position + player2.position) / 2f;
        Vector3 targetPos = midpoint + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}