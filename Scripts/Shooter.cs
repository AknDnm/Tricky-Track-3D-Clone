using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Rigidbody projectilePrefab = null;
    // Projectile objesinin f�lart�laca�� yer.
    [SerializeField] protected Transform projectileLauncher = null;
    [SerializeField] protected float time = 1f;
    [SerializeField] protected float shootRate = 2f;

    protected new Camera camera;

    protected float shootTimer = Mathf.Infinity;
    protected float gravity;

    protected virtual void Start()
    {
        camera = Camera.main;
        gravity = Mathf.Abs(Physics.gravity.y);
    }

    protected void LauchProjectile(Vector3 velocity)
    {
        Rigidbody obj = Instantiate(projectilePrefab, projectileLauncher.position, Quaternion.identity);
        obj.velocity = velocity;
    }

    protected Vector3 CalculateVelocity(Vector3 target, Vector3 origin)
    {
        Vector3 distance = target - origin;

        // Yatay ve dikey eksendeki mesafeleri ayr� hesaplamak i�in y eksenini s�f�rlad�k.
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        // Yatay ve dikey eksendeki uzakl�klar.
        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;
        
        float Vxz = Sxz / time;
        // Dikey eksendeki h�z� bulmak i�in kinetik enerjiden potansiyel enerjiyi ��kararak bu form�l� elde ettik.
        float Vy = (Sy / time) + (0.5f * gravity * time);

        // Vekt�r�n normu y�n� belirlemek i�in.
        Vector3 result = distanceXZ.normalized;

        result *= Vxz;
        result.y = Vy;

        return result;
    }

    protected void ConditionOfProjectileLauncher(bool condition)
    {
        projectileLauncher.gameObject.SetActive(condition);
    }
}
