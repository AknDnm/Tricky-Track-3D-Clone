using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target = null;

    private Vector3 offset;
    private void Start()
    {
        offset = transform.position - target.position;
    }

    // Update fonksiyonlarýnýn sýrasý her frame deðiþip mini stuttering sorununa neden olmamasý için LateUpdate() kullanýldý.
    private void LateUpdate()
    {   
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, 0, target.position.z) + offset, Time.deltaTime * 3);
    }
}
