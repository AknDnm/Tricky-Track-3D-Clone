using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Canvas[] canvasGroups = null; // 0 -> Start 1 -> Game Over 2 -> Winner

    private bool isPlaying = false;

    private void Start()
    {
        canvasGroups[0].gameObject.SetActive(true);
        canvasGroups[1].gameObject.SetActive(false);
        canvasGroups[2].gameObject.SetActive(false);
    }

    private void ActivateGameOver()
    {
        canvasGroups[1].gameObject.SetActive(true);
    }

    private void ActivateWinner()
    {
        canvasGroups[2].gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public bool IsPlaying()
    {
        return isPlaying;
    }


    public void StartGame()
    {
        isPlaying = true;
        Events.StartRunning.Invoke();
        canvasGroups[0].gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaying) return;

        if (other.CompareTag("Player"))
        {
            ActivateWinner();
            isPlaying = false;
            Events.StopRunning.Invoke();
        }
        else if (other.CompareTag("Enemy"))
        {
            ActivateGameOver();
            isPlaying = false;
            Events.StopRunning.Invoke();
        }
    }
}
