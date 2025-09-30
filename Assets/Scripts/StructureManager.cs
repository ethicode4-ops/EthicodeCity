using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public StructurePrefabWeighted[] housesPrefabe, specialPrefabs;

    public StructurePrefabWH[] bigStructuresPrefabs;
    public PlacementManager placementManager;

    public InventoryManager inventoryManager;

    public EarthquakeMovement earthquakeMovement;

    public SlidePanelController slidePanelController;

    public questionManager questionManager;

    public CameraManager cameraManager;

    public GameObject panelPrefab;

    public DialogueManager dialogueManager;

    private readonly float[] specialWeights;

    private float earthQuaketimer = 0;
    private float bankRobbingTimer = 0;

    private int hospitalCount = 0;


    public bool eventInProgress = false;

    public bool AIDestroyed = false;
    private float earthquakeProbability = 0.5f;

    private float bankRobberyProbability = 0.5f;
    private int moneyClearPercentage = 100;
    // private bool bankRobberyOccured = false;

    private Queue<GameObject> ObjectsInMap = new();

    private Queue<GameObject> AIObjectsInMap = new();

    private void Update()
    {
        earthQuaketimer += Time.deltaTime;
        bankRobbingTimer += Time.deltaTime;
        if (earthQuaketimer > 100)
        {
            earthQuaketimer = 0;

            if (UnityEngine.Random.value < earthquakeProbability && AIObjectsInMap.Count != 0)
            {
                earthquakeProbability = Mathf.Max(0.1f, earthquakeProbability - 0.1f);
                StartCoroutine(EarthQuake());
                Debug.Log("Earthquake event triggered.");
            }
            else
            {
                Debug.Log("Earthquake event skipped.");
            }
        }

        if (bankRobbingTimer > 120)
        {
            bankRobbingTimer = 0;

            if (UnityEngine.Random.value < bankRobberyProbability && AIObjectsInMap.Count != 0)
            {
                bankRobberyProbability = Mathf.Max(0.1f, bankRobberyProbability - 0.1f);
                StartCoroutine(BankRobbery());
                Debug.Log("Bank robbery event triggered.");
            }
            else
            {
                Debug.Log("Bank robbery event skipped.");
            }
        }

        int k = ObjectsInMap.Count;

        for (int i = 0; i < k; i++)
        {
            ObjectsInMap.TryDequeue(out GameObject obj);
            if (obj != null)
            {
                ObjectsInMap.Enqueue(obj);
            }
        }

        k = AIObjectsInMap.Count;
        for (int i = 0; i < k; i++)
        {
            AIObjectsInMap.TryDequeue(out GameObject obj);
            if (obj != null)
            {
                AIObjectsInMap.Enqueue(obj);
            }
        }
    }

    private IEnumerator BankRobbery()
    {
        while (eventInProgress)
        {
            yield return null;
        }

        eventInProgress = true;
        foreach (var obj in AIObjectsInMap)
        {
            if (obj != null)
            {
                if (obj.GetComponent<StructureClickController>().isAi && obj.GetComponent<StructureClickController>().isBank)
                {
                    inventoryManager.ClearMoney(moneyClearPercentage);
                    moneyClearPercentage = Mathf.Max(20, moneyClearPercentage - 10);
                    slidePanelController.EnableAchievement("CB");
                    break;
                }
            }
        }
        yield return StartCoroutine(dialogueManager.BankDestroyDialogue());
        eventInProgress = false;
        // bankRobberyOccured = true;
    }

    private IEnumerator EarthQuake()
    {
        while (eventInProgress)
        {
            yield return null;
        }
        eventInProgress = true;
        yield return dialogueManager.EarthQuakeDialogue(0);
        yield return new WaitForSeconds(2);
        earthquakeMovement.TriggerEarthquake(5, 0.1f);


        float chance = 1;
        foreach (var obj in AIObjectsInMap)
        {
            if (obj != null && UnityEngine.Random.value <= chance && !obj.GetComponent<StructureClickController>().isBank) // lets's not collapse banks
            {
                obj.GetComponent<StructureClickController>().Clear();
                chance -= .05f;
                chance = Mathf.Max(0, chance);
                StartCoroutine(DestroyBuilding(obj));
            }
        }
        yield return new WaitForSeconds(5);
        yield return dialogueManager.EarthQuakeDialogue(1);
        slidePanelController.EnableAchievement("CTQ");
        eventInProgress = false;
        // if (earthQuakeOccured)
        // {
        //     yield break;
        // }
        // GameObject panel = Instantiate(panelPrefab, GameObject.Find("Canvas").transform);
        // panel.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "AI Code is not perfect. So buildings collapsed.";
        // panel.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => Destroy(panel));
        // panel.SetActive(true);
        // earthQuakeOccured = true;
    }

    private IEnumerator DestroyBuilding(GameObject obj)
    {
        // if (obj.transform.GetComponent<Renderer>() == null)
        // {
        //     obj.transform.AddComponent<Renderer>();
        // }

        Renderer renderer = obj.transform.GetComponentsInChildren<Renderer>()[0];
        Material oldMaterial = renderer.material;
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = Color.red };
        for (int i = 0; i < 2 * 5; i++)
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

        Destroy(obj);
    }

    internal void PlaceHouseBufferedDelayed(Vector3Int position, int houseNum, bool isAi = false)
    {
        if (CheckPositionBeforePlacement(position) && inventoryManager.CanBuy(housesPrefabe[houseNum].weight) && !isAi)
        {
            StartCoroutine(DelayedPlacement(position, houseNum, isAi));
        }
        else if (CheckPositionBeforePlacement(position) && inventoryManager.CanBuyAi(housesPrefabe[houseNum].aiCost) && isAi)
        {
            StartCoroutine(DelayedPlacement(position, houseNum, isAi));
        }
        else if (CheckPositionBeforePlacement(position) == false && !eventInProgress)
        {
            StartCoroutine(dialogueManager.BuildingPlacementDialogue(0));
        }
        else if (!eventInProgress)
        {
            StartCoroutine(dialogueManager.BuildingPlacementDialogue(1));
        }
    }

    private IEnumerator DelayedPlacement(Vector3Int position, int houseNum, bool isAi)
    {
        Debug.Log("Placement started.");
        float placementTime = isAi ? housesPrefabe[houseNum].aiTime : housesPrefabe[houseNum].time;
        GameObject gameObject = placementManager.CreateANewStructureModelGameObject(position, housesPrefabe[houseNum].scale, housesPrefabe[houseNum].prefab, CellType.Structure, houseNum);
        Renderer renderer = gameObject.GetComponentsInChildren<Renderer>()[0];
        Material oldMaterial = renderer.material;
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = Color.Lerp(Color.green, Color.red, housesPrefabe[houseNum].aiPercentage); ;

        for (int i = 0; i < 2 * placementTime; i++)
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
        Destroy(gameObject);
        GameObject obj = placementManager.PlaceObjectOnTheMap(position, housesPrefabe[houseNum].scale, housesPrefabe[houseNum].prefab, CellType.Structure, 1, 1, houseNum, isAI: isAi);
        ObjectsInMap.Enqueue(obj);

        if (!isAi)
        {
            inventoryManager.Buy(housesPrefabe[houseNum].weight);
            // houseCount++;
            inventoryManager.UpdateHappiness();
        }
        else
        {
            AIObjectsInMap.Enqueue(obj);
            inventoryManager.SpendAiCredits(housesPrefabe[houseNum].aiCost);
        }

        AudioPlayer.instance.PlayPlacementSound();
        Debug.Log("Placement completed.");
    }

    private IEnumerator DelayedPlacementMulti(Vector3Int position, int houseNum, bool isAi, int width, int height)
    {
        Debug.Log("Placement started Multi.");
        float placementTime = isAi ? bigStructuresPrefabs[houseNum].aiTime : bigStructuresPrefabs[houseNum].time;
        GameObject gameObject = placementManager.CreateANewStructureModelGameObject(position, bigStructuresPrefabs[houseNum].scale, bigStructuresPrefabs[houseNum].prefab, CellType.Structure, houseNum, true);
        Renderer renderer = gameObject.GetComponentsInChildren<Renderer>()[0];

        Material oldMaterial = renderer.material;
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = Color.Lerp(Color.green, Color.red, housesPrefabe[houseNum].aiPercentage) };

        for (int i = 0; i < 2 * placementTime; i++)
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
        Destroy(gameObject);

        GameObject obj = placementManager.PlaceObjectOnTheMap(position, bigStructuresPrefabs[houseNum].scale, bigStructuresPrefabs[houseNum].prefab, CellType.Structure, width, height, houseNum, true, isAi);
        obj.GetComponent<StructureClickController>().isBank = bigStructuresPrefabs[houseNum].isBank;
        obj.GetComponent<StructureClickController>().isAiFactory = bigStructuresPrefabs[houseNum].isAiFactory;

        if (isAi && bigStructuresPrefabs[houseNum].isBank)
        {
            while (eventInProgress)
            {
                yield return null;
            }
            eventInProgress = true;
            yield return dialogueManager.BankBuildDialogue();
            eventInProgress = false;
        }

        if (isAi && bigStructuresPrefabs[houseNum].isAiFactory)
        {
            while (eventInProgress)
            {
                yield return null;
            }
            eventInProgress = true;
            yield return dialogueManager.FactoryDialogue();
            yield return WaitforQuestion();
            slidePanelController.EnableAchievement("WC");
            eventInProgress = false;
        }
        if (!bigStructuresPrefabs[houseNum].isBank && !bigStructuresPrefabs[houseNum].isAiFactory)
        {
            if (hospitalCount == 0)
            {
                while (eventInProgress)
                {
                    yield return null;
                }
                eventInProgress = true;
                yield return dialogueManager.DocumentationDialogue();
                eventInProgress = false;
            }
            else if (hospitalCount == 4)
            {
                while (eventInProgress)
                {
                    yield return null;
                }
                eventInProgress = true;
                slidePanelController.EnableAchievement("CD");
                yield return dialogueManager.CongratulationsDialogue();
                eventInProgress = false;
            }
            hospitalCount++;
        }


        ObjectsInMap.Enqueue(obj);
        if (!isAi)
        {
            inventoryManager.Buy(bigStructuresPrefabs[houseNum].weight);
            // houseCount++;
            // inventoryManager.UpdateHappiness();
        }
        else
        {
            AIObjectsInMap.Enqueue(obj);
            inventoryManager.SpendAiCredits(bigStructuresPrefabs[houseNum].aiCost);
        }
        AudioPlayer.instance.PlayPlacementSound();
        Debug.Log("Placement completed.");
    }

    private IEnumerator WaitforQuestion()
    {
        bool old = cameraManager.cameraDragEnabled;
        cameraManager.cameraDragEnabled = false;
        yield return StartCoroutine(questionManager.StartQuestion());
        cameraManager.cameraDragEnabled = old;
    }

    internal void PlaceHouseBuffered(Vector3Int position, int houseNum)
    {
        if (CheckPositionBeforePlacement(position) && inventoryManager.CanBuy(housesPrefabe[houseNum].weight))
        {

            placementManager.PlaceObjectOnTheMap(position, housesPrefabe[houseNum].scale, housesPrefabe[houseNum].prefab, CellType.Structure, houseNum);
            inventoryManager.Buy(housesPrefabe[houseNum].weight);
            inventoryManager.UpdateHappiness();
            AudioPlayer.instance.PlayPlacementSound();
        }
        else if (inventoryManager.CanBuy(housesPrefabe[houseNum].weight) == false)
        {
            Debug.Log("Not enough money");
        }
    }

    internal bool PlaceBigStructure(Vector3Int position, int width, int height, int bigStructureIndex, bool isAI = false)
    {
        if (CheckBigStructure(position, width, height) && inventoryManager.CanBuy(bigStructuresPrefabs[bigStructureIndex].weight) && !isAI)
        {
            StartCoroutine(DelayedPlacementMulti(position, bigStructureIndex, isAI, width, height));
            return true;
        }
        else if (CheckBigStructure(position, width, height) && inventoryManager.CanBuyAi(bigStructuresPrefabs[bigStructureIndex].aiCost) && isAI)
        {
            StartCoroutine(DelayedPlacementMulti(position, bigStructureIndex, isAI, width, height));
            return true;
        }

        else if (!CheckBigStructure(position, width, height) && !eventInProgress)
        {
            StartCoroutine(dialogueManager.BuildingPlacementDialogue(0));
        }
        else if (!eventInProgress)
        {
            StartCoroutine(dialogueManager.BuildingPlacementDialogue(1));
        }

        return false;
    }

    internal bool CheckBigStructure(Vector3Int position, int width, int height)
    {
        bool nearRoad = false;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var newPosition = position + new Vector3Int(x, 0, z);

                if (DefaultCheck(newPosition) == false)
                {
                    return false;
                }
                if (nearRoad == false)
                {
                    nearRoad = RoadCheck(newPosition);
                }
            }
        }
        return nearRoad;
    }

    public void PlaceSpecial(Vector3Int position)
    {
        if (CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(specialWeights);
            placementManager.PlaceObjectOnTheMap(position, specialPrefabs[randomIndex].scale, specialPrefabs[randomIndex].prefab, CellType.Structure);
            AudioPlayer.instance.PlayPlacementSound();
        }
    }

    private int GetRandomWeightedIndex(float[] weights)
    {
        float sum = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
        }

        float randomValue = UnityEngine.Random.Range(0, sum);
        float tempSum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            //0->weihg[0] weight[0]->weight[1]
            if (randomValue >= tempSum && randomValue < tempSum + weights[i])
            {
                return i;
            }
            tempSum += weights[i];
        }
        return 0;
    }

    private bool CheckPositionBeforePlacement(Vector3Int position)
    {
        if (DefaultCheck(position) == false)
        {
            return false;
        }

        if (RoadCheck(position) == false)
            return false;

        return true;
    }

    private bool RoadCheck(Vector3Int position)
    {
        if (placementManager.GetNeighboursOfTypeFor(position, CellType.Road).Count <= 0)
        {
            Debug.Log("Must be placed near a road");
            return false;
        }
        return true;
    }

    private bool DefaultCheck(Vector3Int position)
    {
        if (placementManager.CheckIfPositionInBound(position) == false)
        {
            Debug.Log("This position is out of bounds");
            return false;
        }
        if (placementManager.CheckIfPositionIsFree(position) == false)
        {
            Debug.Log("This position is not EMPTY");
            return false;
        }
        return true;
    }

    // public float GetHouseCount()
    // {
    //     return houseCount;
    // }
}

[Serializable]
public struct StructurePrefabWeighted
{
    public GameObject prefab;
    [Range(100, 10_000)]
    public float weight;
    public Vector3 scale;

    public int time;
    [Range(10, 100)]
    public int aiCost;
    public int aiTime;

    [Range(0, 1)]
    public float aiPercentage;

    public StructurePrefabWeighted(GameObject prefab, float weight, int time, float aiPercentage, int aiCost, int aiTime)
    {
        this.prefab = prefab;
        this.weight = weight;
        this.time = time;
        this.aiCost = aiCost;
        this.aiTime = aiTime;
        this.aiPercentage = aiPercentage;
        scale = new Vector3(1, 1, 1);

    }
}


[Serializable]
public struct StructurePrefabWH
{
    public GameObject prefab;
    [Range(100, 10_000)]
    public float weight;
    public Vector3 scale;

    public int width;
    public int height;

    public int time;

    public int aiCost;

    public int aiTime;

    [Range(0, 1)]
    public float aiPercentage;

    public bool isBank;
    public bool isAiFactory;

    public StructurePrefabWH(GameObject prefab, float weight, int width, int height, int time, float aiPercentage, int aiCost, int aiTime, bool isBank, bool isAiFactory)
    {
        this.prefab = prefab;
        this.weight = weight;
        this.width = width;
        this.height = height;
        this.time = time;
        this.aiCost = aiCost;
        this.aiTime = aiTime;
        this.aiPercentage = aiPercentage;
        this.isBank = isBank;
        this.isAiFactory = isAiFactory;
        scale = new Vector3(1, 1, 1);

    }
}
