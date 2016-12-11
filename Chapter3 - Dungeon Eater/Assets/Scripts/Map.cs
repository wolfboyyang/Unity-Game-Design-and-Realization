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

    public Transform floor;
    private GameObject gameController;

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
    private ParticleSystem.Particle[] gemParticles;
    private int totalGemNum;
    private int currentGemNum;
    public AudioClip pickupGemSound;

    private const int GemScore = 10;
    
    // Use this for initialization
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateModel()
    {
        LoadFromAsset(defaultMap);
        CreateMap("MapModel", true, false);
    }

    private void SetMapData(int stage)
    {
        if (stage > mapTextAssets.Length)
            stage %= mapTextAssets.Length;

        LoadFromAsset(mapTextAssets[stage]);
    }

    public void OnStageStart(int stage)
    {
        DestroyMap();
        SetMapData(stage);
        CreateMap("MapCollision", true, false);
        CreateMap("MapBlocks", false, false);
        floor.position = new Vector3(
            mapData.offsetX + (mapData.width-1) / 2.0f,
            floor.position.y, 
            mapData.offsetZ + (mapData.height-1) / 2.0f);
        floor.localScale = new Vector3(mapData.width / 10.0f, 1.0f, mapData.height / 10.0f);

        currentGemNum = totalGemNum;
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
        for (int z = mapData.height - 1, i = 1; z >= 0; z--, i++)
        {
            if (i >= lines.Length) break;

            var data = lines[i].Split(spliter, option);
            for (int x = 0; x < mapData.width; x++)
            {
                if (x >= data.Length) break;
                blockData[z, x] = data[x][0];
            }
        }
        mapData.data = blockData;

    }

    private void SetupGemsAndItems()
    {
        mapData.gemParticleIndex = new int[mapData.height, mapData.width];
        totalGemNum = 0;

        for(int z = 0; z < mapData.height; z++)
        {
            for(int x=0;x<mapData.width;x++)
            {
                if (IsGem(x, z))
                    totalGemNum++;
            }
        }

        gemEmitter = GetComponent<ParticleSystem>();
        ParticleSystem.EmitParams e = new ParticleSystem.EmitParams();
        gemEmitter.Emit(totalGemNum);

        gemParticles = new ParticleSystem.Particle[totalGemNum];
        var count = gemEmitter.GetParticles(gemParticles);
        int gemIndex = 0;
        for (int z = 0; z < mapData.height; z++)
        {
            for (int x = 0; x < mapData.width; x++)
            {
                var position = new Vector3(x + mapData.offsetX, GemY, z + mapData.offsetZ);
                mapData.gemParticleIndex[z, x] = -1;
                if (IsGem(x, z))
                {
                    gemParticles[gemIndex].position = position;
                    gemParticles[gemIndex].startSize = GemSize;
                    mapData.gemParticleIndex[z, x] = gemIndex;
                    gemIndex++;
                }

                if(mapData.data[z,x] == Sword)
                {
                    var go = Instantiate(itemObjects[0], position, Quaternion.identity);
                    go.transform.parent = items.transform;
                }
            }
        }

        gemEmitter.SetParticles(gemParticles, totalGemNum);
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
            Destroy(items, collisionMode);
        items = new GameObject("Iterm Folder");
        items.AddComponent<AudioSource>();

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
                            var go = Instantiate(
                                wallForCollision,
                                position + Vector3.up * 0.5f,
                                Quaternion.identity);
                            go.transform.parent = mapObjects.transform;
                        }
                        else
                        {
                            var go = Instantiate(
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

        var children = mapObjects.GetComponentsInChildren<Transform>();
        mapObjects.AddComponent<CombineChildren>();
        mapObjects.GetComponent<CombineChildren>().Combine();
        Destroy(mapObjects.GetComponent<CombineChildren>());

        // Note: first "child" is parent
        //for (int i = 1; i < children.Length; i++)
        //    Destroy(children[i].gameObject, collisionMode);
        
        if (collisionMode)
        {
            mapObjects.AddComponent<MeshCollider>();
            mapObjects.GetComponent<MeshCollider>().sharedMesh = mapObjects.GetComponent<MeshFilter>().mesh;
            Destroy(mapObjects.GetComponent<MeshRenderer>(), collisionMode);
            mapCollision = mapObjects;
        }

        if(!collisionMode)
            SetupGemsAndItems();
    }

    private void Destroy(Object obj, bool collisionMode)
    {
#if UNITY_EDITOR
        DestroyImmediate(obj);
#else
        Destroy(obj);
#endif
    }

    private void Destroy(Object[] obj, bool collisionMode = false)
    {
#if UNITY_EDITOR
        for(int i = 0; i < obj.Length; i++)
        {
            DestroyImmediate(obj[i]);
            obj[i] = null;
        }
#else
         for(int i = 0; i < obj.Length; i++)
        {
            Destroy(obj[i]);
            obj[i] = null;
        }
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

    public Vector3 GetSpawnPoint(SpawnPointType type)
    {
        int t = (int)type;
        if (t < spawnPositions.Length)
        {
            return spawnPositions[t];
        }
        else
        {
            Debug.LogWarning("Spawn Point is not found.");
            return new Vector3(MapOriginX, 0, MapOriginZ);
        }
    }

    public bool GetGridPosition(Vector3 position, out int x, out int z)
    {
        x = Mathf.RoundToInt(position.x);
        z = Mathf.RoundToInt(position.z);

        x -= mapData.offsetX;
        z -= mapData.offsetZ;
        if (x < 0 || x >= mapData.width || z < 0 || z >= mapData.height)
            return false;
        else
            return true;
    }

    public void PickupItem(Vector3 position)
    {
        int x, z;
        if(GetGridPosition(position, out x, out z))
        {
            var gemIndex = mapData.gemParticleIndex[z, x];
            if (gemIndex >= 0)
            {
                gemEmitter.GetParticles(gemParticles);
                gemParticles[gemIndex].startSize = 0;
                gemEmitter.SetParticles(gemParticles,totalGemNum);
                mapData.gemParticleIndex[z, x] = -1;
                GetComponent<AudioSource>().PlayOneShot(pickupGemSound);
                gameController.SendMessage("AddScore", GemScore);
                currentGemNum--;
                if (currentGemNum <= 0)
                    gameController.SendMessage("OnEatAll");
            }
        }
    }
    
}
