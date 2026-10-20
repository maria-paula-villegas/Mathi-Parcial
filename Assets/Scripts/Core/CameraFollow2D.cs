using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
    [SerializeField] private float deadZoneX = 1.5f;
    [SerializeField] private bool useLimits = true;
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 35f;
    private void Start()
    {
        if (target == null)
        {
            GameObject personaje = GameObject.Find("Player");
            if (personaje != null) target = personaje.transform;
        }
    }
    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 siguiente = transform.position;
        float diferencia = target.position.x - transform.position.x;
        if (Mathf.Abs(diferencia) > deadZoneX)
            siguiente.x = target.position.x - Mathf.Sign(diferencia) * deadZoneX;
        siguiente.y = target.position.y + offset.y;
        siguiente.z = offset.z;
        if (useLimits) siguiente.x = Mathf.Clamp(siguiente.x, minX, maxX);
        transform.position = Vector3.Lerp(transform.position, siguiente, smoothSpeed * Time.deltaTime);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneX * 2, 2, 0));
        if (!useLimits) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minX, -10, 0), new Vector3(minX, 10, 0));
        Gizmos.DrawLine(new Vector3(maxX, -10, 0), new Vector3(maxX, 10, 0));
    }
}
