using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Shooter
{
    // Oyuncu taraf�ndaki buttona g�nderilen projectile objelerinin h�zlar�nda random olarak 
    // sapma sa�lan�cak. B�ylece her zaman butona �arpmas� engellenicek.
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
            // Oyuncu taraf� i�in e�er engel a��ksa ve daha �nce enemy taraf�ndan at�� yap�lmad�ysa, at�� ger�ekle�trilicek.
            if (obstacle.IsPlayerObstacle() && obstacle.IsOpen() && !obstacle.IsHit())
            {
                Vector3 targetPos = obstacle.ButtonPosition();
                Vector3 randomTargetPos = new Vector3(Random.Range(targetPos.x - randomShootFactor, targetPos.x + randomShootFactor),
                                                      Random.Range(targetPos.y - randomShootFactor, targetPos.y + randomShootFactor),
                                                      Random.Range(targetPos.z - randomShootFactor, targetPos.z + randomShootFactor));
                Vector3 velocity = CalculateVelocity(randomTargetPos, projectileLauncher.position);
                LauchProjectile(velocity);
                // Sadece bir defa oyuncu taraf�ndaki button i�in at�� denemesi yapmas� i�in listeden ��kart�yoruz ve 
                // isHit de�i�kenini true yap�yoruz.
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

            // E�er projectile f�rlat��� ger�ekle�mi�se foreach d�ng�s�nden ��k�l�cak ve shootTimer beklenicek bir dahaki at�� i�in.
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
        // Obstacle objesinin alt�nda bulunan button objesinin collider komponentini hedef ald���m�z i�in
        // GetComponentInParent() fonksiyonunu �a��rd�k.
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
