using UnityEngine;
using UnityEngine.InputSystem;

// Delete once real movement exists.
public class PipelineTestMover : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        float input = 0f;
        if (keyboard.aKey.isPressed) input -= 1f;
        if (keyboard.dKey.isPressed) input += 1f;

        transform.position += Vector3.right * (input * speed * Time.deltaTime);
    }
}
