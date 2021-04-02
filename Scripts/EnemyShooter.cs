using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Shooter
{
    // Oyuncu tarafýndaki buttona gönderilen projectile objelerinin hýzlarýnda random olarak 
    // sapma saðlanýcak. Böylece her zaman butona çarpmasý engellenicek.
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
            // Oyuncu tarafý için eðer engel açýksa ve daha önce enemy tarafýndan atýþ yapýlmadýysa, atýþ gerçekleþtrilicek.
            if (obstacle.IsPlayerObstacle() && obstacle.IsOpen() && !obstacle.IsHit())
            {
                Vector3 targetPos = obstacle.ButtonPosition();
                Vector3 randomTargetPos = new Vector3(Random.Range(targetPos.x - randomShootFactor, targetPos.x + randomShootFactor),
                                                      Random.Range(targetPos.y - randomShootFactor, targetPos.y + randomShootFactor),
                                                      Random.Range(targetPos.z - randomShootFactor, targetPos.z + randomShootFactor));
                Vector3 velocity = CalculateVelocity(randomTargetPos, projectileLauncher.position);
                LauchProjectile(velocity);
                // Sadece bir defa oyuncu tarafýndaki button için atýþ denemesi yapmasý için listeden çýkartýyoruz ve 
                // isHit deðiþkenini true yapýyoruz.
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

            // Eðer projectile fýrlatýþý gerçekleþmiþse foreach döngüsünden çýkýlýcak ve shootTimer beklenicek bir dahaki atýþ için.
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
        // Obstacle objesinin altýnda bulunan button objesinin collider komponentini hedef aldýðýmýz için
        // GetComponentInParent() fonksiyonunu çaðýrdýk.
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
