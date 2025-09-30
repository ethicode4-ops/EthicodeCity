using UnityEngine;

public class ExtractTrees : MonoBehaviour
{
    public Terrain terrain;

    public string treeLayerName = "Nature";
    void Start()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();
        int treeLayer = LayerMask.NameToLayer(treeLayerName);
        TerrainData terrainData = terrain.terrainData;
        foreach (var tree in terrainData.treeInstances)
        {
            Vector3 worldPosition = Vector3.Scale(tree.position, terrainData.size) + terrain.transform.position;
            GameObject treePrefab = terrainData.treePrototypes[tree.prototypeIndex].prefab;
            GameObject newTree = Instantiate(treePrefab, worldPosition, Quaternion.identity);
            newTree.transform.localScale = Vector3.one * tree.widthScale;
            newTree.layer = treeLayer;
        }

        // terrainData.treeInstances = new TreeInstance[0];
        // terrainData.RefreshPrototypes();
        terrain.treeDistance = 0;
    }
}