using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : Shooter
{
    [SerializeField] private int lineSegment = 10;
    // Ilk etkilesime girildiginde ilk hedef alinicak pozisyon.
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
        // Ekran koordinatlarindan oyun dunyasindaki koordinatlara donusum saglanirken z koordinati, aimSensitivity degeri kadar 
        // kaydirildi. Boylece mouse hareket araligi ayarlanarak kontrol hassasiyeti degistirilebilecek.
        Vector3 mousePos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, aimSensitivity));

        // Mouse ilk basildigi yerle ilk aim aldigimiz yer arasi offset alinarak, origin noktamiz belirleniyor.
        if (Input.GetMouseButtonDown(0))
        {
            startPosY = mousePos.y - firstAim.position.y;
            startPosZ = mousePos.z - firstAim.position.z;
            lineRenderer.enabled = true;
            interacted = true;
        }

        if (interacted)
        {
            // Origin noktamizdan, ne kadar uzaklastigimiza gore yeni pozisyonumuz belirleniyor. X ekseni icin mesafe, range degiskeni
            // ile ayarlaniyor.
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
        // Burada kendi ayarladigimiz minYPosition icin sure hesaplanip zaman araligi bulunuyor.
        // Boylece cizginin kendi ayarladigimiz y pozisyonuna kadar inmesi saglanicak.
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

        // y = z + v.t - gt²/2  ( y = varilmak istenilen pozisyon. z = baslangic pozisyonu)
        float sY = (-0.5f * gravity * (time * time)) + Vo.y * time + projectileLauncher.position.y;
        result.y = sY;

        return result;
    }

    private float CalculateMaxTimeInY(Vector3 velocity)
    {
        float Vy = velocity.y;

        // y = z + v.t - gt²/2  ( y = varilmak istenilen pozisyon. z = baslangic pozisyonu)
        // Bu formul yukaridaki formulde t cekilerek elde edildi. 
        float t = (Vy + Mathf.Sqrt(Vy * Vy + 2 * gravity * (projectileLauncher.position.y - (minYPosition)))) / gravity;
        return t;
    }
}
