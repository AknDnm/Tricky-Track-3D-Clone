using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float runSpeed = 10f;

    private CharacterController characterController;
    private bool isStop = true;

    private void OnEnable()
    {
        Events.StopRunning.AddListener(Stop);
        Events.StartRunning.AddListener(StartRunning);
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isStop) return;

        characterController.Move(new Vector3(runSpeed, 0, 0) * Time.deltaTime);

    }

    public void StopAtPosition(Vector3 stopPosition)
    {
        Stop();
        transform.position = new Vector3(stopPosition.x, transform.position.y, transform.position.z);
    }

    public void Stop()
    {
        isStop = true;   
    }

    public void StartRunning()
    {
        isStop = false;
    }

    private void OnDisable()
    {
        Events.StopRunning.RemoveListener(Stop);
        Events.StartRunning.RemoveListener(StartRunning);
    }
}
