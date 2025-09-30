using System;
using System.Collections.Generic;
using UnityEngine;

public class StructureInfoManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public StructureInfo[] buildingStructureInfos;

    public StructureInfoMulti[] multiBuildingStructureInfos;
    public StructureManager structureManager;

    public StructurePrefabWeighted[] structurePrefabWeighted;

    public StructurePrefabWH[] bigStructuresPrefabs;

    public Dictionary<int, StructureInfo> structureInfoDictionary = new();
    public Dictionary<int, StructureInfoMulti> multiStructureInfoDictionary = new();

    public int bigStructureOffset = 100;

    private void Start()
    {
        structurePrefabWeighted = new StructurePrefabWeighted[buildingStructureInfos.Length];
        bigStructuresPrefabs = new StructurePrefabWH[multiBuildingStructureInfos.Length];
        foreach (StructureInfo structureInfo in buildingStructureInfos)
        {
            structureInfoDictionary.Add(structureInfo.id, structureInfo);
            structurePrefabWeighted[structureInfo.id] = structureInfo.weightedPrefab;
        }
        foreach (StructureInfoMulti structureInfo in multiBuildingStructureInfos)
        {
            multiStructureInfoDictionary.Add(structureInfo.id - bigStructureOffset, new StructureInfoMulti(structureInfo.image, structureInfo.id, structureInfo.weightedPrefab));
            bigStructuresPrefabs[structureInfo.id - bigStructureOffset] = structureInfo.weightedPrefab;
        }
        structureManager.bigStructuresPrefabs = bigStructuresPrefabs;
        structureManager.housesPrefabe = structurePrefabWeighted;
    }


}

[Serializable]
public struct StructureInfo
{

    public int id;

    public RenderTexture image;
    public StructurePrefabWeighted weightedPrefab;

    public StructureInfo(RenderTexture image, int id, StructurePrefabWeighted prefabWeighted)
    {
        this.image = image;
        this.id = id;

        weightedPrefab = prefabWeighted;
    }
}

[Serializable]
public struct StructureInfoMulti
{

    public int id;

    public RenderTexture image;
    public StructurePrefabWH weightedPrefab;


    public StructureInfoMulti(RenderTexture image, int id, StructurePrefabWH prefabWeighted)
    {
        this.image = image;
        this.id = id;
        weightedPrefab = prefabWeighted;
    }
}
