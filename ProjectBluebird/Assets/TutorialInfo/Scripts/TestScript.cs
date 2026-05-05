using UnityEngine;
using UnityEngine.InputSystem;

public class TestScript : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // The new system way of getting horizontal input
        float moveX = 0;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) 
                moveX = -1;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) 
                moveX = 1;
        }

        transform.Translate(moveX * speed * Time.deltaTime, 0, 0);
    }
}
