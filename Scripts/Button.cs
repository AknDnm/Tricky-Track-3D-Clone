using UnityEngine.Events;
using UnityEngine;

public class Button : MonoBehaviour
{
    // Carpisma gerçeklestiginde Parent objesinde bulunan obstacle classindaki fonksiyon cagriliyor bu event ile.
    [SerializeField] private UnityEvent onCollisionEvent;

    [SerializeField] private Material[] materials = null; // 0 -> Green 1 -> Red

    private bool isPressed = false;
    private new Renderer renderer;
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public void SetButton(bool condition)
    {
        isPressed = !condition;
        if (isPressed) { renderer.material = materials[0]; }
        else { renderer.material = materials[1]; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        SetButton(isPressed);

        onCollisionEvent.Invoke();
    }
}
