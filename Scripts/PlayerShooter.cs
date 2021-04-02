using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : Shooter
{
    [SerializeField] private int lineSegment = 10;
    // Ýlk etkiliþeme girildiðinde ilk hedef alýnýcak pozisyon.
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
        // Ekran koordinatlarýndan oyun dünyasýndaki koordinatlara dönüþüm saðlanýrken z koordinatý, aimSensitivity deðeri kadar 
        // kaydýrýldý. Böylece mouse hareket aralýðý ayarlanarak kontrol hassasiyeti deðiþtirilebilecek.
        Vector3 mousePos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, aimSensitivity));

        // Mouse ilk basýldýðý yerle ilk aim aldýðýmýz yer arasý offset alýnarak, origin noktamýz belirleniyor.
        if (Input.GetMouseButtonDown(0))
        {
            startPosY = mousePos.y - firstAim.position.y;
            startPosZ = mousePos.z - firstAim.position.z;
            lineRenderer.enabled = true;
            interacted = true;
        }

        if (interacted)
        {
            // Origin noktamýzdan, ne kadar uzaklaþtýðýmýza göre yeni pozisyonumuz belirleniyor. X ekseni için mesafe, range deðiþkeni
            // ile ayarlanýyor.
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
        // Burada kendi ayarladýðýmýz minYPosition için süre hesaplanýp zaman aralýðý bulunuyor.
        // Böylece çizginin kendi ayarladýðýmýz y pozisyonuna kadar inmesi saðlanýcak.
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

        // y = z + v.t - gt²/2  ( y = varýlmak istenilen pozisyon. z = baþlangýç pozisyonu)
        float sY = (-0.5f * gravity * (time * time)) + Vo.y * time + projectileLauncher.position.y;
        result.y = sY;

        return result;
    }

    private float CalculateMaxTimeInY(Vector3 velocity)
    {
        float Vy = velocity.y;

        // y = z + v.t - gt²/2  ( y = varýlmak istenilen pozisyon. z = baþlangýç pozisyonu)
        // Bu formül yukarýdaki formülde t çekilerek elde edildi. 
        float t = (Vy + Mathf.Sqrt(Vy * Vy + 2 * gravity * (projectileLauncher.position.y - (minYPosition)))) / gravity;
        return t;
    }
}
