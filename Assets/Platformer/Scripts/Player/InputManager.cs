using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer
{
    public class InputManager : MonoBehaviour
    {
        public Vector2 move;
        public bool jump;
        public bool dash;
        public bool grab;

        void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        void OnJump(InputValue value)
        {
            jump = value.isPressed;
        }

        void OnDash(InputValue value)
        {
            dash = value.isPressed;
        }

        void OnGrab(InputValue value)
        {
            grab = value.isPressed;
        }
    }
}