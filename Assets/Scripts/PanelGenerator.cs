using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject panelPrefab;
    public GameObject parent;

    public GameObject multiparent;

    public UIController uIController;

    public StructureInfoManager structureInfoManager;
    public DragDropManager dragDropManager;

    private int offset = 100;

    private int multiOffset = 100;

    public void AddPanel(int id, StructureInfo structureInfo, GameObject parent)
    {
        GameObject panel = Instantiate(panelPrefab, parent.transform);
        uIController.AddButton(panel.transform.Find("MoneyButton").GetComponent<Button>(), id);
        dragDropManager.DragDrop(panel.transform.Find("MoneyButton").GetComponent<Button>(), id);
        dragDropManager.DragDrop(panel.transform.Find("AIButton").GetComponent<Button>(), id, true);
        uIController.AddButton(panel.transform.Find("AIButton").GetComponent<Button>(), id, true);

        panel.transform.Find("Image").GetComponent<RawImage>().texture = structureInfo.image;
        panel.transform.Find("MoneyButton").GetComponent<Transform>().Find("Cost").GetComponent<TMP_Text>().text = "$" + structureInfo.weightedPrefab.weight;
        panel.transform.Find("MoneyButton").GetComponent<Transform>().Find("Time").GetComponent<TMP_Text>().text = structureInfo.weightedPrefab.time + "s";
        panel.transform.Find("AIButton").GetComponent<Transform>().Find("AICost").GetComponent<TMP_Text>().text = ""+structureInfo.weightedPrefab.aiCost;
        panel.transform.Find("AIButton").GetComponent<Transform>().Find("AITime").GetComponent<TMP_Text>().text = structureInfo.weightedPrefab.aiTime + "s";
        panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset, 0);
        panel.SetActive(true);
        offset += 250;

        RectTransform parentRectTransform = parent.GetComponent<RectTransform>();
        if (offset > parentRectTransform.rect.width)
        {
            parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x + 250, parentRectTransform.sizeDelta.y);
        }
    }

    public void AddPanelMulti(int id, StructureInfoMulti structureInfo, GameObject parent)
    {
        GameObject panel = Instantiate(panelPrefab, parent.transform);
        uIController.AddBigStructureButton(panel.transform.Find("MoneyButton").GetComponent<Button>(), id);
        dragDropManager.DragDropBigStructure(panel.transform.Find("MoneyButton").GetComponent<Button>(), id);
        dragDropManager.DragDropBigStructure(panel.transform.Find("AIButton").GetComponent<Button>(), id, true);
        uIController.AddBigStructureButton(panel.transform.Find("AIButton").GetComponent<Button>(), id, true);

        panel.transform.Find("Image").GetComponent<RawImage>().texture = structureInfo.image;
        panel.transform.Find("MoneyButton").GetComponent<Transform>().Find("Cost").GetComponent<TMP_Text>().text = "$" + structureInfo.weightedPrefab.weight;
        panel.transform.Find("MoneyButton").GetComponent<Transform>().Find("Time").GetComponent<TMP_Text>().text = structureInfo.weightedPrefab.time + "s";
        panel.transform.Find("AIButton").GetComponent<Transform>().Find("AICost").GetComponent<TMP_Text>().text = "$" + structureInfo.weightedPrefab.aiCost;
        panel.transform.Find("AIButton").GetComponent<Transform>().Find("AITime").GetComponent<TMP_Text>().text = structureInfo.weightedPrefab.aiTime + "s";
        panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(multiOffset, 0);
        panel.SetActive(true);
        multiOffset += 250;

        RectTransform parentRectTransform = parent.GetComponent<RectTransform>();
        if (multiOffset > parentRectTransform.rect.width)
        {
            parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x + 250, parentRectTransform.sizeDelta.y);
        }
    }

    public void Start()
    {
        for (int i = 0; i < structureInfoManager.buildingStructureInfos.Length; i++)
        {
            AddPanel(i, structureInfoManager.buildingStructureInfos[i], parent);
        }
        for (int i = 0; i < structureInfoManager.multiBuildingStructureInfos.Length; i++)
        {
            AddPanelMulti(i, structureInfoManager.multiBuildingStructureInfos[i], multiparent);
        }
    }
}
