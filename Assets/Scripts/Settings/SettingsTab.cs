using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SettingsTab : MonoBehaviour {
    public Image leftImage, middleImage, rightImage;
    public GameObject page;

    public void SetTabSprite(Sprite left, Sprite middle, Sprite right) {
        leftImage.sprite = left;
        middleImage.sprite = middle;
        rightImage.sprite = right;
    }
}
