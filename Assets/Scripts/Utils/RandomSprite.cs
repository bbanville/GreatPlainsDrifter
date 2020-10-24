using System.Collections.Generic;
using UnityEngine;

namespace BrendonBanville.Tools
{
    public class RandomSprite : MonoBehaviour
    {
        public List<Sprite> Sprites;

        void Start ()
        {
            var sr = GetComponent<SpriteRenderer>();
            sr.sprite = Sprites.Random();
        }
    }
}