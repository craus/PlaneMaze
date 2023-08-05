using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DestroyExtensions
{
    public static void SoftDestroy(this MonoBehaviour mb, GameObject gameObject) {
        gameObject.SetActive(false);
        if (gameObject.GetComponent<Figure>() != null) {
            gameObject.GetComponent<Figure>().OnDestroy();
        }
        if (gameObject.GetComponent<Item>() != null) {
            gameObject.GetComponent<Item>().icon.gameObject.SetActive(false);
        }
    }
}
