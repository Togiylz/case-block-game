using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    public static BlockPool Instance; // Singleton yapýsý

    public GameObject[] blockPrefabs; // Blok prefablarý
    public int poolSizePerPrefab = 20; // Her prefab için havuz boyutu

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    private int activeColorCount; // Aktif renk sayýsý

    public int ActiveColorCount
    {
        get { return activeColorCount; }
        private set { activeColorCount = value; }
    }

    private void Awake()
    {
        // Singleton yapýsý: Sadece bir BlockPool örneði olmalý
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Havuzu baþlat
        InitializePool();
    }

    // Havuzu baþlat
    private void InitializePool()
    {
        foreach (GameObject prefab in blockPrefabs)
        {
            if (prefab == null)
            {
                continue;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                GameObject block = Instantiate(prefab, transform);
                block.SetActive(false);
                objectPool.Enqueue(block);
            }

            poolDictionary[prefab] = objectPool;
        }
    }

    // Aktif renk sayýsýný ayarla
    public void SetActiveColorCount(int colorCount)
    {
        if (colorCount < 1 || colorCount > blockPrefabs.Length)
        {
            return;
        }

        ActiveColorCount = colorCount;
    }

    // Belirli bir renk ID'sine ait bloklarý havuzdan temizle
    public void ClearBlocksByColorID(int colorID)
    {
        if (colorID < 0 || colorID >= blockPrefabs.Length)
        {
            return;
        }

        GameObject prefab = blockPrefabs[colorID];

        if (poolDictionary.ContainsKey(prefab))
        {
            Queue<GameObject> objectPool = poolDictionary[prefab];
            while (objectPool.Count > 0)
            {
                GameObject block = objectPool.Dequeue();
                Destroy(block);
            }

            poolDictionary.Remove(prefab);
        }
    }

    // Havuzdan blok al
    public GameObject GetBlock(int colorID)
    {
        if (colorID < 0 || colorID >= ActiveColorCount)
        {
            return null;
        }

        GameObject prefab = blockPrefabs[colorID];

        if (prefab == null)
        {
            return null;
        }

        if (!poolDictionary.ContainsKey(prefab))
        {
            InitializePoolForPrefab(prefab);
        }

        Queue<GameObject> objectPool = poolDictionary[prefab];

        if (objectPool.Count > 0)
        {
            GameObject block = objectPool.Dequeue();
            block.SetActive(true);
            return block;
        }
        else
        {
            GameObject newBlock = Instantiate(prefab, transform);
            return newBlock;
        }
    }

    // Belirli bir prefab için havuzu baþlat
    private void InitializePoolForPrefab(GameObject prefab)
    {
        if (prefab == null)
        {
            return;
        }

        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < poolSizePerPrefab; i++)
        {
            GameObject block = Instantiate(prefab, transform);
            block.SetActive(false);
            objectPool.Enqueue(block);
        }

        poolDictionary[prefab] = objectPool;
    }

    // Bloðu havuza geri gönder
    public void ReturnBlock(GameObject block)
    {
        if (block == null)
        {
            return;
        }

        block.SetActive(false);

        Block blockComponent = block.GetComponent<Block>();
        if (blockComponent != null)
        {
            blockComponent.ResetIcon();
        }

        GameObject prefab = blockPrefabs[blockComponent.colorID];
        if (poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab].Enqueue(block);
        }
    }
}