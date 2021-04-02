using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Shooter
{
    // Oyuncu tarafindaki buttona gonderilen projectile objelerinin pozisyonlarinda random olarak 
    // sapma saglanicak. Boylece her zaman butona carpmasi engellenicek.
    [SerializeField] private float randomShootFactor = 2f;

    private List<Obstacle> obstaclesInRange;

    protected override void Start()
    {
        base.Start();
        obstaclesInRange = new List<Obstacle>();
    }

    private void Update()
    {
        if(shootTimer > shootRate)
        {
            ConditionOfProjectileLauncher(true);
            Shoot();
        }
        shootTimer += Time.deltaTime;
    }

    private void Shoot()
    {
        bool stopIteration = false;

        foreach (Obstacle obstacle in obstaclesInRange)
        {
            // Oyuncu tarafi icin eger engel aciksa ve daha once enemy tarafindan atis yapilmadiysa, atis gerceklestirilicek.
            if (obstacle.IsPlayerObstacle() && obstacle.IsOpen() && !obstacle.IsHit())
            {
                Vector3 targetPos = obstacle.ButtonPosition();
                Vector3 randomTargetPos = new Vector3(Random.Range(targetPos.x - randomShootFactor, targetPos.x + randomShootFactor),
                                                      Random.Range(targetPos.y - randomShootFactor, targetPos.y + randomShootFactor),
                                                      Random.Range(targetPos.z - randomShootFactor, targetPos.z + randomShootFactor));
                Vector3 velocity = CalculateVelocity(randomTargetPos, projectileLauncher.position);
                LauchProjectile(velocity);
                // Sadece bir defa oyuncu tarafindaki buttona atis denemesi yapmasi icin listeden cikartiyoruz ve 
                // isHit degiskenini true yapiyoruz.
                obstaclesInRange.Remove(obstacle);
                obstacle.SetIsHit(true);

                stopIteration = true;
            }
            else if (!obstacle.IsPlayerObstacle() && !obstacle.IsOpen())
            {
                Vector3 velocity = CalculateVelocity(obstacle.ButtonPosition(), projectileLauncher.position);
                LauchProjectile(velocity);
                
                stopIteration = true;
            }

            // Eðer projectile firlatisi gerçeklesmisse foreach dongusunden cikilicak ve shootTimer beklenicek bir dahaki atýs icin.
            if (stopIteration)
            {
                ConditionOfProjectileLauncher(false);
                shootTimer = 0f;
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Obstacle objesinin altinda bulunan button objesinin collider komponentini hedef aldigimiz icin
        // GetComponentInParent() fonksiyonunu cagirdik.
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();

        if(obstacle != null && !obstaclesInRange.Contains(obstacle))
        {
            obstaclesInRange.Add(obstacle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();
        if (obstacle != null && obstaclesInRange.Contains(obstacle))
        {
            obstaclesInRange.Remove(obstacle);
        }
    }
}
