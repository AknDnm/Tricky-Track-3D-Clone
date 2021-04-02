using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : Shooter
{
    [SerializeField] private int lineSegment = 10;
    // �lk etkili�eme girildi�inde ilk hedef al�n�cak pozisyon.
    [SerializeField] private Transform firstAim = null;
    [SerializeField] private float range = 5f;
    [SerializeField] private float aimSensitivity = 15f;
    [SerializeField] private float minYPosition = -10f;

    private float startPosY;
    private float startPosZ;
    private bool interacted = false;
    private LineRenderer lineRenderer;
    private GameManager gameManager;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    protected override void Start()
    {
        base.Start();
        lineRenderer.positionCount = lineSegment;
    }


    private void Update()
    {
        if(!gameManager.IsPlaying()) return;

        if(shootTimer > shootRate)
        {
            ConditionOfProjectileLauncher(true);
            AimWithMouse();
        }

        shootTimer += Time.deltaTime;
    }

    private void AimWithMouse()
    {
        // Ekran koordinatlar�ndan oyun d�nyas�ndaki koordinatlara d�n���m sa�lan�rken z koordinat�, aimSensitivity de�eri kadar 
        // kayd�r�ld�. B�ylece mouse hareket aral��� ayarlanarak kontrol hassasiyeti de�i�tirilebilecek.
        Vector3 mousePos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, aimSensitivity));

        // Mouse ilk bas�ld��� yerle ilk aim ald���m�z yer aras� offset al�narak, origin noktam�z belirleniyor.
        if (Input.GetMouseButtonDown(0))
        {
            startPosY = mousePos.y - firstAim.position.y;
            startPosZ = mousePos.z - firstAim.position.z;
            lineRenderer.enabled = true;
            interacted = true;
        }

        if (interacted)
        {
            // Origin noktam�zdan, ne kadar uzakla�t���m�za g�re yeni pozisyonumuz belirleniyor. X ekseni i�in mesafe, range de�i�keni
            // ile ayarlan�yor.
            Vector3 newPosition = new Vector3(range + firstAim.position.x, mousePos.y - startPosY, mousePos.z - startPosZ);
            Vector3 velocity = CalculateVelocity(newPosition, projectileLauncher.transform.position);

            if (Input.GetMouseButton(0))
            {
                Visualize(velocity);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                LauchProjectile(velocity);
                lineRenderer.enabled = false;
                shootTimer = 0;
                interacted = false;
                ConditionOfProjectileLauncher(false);
            }
        }
    }

    private void Visualize(Vector3 velocity)
    {
        // Burada kendi ayarlad���m�z minYPosition i�in s�re hesaplan�p zaman aral��� bulunuyor.
        // B�ylece �izginin kendi ayarlad���m�z y pozisyonuna kadar inmesi sa�lan�cak.
        var lowestTimeValue = CalculateMaxTimeInY(velocity) / (float)lineSegment;

        for (int i = 0; i < lineSegment; i++)
        {
            float t = lowestTimeValue * i;
            Vector3 pos = CalculatePosInTime(velocity, t);
            lineRenderer.SetPosition(i, pos);
        }
    }

    private Vector3 CalculatePosInTime(Vector3 Vo, float time)
    {
        Vector3 Vxz = Vo;
        Vxz.y = 0f;
        Vector3 projLaunch = projectileLauncher.position;
        projLaunch.y = 0f;

        Vector3 result = projLaunch + Vxz * time;

        // y = z + v.t - gt�/2  ( y = var�lmak istenilen pozisyon. z = ba�lang�� pozisyonu)
        float sY = (-0.5f * gravity * (time * time)) + Vo.y * time + projectileLauncher.position.y;
        result.y = sY;

        return result;
    }

    private float CalculateMaxTimeInY(Vector3 velocity)
    {
        float Vy = velocity.y;

        // y = z + v.t - gt�/2  ( y = var�lmak istenilen pozisyon. z = ba�lang�� pozisyonu)
        // Bu form�l yukar�daki form�lde t �ekilerek elde edildi. 
        float t = (Vy + Mathf.Sqrt(Vy * Vy + 2 * gravity * (projectileLauncher.position.y - (minYPosition)))) / gravity;
        return t;
    }
}
