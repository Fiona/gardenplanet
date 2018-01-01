using System;
using StompyBlondie;
using UnityEngine;
using UnityEngine.UI;

namespace StrawberryNova
{
    public class PlayerEnergyHeart: MonoBehaviour
    {
        public Image heartImage;

        // Between 0-1
        public void SetValue(float amount)
        {
            heartImage.transform.localScale = new Vector3(amount, amount, amount);
            Pop(amount > 0f && amount < 1f);
        }

        public void Pop(bool yes)
        {
            transform.localScale = yes ? new Vector3(1.2f, 1.2f, 1.2f) : new Vector3(1f, 1f, 1f);
        }
    }
}