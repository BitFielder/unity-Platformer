using System.Collections;
using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAfterImage : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        public void StartAfterImage(Sprite sprite)
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.sprite = sprite;
            StartCoroutine(AfterImage());
        }

        IEnumerator AfterImage()
        {
            Color startColor = _renderer.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
            for (int i = 0; i < 50; i++)
            {
                _renderer.color = Color.Lerp(_renderer.color, endColor, 0.25f);
                yield return new WaitForSeconds(.01f);
            }
            Destroy(gameObject);
        }
    }
}