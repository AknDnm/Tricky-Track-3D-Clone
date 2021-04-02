using UnityEngine.Events;
using UnityEngine;

public class Button : MonoBehaviour
{
    // �arp��ma ger�ekle�ti�inde Parent objesinde bulunan obstacle class�ndaki fonksiyon �a�r�l�yor bu event ile.
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