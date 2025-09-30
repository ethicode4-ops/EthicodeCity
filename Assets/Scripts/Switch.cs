using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button[] Buttons;
    public GameObject[] GameObjects;

    public GameObject[] OtherGameObjects;
    void Start()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            int index = i;
            Buttons[index].onClick.AddListener(() =>
            {
                for (int j = 0; j < GameObjects.Length; j++)
                {
                    GameObjects[j].SetActive(false);
                }
                OtherGameObjects[index].SetActive(true);
            });
        }
    }
}
