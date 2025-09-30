using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public int width, height;
    public StructureInfoManager structureInfoManager;
    public InventoryManager inventoryManager;

    public GameObject detailsPanel;
    Grid placementGrid;

    private Dictionary<Vector3Int, StructureModel> temporaryRoadobjects = new Dictionary<Vector3Int, StructureModel>();
    private Dictionary<Vector3Int, StructureModel> structureDictionary = new Dictionary<Vector3Int, StructureModel>();

    private GameObject selectedPrefab;

    private void Start()
    {
        placementGrid = new Grid(width, height);
    }

    internal CellType[] GetNeighbourTypesFor(Vector3Int position)
    {
        return placementGrid.GetAllAdjacentCellTypes(position.x, position.z);
    }

    internal bool CheckIfPositionInBound(Vector3Int position)
    {
        if (position.x >= 0 && position.x < width && position.z >= 0 && position.z < height)
        {
            return true;
        }
        return false;
    }

    internal GameObject PlaceObjectOnTheMap(Vector3Int position, Vector3 scale, GameObject structurePrefab, CellType type, int width = 1, int height = 1, int id = -1, bool isBigStructure = false, bool isAI = false)
    {

        StructureModel structure = CreateANewStructureModel(position, scale, structurePrefab, type, id, isBigStructure);

        structure.GameObject().GetComponent<StructureClickController>().isAi = isAI;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var newPosition = position + new Vector3Int(x, 0, z);
                placementGrid[newPosition.x, newPosition.z] = type;
                structureDictionary.Add(newPosition, structure);
                structure.GameObject().GetComponent<StructureClickController>().positions.Add(newPosition);
                DestroyNatureAt(newPosition);
            }
        }
        return structure.GameObject();

    }

    private void DestroyNatureAt(Vector3Int position)
    {
        RaycastHit[] hits = Physics.BoxCastAll(position + new Vector3(0, 0.5f, 0), new Vector3(0.5f, 0.5f, 0.5f), transform.up, Quaternion.identity, 1f, 1 << LayerMask.NameToLayer("Nature"));
        foreach (var item in hits)
        {
            Destroy(item.collider.gameObject);
        }
    }

    internal bool CheckIfPositionIsFree(Vector3Int position)
    {
        return CheckIfPositionIsOfType(position, CellType.Empty);
    }

    private bool CheckIfPositionIsOfType(Vector3Int position, CellType type)
    {
        return placementGrid[position.x, position.z] == type;
    }

    internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        placementGrid[position.x, position.z] = type;
        Vector3 scale = new Vector3(1, 1, 1);
        StructureModel structure = CreateANewStructureModel(position, scale, structurePrefab, type);
        temporaryRoadobjects.Add(position, structure);
    }

    internal void RemoveSelectedPrefab()
    {
        if (selectedPrefab != null)
        {
            Destroy(selectedPrefab);
        }
    }
    internal void PlaceCurrentSelection(Vector3Int position, Vector3 scale, GameObject structurePrefab, CellType type)
    {
        if (selectedPrefab != null)
        {
            Destroy(selectedPrefab);
        }
        selectedPrefab = CreateANewStructureModelGameObject(position, scale, structurePrefab, type);
    }

    internal void ReplaceCurrentSelection(Vector3Int position, Color color, int width = 1, int height = 1)
    {
        selectedPrefab.transform.localPosition = position;
        Renderer[] renderers = selectedPrefab.GetComponentsInChildren<Renderer>();
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = color;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = material;
        }
        // if (renderers.Length == 0)
        // {
        //     if (selectedPrefab.GetComponent<Renderer>() != null)
        //     {
        //         selectedPrefab.GetComponent<Renderer>().material.color = color;
        //     }
        //     else
        //     {
        //         selectedPrefab.AddComponent<Renderer>().material.color = color;
        //     }
        // }

    }

    internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type)
    {
        var neighbourVertices = placementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (var point in neighbourVertices)
        {
            neighbours.Add(new Vector3Int(point.X, 0, point.Y));
        }
        return neighbours;
    }

    private StructureModel CreateANewStructureModel(Vector3Int position, Vector3 scale, GameObject structurePrefab, CellType type, int id = -1, bool isBigStructure = false)
    {
        GameObject structure = new GameObject(type.ToString());
        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;
        structure.transform.localScale = scale;
        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);

        BoxCollider collider = structure.GetComponent<BoxCollider>();
        if (type == CellType.Road)
            return structureModel;
        if (collider == null)
        {
            collider = structure.AddComponent<BoxCollider>();
        }
        collider.size = structurePrefab.GetComponentInChildren<Renderer>().bounds.size;

        structure.AddComponent<StructureClickController>();
        structure.GetComponent<StructureClickController>().structureInfoManager = structureInfoManager;
        structure.GetComponent<StructureClickController>().id = id;
        structure.GetComponent<StructureClickController>().isBigStructure = isBigStructure;
        structure.GetComponent<StructureClickController>().detailsPanel = detailsPanel;
        structure.GetComponent<StructureClickController>().inventoryManager = inventoryManager;
        structure.GetComponent<StructureClickController>().placementManager = this;
        structure.GetComponent<StructureClickController>().positions = new HashSet<Vector3Int>();
        // structure.layer = LayerMask.NameToLayer("Building");

        return structureModel;
    }

    internal GameObject CreateANewStructureModelGameObject(Vector3Int position, Vector3 scale, GameObject structurePrefab, CellType type, int id = -1, bool isBigStructure = false)
    {
        GameObject structure = new GameObject(type.ToString());
        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;
        structure.transform.localScale = scale;
        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);

        if (structure.GetComponent<BoxCollider>() == null)
        {
            structure.AddComponent<BoxCollider>();
        }

        structure.AddComponent<StructureClickController>();
        structure.GetComponent<StructureClickController>().structureInfoManager = structureInfoManager;
        structure.GetComponent<StructureClickController>().id = id;
        structure.GetComponent<StructureClickController>().isBigStructure = isBigStructure;
        structure.GetComponent<StructureClickController>().detailsPanel = detailsPanel;
        structure.GetComponent<StructureClickController>().inventoryManager = inventoryManager;
        structure.GetComponent<StructureClickController>().placementManager = this;
        structure.GetComponent<StructureClickController>().positions = new HashSet<Vector3Int>();

        return structure;
    }

    internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition)
    {
        var resultPath = GridSearch.AStarSearch(placementGrid, new Point(startPosition.x, startPosition.z), new Point(endPosition.x, endPosition.z));
        List<Vector3Int> path = new List<Vector3Int>();
        foreach (Point point in resultPath)
        {
            path.Add(new Vector3Int(point.X, 0, point.Y));
        }
        return path;
    }

    internal void RemoveAllTemporaryStructures()
    {
        foreach (var structure in temporaryRoadobjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            placementGrid[position.x, position.z] = CellType.Empty;
            Destroy(structure.gameObject);
        }
        temporaryRoadobjects.Clear();
    }

    internal void AddtemporaryStructuresToStructureDictionary()
    {
        foreach (var structure in temporaryRoadobjects)
        {
            structureDictionary.Add(structure.Key, structure.Value);
            DestroyNatureAt(structure.Key);
        }
        temporaryRoadobjects.Clear();
    }

    public void ModifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        if (temporaryRoadobjects.ContainsKey(position))
            temporaryRoadobjects[position].SwapModel(newModel, rotation);
        else if (structureDictionary.ContainsKey(position))
            structureDictionary[position].SwapModel(newModel, rotation);
    }

    public void ClearLocation(Vector3Int location)
    {
        structureDictionary.Remove(location);
        placementGrid[location.x, location.z] = CellType.Empty;
    }

}
