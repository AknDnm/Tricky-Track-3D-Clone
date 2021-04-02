using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target = null;

    private Vector3 offset;
    private void Start()
    {
        offset = transform.position - target.position;
    }

    // Update fonksiyonlar�n�n s�ras� her frame de�i�ip mini stuttering sorununa neden olmamas� i�in LateUpdate() kullan�ld�.
    private void LateUpdate()
    {   
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, 0, target.position.z) + offset, Time.deltaTime * 3);
    }
}
