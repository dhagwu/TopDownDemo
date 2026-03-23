using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(-7f, 12f, -7f);
    [SerializeField] private float followSmooth = 8f;

    [Header("View Settings")]
    [SerializeField] private Vector3 fixedEulerAngles = new Vector3(55f, 45f, 0f);

    private Vector3 currentVelocity;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / followSmooth
        );

        transform.rotation = Quaternion.Euler(fixedEulerAngles);
    }
}