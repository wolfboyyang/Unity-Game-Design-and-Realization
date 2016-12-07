using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    private const int MapOriginX = 100;
    private const int MapOriginZ = 100;

    private const char Gem                  = 'c';
    private const char Wall                 = '*';
    private const char Sword                = 's';
    private const char PlayerSpawnPoint     = 'p';
    private const char TreasureSpawnPoint   = 't';

    private const float WallY = 0.0f;
    private const float GemY = 0.5f;
    private const float GemSize = 0.4f;

    public Transform[] children;

    public enum SpawnPointType
    {
        Player = 0,
        Enemy1,
        Enemy2,
        Enemy3,
        Enemy4,
        Treasure,
        Num // Count
    }

    struct MapData
    {
        public int width;
        public int height;
        public int offsetX;
        public int offsetZ;
        public char[,] data;
        public int[,] gemParticleIndex;
    }

    private MapData mapData;
    private Vector3[] spawnPositions;

    private GameObject items;
    private GameObject mapObjects;
    private GameObject mapCollision;

    public GameObject[] wallObjects;
    public GameObject[] itemObjects;
    public GameObject wallForCollision;

    public TextAsset defaultMap;
    public TextAsset[] mapTextAssets;

    private ParticleSystem gemEmitter;
    private int totalGemNum;
    private int currentGemNum;
    public AudioClip pickGemSound;

    private const int GemScore = 10;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateModel()
    {
        LoadFromAsset(defaultMap);
        CreateMap("MapModel", false, false);
    }

    private void SetMapData()
    {

    }

    public void OnStageStart()
    {

    }

    private void LoadFromAsset(TextAsset asset)
    {
        if (asset == null)
        {
            Debug.LogError("No Map Data Asset.");
            return;
        }

        mapData.offsetX = MapOriginX;
        mapData.offsetZ = MapOriginZ;

        string textData = asset.text;
        var option = System.StringSplitOptions.RemoveEmptyEntries;
        var lines = textData.Split(new char[] { '\r', '\n' }, option);

        var spliter = new char[] { ',' };

        var size = lines[0].Split(spliter, option);
        mapData.width = int.Parse(size[0]);
        mapData.height = int.Parse(size[1]);

        var blockData = new char[mapData.height, mapData.width];
        for(int i = 0; i < mapData.height; i++)
        {
            if (i + 1 >= lines.Length) break;

            var data = lines[i+1].Split(spliter, option);
            for(int j = 0; j < mapData.width; j++)
            {
                if (j >= data.Length) break;
                blockData[i, j] = data[j][0];
            }
        }
        mapData.data = blockData;

    }

    private void SetupGemsAndItems()
    {
        mapData.gemParticleIndex = new int[mapData.height, mapData.width];
        totalGemNum = 0;

        for(int i = 0; i < mapData.height; i++)
        {
            for(int j=0;j<mapData.width;j++)
            {
                if (IsGem(j, i))
                    totalGemNum++;
            }
        }

        gemEmitter.Emit(totalGemNum);

        //var gemParticle = gemEmitter.GetParticles()

    }

    private bool IsGem(int x, int z)
    {
        switch (mapData.data[z, x])
        {
            case Gem:
            case '1':
            case '2':
            case '3':
            case '4':
                return true;
        }

        return false;
    }

    private void CreateMap(string mapName, bool collisionMode, bool modelOnly = false)
    {
        mapObjects = new GameObject(mapName);
        spawnPositions = new Vector3[(int)SpawnPointType.Num];

        if (items != null)
            Destroy(items);

        for (int z = 0; z < mapData.height; z++)
        {
            for (int x = 0; x < mapData.width; x++)
            {
                var position = new Vector3(x + mapData.offsetX, 0.0f, z + mapData.offsetZ);
                switch(mapData.data[z,x])
                {
                    case Wall:
                        if (collisionMode)
                        {
                            var go = Instantiate<GameObject>(
                                wallForCollision,
                                position + Vector3.up * 0.5f,
                                Quaternion.identity);
                            go.transform.parent = mapObjects.transform;
                        }
                        else
                        {
                            var go = Instantiate<GameObject>(
                                wallObjects[0],
                                position + Vector3.up * WallY,
                                Quaternion.identity);
                            go.transform.parent = mapObjects.transform;
                        }
                        break;
                    case PlayerSpawnPoint:
                        spawnPositions[(int)SpawnPointType.Player] = position;
                        break;
                    case TreasureSpawnPoint:
                        spawnPositions[(int)SpawnPointType.Treasure] = position;
                        break;
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                        int enemyType = mapData.data[z, x] - '0';
                        spawnPositions[enemyType] = position;
                        break;
                    // Ghost trap
                    case '5':
                        spawnPositions[1] = position;
                        spawnPositions[2] = position;
                        break;
                }
            }
        }

        if (modelOnly) return;

        children = mapObjects.GetComponentsInChildren<Transform>();
        mapObjects.AddComponent<CombineChildren>();
        mapObjects.GetComponent<CombineChildren>().Combine();

        // Note: first "child" is parent
        for (int i = 1; i < children.Length; i++)
            Destroy(children[i].gameObject, collisionMode);
        
        //if (collisionMode)
        //{
        //    mapObjects.AddComponent<MeshCollider>();
        //     mapObjects.GetComponent<MeshCollider>().sharedMesh = mapObjects.GetComponent<MeshFilter>().mesh;
        //    Destroy(mapObjects.GetComponent<MeshRenderer>(), collisionMode);
        //    mapCollision = mapObjects;
        //}

        //if(!collisionMode)
        //    SetupGemsAndItems();
    }

    private void Destroy(Object obj, bool collisionMode)
    {
#if UNITY_EDITOR
        DestroyImmediate(obj);
#else
        Destroy(obj);
#endif
    }
    private void DestroyMap()
    {
        if (mapObjects != null)
            Destroy(mapObjects);
        mapObjects = null;

        if (mapCollision = null)
            Destroy(mapCollision);
        if (items != null)
            Destroy(items);
        items = null;
    }

    private int GetBlock(Vector3 position)
    {
        int gridX = Mathf.RoundToInt(position.x);
        int gridZ = Mathf.RoundToInt(position.z);

        gridX -= mapData.offsetX;
        gridZ -= mapData.offsetZ;
        if (gridX < 0 || gridX >= mapData.width || gridZ < 0 || gridZ >= mapData.height)
            return 0;

        return mapData.data[gridZ, gridX];
    }

    
}
