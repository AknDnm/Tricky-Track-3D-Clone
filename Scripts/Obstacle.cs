using System;
using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Obstacle kapal�ysa karakterin durdurulaca�� pozisyon.
    [SerializeField] private Transform stopPosition = null;

    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isHitted = false;
    [SerializeField] private bool isPlayerObstacle = true;
    [SerializeField] private Button button = null;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        animator.SetBool("OpenDoor", isOpen);
        button.SetButton(isOpen);
    }

    // Button class�nda bulunan onCollisionEvent taraf�ndan �a�r�l�yor.
    public void HandleObstacleCondition()
    {
        isOpen = !isOpen;
        animator.SetBool("OpenDoor", isOpen);
    }

    private void OnTriggerStay(Collider other)
    {
        Mover mover = other.GetComponent<Mover>();
        if (mover == null) return;

        // Burada obstacle objesinin rakibin ya da oyuncunun ko�tu�u yola ait olup olmad��� ve collider a giren objenin 
        // rakip ya da oyuncu olup olmad��� kontrol ediyor. E�er iki ko�uldan biri do�ruysa karater "stopPosition" lokasyonuna
        // gelince durduruluyor.
        if ((isPlayerObstacle && mover.CompareTag("Player")) || (!isPlayerObstacle && mover.CompareTag("Enemy")))
        {
            if (mover.transform.position.x > stopPosition.position.x) 
            {
                other.gameObject.GetComponent<Mover>().StopAtPosition(stopPosition.position);
            }
        }
    }

    // Butona bas�ld���nda animasyondaki collider keyframe ile collider kald�r�l�yor.
    private void OnTriggerExit(Collider other)
    {
        Mover playerController = other.GetComponent<Mover>();
        if (playerController != null) { playerController.GetComponent<Mover>().StartRunning(); }
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    public bool IsPlayerObstacle()
    {
        return isPlayerObstacle;
    }

    public bool IsHit()
    {
        return isHitted;
    }

    public void SetIsHit(bool condition)
    {
        isHitted = condition;
    }

    public Vector3 ButtonPosition()
    {
        return button.transform.position;
    }
}
