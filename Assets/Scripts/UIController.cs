using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Action OnRoadPlacement, OnSpecialPlacement, OnClear;

    public Action<int, bool> OnHousePlacement;
    public StructureManager structureManager;
    public Action<int, int, int> OnBigStructurePlacement;
    public Button placeRoadButton, placeSpecialButton, placeHouseButton;

    public Button[] placeHouseButtons, placeBigStructureButtons;

    public Color outlineColor;
    List<Button> buttonList;

    private void Start()
    {
        buttonList = new List<Button> { placeRoadButton, placeSpecialButton, };

        placeRoadButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(placeRoadButton);
            OnRoadPlacement?.Invoke();

        });

        placeHouseButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            OnClear?.Invoke();

        });
        for (int i = 0; i < placeHouseButtons.Length; i++)
        {
            int index = i;

        }

        placeSpecialButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(placeSpecialButton);
            OnSpecialPlacement?.Invoke();

        });
    }

    private void ModifyOutline(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.enabled = true;
    }

    private void ResetButtonColor()
    {
        foreach (Button button in buttonList)
        {
            button.GetComponent<Outline>().enabled = false;
        }
    }

    public void AddButton(Button button, int id, bool isAi = false)
    {
        button.onClick.AddListener(() =>
            {
                ResetButtonColor();
                Debug.Log(id);
                OnHousePlacement?.Invoke(id, isAi);
            });
    }

    public void AddBigStructureButton(Button button, int id, bool isAi = false)
    {
        button.onClick.AddListener(() =>
        {
            ResetButtonColor();
            Debug.Log(id);
            ModifyOutline(button);
            OnBigStructurePlacement?.Invoke(id, structureManager.bigStructuresPrefabs[id].width, structureManager.bigStructuresPrefabs[id].height);
        });
    }
}
