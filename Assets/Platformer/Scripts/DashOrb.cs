using UnityEngine;

namespace Platformer
{
    public class DashOrb : MonoBehaviour
    {
        public void Consume()
        {
            gameObject.SetActive(false);
            Invoke(nameof(Reveal), 3f);
        }

        void Reveal()
        {
            gameObject.SetActive(true);
        }
    }
}