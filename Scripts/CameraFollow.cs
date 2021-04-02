using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target = null;

    private Vector3 offset;
    private void Start()
    {
        offset = transform.position - target.position;
    }

    // Update fonksiyonlarinin sirasi her frame degisip mini stuttering sorununa neden olmamasi icin LateUpdate() kullanildi.
    private void LateUpdate()
    {   
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, 0, target.position.z) + offset, Time.deltaTime * 3);
    }
}
