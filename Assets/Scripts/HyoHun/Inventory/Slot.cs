using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image image;

    private Item _item;
    public Item item
    {
        get { return _item; }
        set
        {
            _item = value;

            if (_item != null)
            {
                image.sprite = _item.icon;
                image.color = new Color(1, 1, 1, 1);
            }
            else
            {
                image.sprite = null;
                image.color = new Color(1, 1, 1, 0);
            }
        }
    }
}