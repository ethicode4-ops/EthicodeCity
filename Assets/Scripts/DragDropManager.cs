using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDropManager : MonoBehaviour
{
    private StructurePrefabWeighted[] prefabs;

    private StructurePrefabWH[] bigStructuresPrefabs;

    public CameraManager cameraManager;

    public PlacementManager placementManager;

    public StructureManager structureManager;

    public StructureInfoManager structureInfoManager;
    bool isDragging = false;
    bool isAI = false;

    bool isBigStructure = false;

    bool initial = true;

    int currentPrefabIndex = 0;

    public InputManager inputManager;
    void Start()
    {

        StructureInfo[] structureInfos = structureInfoManager.buildingStructureInfos;
        prefabs = new StructurePrefabWeighted[structureInfos.Length];

        StructureInfoMulti[] structureInfoMulti = structureInfoManager.multiBuildingStructureInfos;
        bigStructuresPrefabs = new StructurePrefabWH[structureInfoMulti.Length];

        for (int i = 0; i < structureInfos.Length; i++)
        {
            prefabs[i] = structureInfos[i].weightedPrefab;
        }
        for (int i = 0; i < bigStructuresPrefabs.Length; i++)
        {
            bigStructuresPrefabs = structureInfoManager.bigStructuresPrefabs;
        }

    }

    public void DragDrop(Button button, int index, bool isAi = false)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) =>
        {
            if (initial)
            {
                return;
            }
            isDragging = true;
            Vector3Int? pos = inputManager.RaycastGround();
            isAI = isAi;

            if (pos != null)
            {
                currentPrefabIndex = index;
                placementManager.PlaceCurrentSelection(pos.Value, prefabs[index].scale, prefabs[index].prefab, CellType.Structure);
            }
        });
        trigger.triggers.Add(entry);
    }

    public void DragDropBigStructure(Button button, int index, bool isAI = false)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) =>
        {
            if (initial && !(bigStructuresPrefabs[index].isBank || bigStructuresPrefabs[index].isAiFactory))
            {
                return;
            }
            isDragging = true;
            Vector3Int? pos = inputManager.RaycastGround();
            this.isAI = isAI;
            if (pos != null)
            {
                currentPrefabIndex = index;
                isBigStructure = true;
                placementManager.PlaceCurrentSelection(pos.Value, bigStructuresPrefabs[index].scale, bigStructuresPrefabs[index].prefab, CellType.Structure);
            }
        });
        trigger.triggers.Add(entry);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            placementManager.RemoveSelectedPrefab();
            Vector3Int? pos = inputManager.RaycastGround();
            // cameraManager.cameraDragEnabled = true;

            if (pos != null && isBigStructure)
            {
                if (structureManager.PlaceBigStructure(pos.Value, bigStructuresPrefabs[currentPrefabIndex].width, bigStructuresPrefabs[currentPrefabIndex].height, currentPrefabIndex, isAI))
                {
                    initial = false;
                }
                isBigStructure = false;
            }
            else if (pos != null)
            {
                // structureManager.PlaceHouseBuffered(pos.Value, currentPrefabIndex);
                structureManager.PlaceHouseBufferedDelayed(pos.Value, currentPrefabIndex, isAI);
            }
        }
        if (isDragging)
        {
            Vector3Int? pos = inputManager.RaycastGround();
            if (pos != null && isBigStructure)
            {
                if (structureManager.CheckBigStructure(pos.Value, bigStructuresPrefabs[currentPrefabIndex].width, bigStructuresPrefabs[currentPrefabIndex].height))
                {
                    placementManager.ReplaceCurrentSelection(pos.Value, Color.green, bigStructuresPrefabs[currentPrefabIndex].width, bigStructuresPrefabs[currentPrefabIndex].height);
                }
                else
                {
                    placementManager.ReplaceCurrentSelection(pos.Value, Color.red, bigStructuresPrefabs[currentPrefabIndex].width, bigStructuresPrefabs[currentPrefabIndex].height);
                }
            }
            else if (pos != null)
            {
                if (structureManager.CheckBigStructure(pos.Value, 1, 1))
                {
                    placementManager.ReplaceCurrentSelection(pos.Value, Color.green);
                }
                else
                {
                    placementManager.ReplaceCurrentSelection(pos.Value, Color.red);
                }
            }
        }
    }
}
