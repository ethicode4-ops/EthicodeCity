using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class StructureClickController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int id;

    public bool isBigStructure = false;

    public bool isAi = false;

    public bool isBank = false;

    public bool isAiFactory = false;

    public float time;

    public float updateTime;
    public StructureInfoManager structureInfoManager;
    public InventoryManager inventoryManager;

    public PlacementManager placementManager;

    public GameObject detailsPanel;

    public HashSet<Vector3Int> positions;

    private bool notTriedToDestroy = true;

    private void Update()
    {
        time += Time.deltaTime;
        updateTime += Time.deltaTime;
        if (updateTime > 10 && isBank)
        {
            inventoryManager.AddMoney(100);
            updateTime = 0;
        }
        if (updateTime > 10 && isAiFactory)
        {
            inventoryManager.AddAiCredits(10);
            updateTime = 0;
        }

        if (time > 10 && notTriedToDestroy)
        {
            notTriedToDestroy = false;
            if (isAi && Random.value >= 0 && !isBank && !isAiFactory)
            {
                if (!structureInfoManager.structureManager.AIDestroyed)
                {
                    StartCoroutine(DestroyAI());
                }
                StartCoroutine(DestroyModel());
            }
        }

    }

    private IEnumerator DestroyModel()
    {
        Renderer renderer = transform.GetComponentsInChildren<Renderer>()[0];
        Material oldMaterial = renderer.material;
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = Color.red };
        for (int i = 0; i < 2 * 2; i++)
        {
            if (i % 2 == 1)
            {
                renderer.material = oldMaterial;
            }
            else
            {
                renderer.material = material;
            }
            yield return new WaitForSeconds(0.5f);
        }
        Clear();
        Destroy(gameObject);
    }

    private IEnumerator DestroyAI()
    {
        while (structureInfoManager.structureManager.eventInProgress)
        {
            yield return null;
        }
        structureInfoManager.structureManager.eventInProgress = true;
        structureInfoManager.structureManager.slidePanelController.EnableAchievement("AI");
        structureInfoManager.structureManager.AIDestroyed = true;
        yield return structureInfoManager.structureManager.dialogueManager.AIBuildingDialogue();
        structureInfoManager.structureManager.eventInProgress = false;

    }

    void OnMouseDown()
    {
        if (!isBigStructure)
        {
            Debug.Log("placed " + structureInfoManager.structureInfoDictionary.TryGetValue(id, out StructureInfo info));
            Debug.Log("ID " + id);
            UpdateDetailsPanel(info.weightedPrefab.weight, info.weightedPrefab.time, info.image);
        }
        else
        {
            Debug.Log("placed multi " + structureInfoManager.multiStructureInfoDictionary.TryGetValue(id, out StructureInfoMulti info));
            Debug.Log("ID " + id);
            UpdateDetailsPanel(info.weightedPrefab.weight, info.weightedPrefab.time, info.image);
        }

        detailsPanel.SetActive(true);
    }

    internal void UpdateDetailsPanel(float cost, float time, RenderTexture image)
    {
        detailsPanel.SetActive(true);
        detailsPanel.transform.Find("Cost").GetComponent<TMP_Text>().text = "$" + cost;
        detailsPanel.transform.Find("Time").GetComponent<TMP_Text>().text = time + "s";
        detailsPanel.transform.Find("Image").GetComponent<RawImage>().texture = image;
        detailsPanel.transform.Find("RemoveButton").GetComponent<Button>().onClick.RemoveAllListeners();
        detailsPanel.transform.Find("RemoveButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(gameObject);
            inventoryManager.AddMoney(cost);
            detailsPanel.transform.Find("RemoveButton").GetComponent<Button>().onClick.RemoveAllListeners();
            foreach (Vector3Int position in positions)
            {
                placementManager.ClearLocation(position);
            }
            detailsPanel.SetActive(false);
        });
    }

    internal void Clear()
    {
        foreach (Vector3Int position in positions)
        {
            placementManager.ClearLocation(position);
        }
    }
}
