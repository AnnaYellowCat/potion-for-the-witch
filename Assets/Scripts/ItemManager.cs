using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;

    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public string sceneName;
        public string parentChestName;
        public int spriteIndex;
        public bool isTargetItem;
        public bool isCollected;
        public string uniqueId;
        public int targetFrameIndex = -1;
        public bool isInInventory = false;
        public int inventorySlotIndex = -1;
    }

    public List<ItemData> allItemsData = new List<ItemData>();
    private Dictionary<string, ItemData> itemsDictionary = new Dictionary<string, ItemData>();

    [Header("Sprite Settings")]
    public string spritesPath = "Random Items";
    private Sprite[] itemSprites;

    public int targetItemsCount = 5;

    [Header("Target Frames")]
    public GameObject[] targetFrames;
    public Transform targetFramesContainer;

    [Header("Inventory Slots")]
    public int maxInventorySize = 5;
    private GameObject[] inventorySlots;
    private List<ItemData> inventoryItems = new List<ItemData>();

    private GameStateManager gameStateManager;
    private bool isInitialized = false;

    private List<int> level1SpriteIndices = new List<int>();
    private List<int> level2SpriteIndices = new List<int>();
    private List<int> targetAllIndices = new List<int>();
    private Dictionary<int, int> targetIndexToFrameMap = new Dictionary<int, int>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSpritesFromResources();
    }

    private void Start()
    {
        gameStateManager = FindFirstObjectByType<GameStateManager>();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        StartCoroutine(ApplyItemsStateAfterLoad(scene.name));
    }

    private IEnumerator ApplyItemsStateAfterLoad(string sceneName)
    {
        yield return new WaitForEndOfFrame();

        if (!isInitialized)
        {
            if (sceneName == "Level1")
            {
                InitializeSpritePools();

                CollectItemsFromCurrentScene();

                isInitialized = true;
            }
        }

        if (sceneName == "Level2" && allItemsData.Count > 0)
        {
            bool hasLevel2Items = allItemsData.Any(item => item.sceneName == "Level2");
            if (!hasLevel2Items)
            {
                CollectItemsFromCurrentScene();
            }
        }

        ActivateAndSetupChestItems();

        ApplyItemsState();

        ApplySpritesToInactiveItems();

        UpdateTargetFrames();

        FindInventorySlots();
        UpdateInventoryDisplay();
    }

    private void InitializeSpritePools()
    {
        if (itemSprites == null || itemSprites.Length < 24)
        {
            Debug.LogError("Not enough sprites! Need at least 24.");
            return;
        }

        List<int> allIndices = Enumerable.Range(0, itemSprites.Length).ToList();
        allIndices = allIndices.OrderBy(x => Random.value).ToList();

        List<int> selectedIndices = allIndices.Take(24).ToList();

        level1SpriteIndices = selectedIndices.Take(12).ToList();
        level2SpriteIndices = selectedIndices.Skip(12).Take(12).ToList();

        List<int> targetLevel1 = level1SpriteIndices.OrderBy(x => Random.value).Take(2).ToList();
        List<int> targetLevel2 = level2SpriteIndices.OrderBy(x => Random.value).Take(3).ToList();

        targetAllIndices = targetLevel1.Concat(targetLevel2).OrderBy(x => Random.value).ToList();

        targetIndexToFrameMap.Clear();
        for (int i = 0; i < targetAllIndices.Count; i++)
        {
            targetIndexToFrameMap[targetAllIndices[i]] = i;
        }
    }

    private void ApplySpritesToInactiveItems()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        Unboxing[] chests = FindObjectsByType<Unboxing>(FindObjectsSortMode.None);
        foreach (var chest in chests)
        {
            foreach (Transform child in chest.transform)
            {
                if (child.CompareTag("ChestItem"))
                {
                    GameObject item = child.gameObject;
                    string itemKey = GetItemKey(item);

                    if (itemsDictionary.ContainsKey(itemKey))
                    {
                        ItemData itemData = itemsDictionary[itemKey];
                        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();

                        if (spriteRenderer != null && itemSprites != null && itemData.spriteIndex < itemSprites.Length)
                        {
                            spriteRenderer.sprite = itemSprites[itemData.spriteIndex];
                            if (!item.activeSelf)
                            {
                                spriteRenderer.enabled = false;
                            }
                        }
                    }
                }
            }
        }
    }

    private void FindInventorySlots()
    {
        GameObject[] slots = GameObject.FindGameObjectsWithTag("InventorySlot");
        if (slots.Length > 0)
        {
            inventorySlots = slots.OrderBy(s => s.name).ToArray();
            return;
        }

        List<GameObject> foundSlots = new List<GameObject>();
        for (int i = 1; i <= maxInventorySize; i++)
        {
            GameObject slot = GameObject.Find($"Slot{i}");
            if (slot != null)
            {
                foundSlots.Add(slot);
            }
        }
        if (foundSlots.Count > 0)
        {
            inventorySlots = foundSlots.OrderBy(s => s.name).ToArray();
        }
    }

    public void UpdateInventoryDisplay()
    {
        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            FindInventorySlots();
            if (inventorySlots == null || inventorySlots.Length == 0)
                return;
        }

        foreach (var slot in inventorySlots)
        {
            if (slot == null) continue;
            Transform itemTransform = slot.transform.Find("Item");
            if (itemTransform != null)
            {
                SpriteRenderer itemSprite = itemTransform.GetComponent<SpriteRenderer>();
                if (itemSprite != null)
                {
                    itemSprite.sprite = null;
                    itemSprite.enabled = false;
                }
            }
        }

        for (int i = 0; i < inventoryItems.Count && i < inventorySlots.Length; i++)
        {
            ItemData item = inventoryItems[i];
            GameObject slot = inventorySlots[i];
            if (slot == null) continue; // защита
            Transform itemTransform = slot.transform.Find("Item");
            if (itemTransform == null) continue;

            SpriteRenderer itemSprite = itemTransform.GetComponent<SpriteRenderer>();
            if (itemSprite != null && itemSprites != null && item.spriteIndex < itemSprites.Length)
            {
                itemSprite.sprite = itemSprites[item.spriteIndex];
                itemSprite.enabled = true;
            }
        }
    }

    public bool AddItemToInventory(GameObject item)
    {
        if (item == null) return false;

        string itemKey = GetItemKey(item);

        if (!itemsDictionary.ContainsKey(itemKey))
        {
            return false;
        }

        ItemData itemData = itemsDictionary[itemKey];

        if (itemData.isInInventory)
        {
            return false;
        }

        if (inventoryItems.Count >= maxInventorySize)
        {
            return false;
        }

        itemData.isInInventory = true;
        itemData.inventorySlotIndex = inventoryItems.Count;
        inventoryItems.Add(itemData);

        UpdateInventoryDisplay();

        return true;
    }

    public bool HasInventorySpace()
    {
        return inventoryItems.Count < maxInventorySize;
    }

    public int GetInventoryCount()
    {
        return inventoryItems.Count;
    }

    public List<ItemData> GetInventoryItems()
    {
        return new List<ItemData>(inventoryItems);
    }

    public bool IsInventoryFull()
    {
        return inventoryItems.Count >= maxInventorySize;
    }

    public bool IsItemInInventory(GameObject item)
    {
        string itemKey = GetItemKey(item);
        if (itemsDictionary.ContainsKey(itemKey))
        {
            return itemsDictionary[itemKey].isInInventory;
        }
        return false;
    }

    public void SaveInventoryState(GameStateManager gameState, string sceneName)
    {
        if (!gameState.scenesState.ContainsKey(sceneName))
        {
            gameState.scenesState[sceneName] = new GameStateManager.SceneState();
        }

        var sceneState = gameState.scenesState[sceneName];

        sceneState.inventoryItems.Clear();
        foreach (var item in inventoryItems)
        {
            sceneState.inventoryItems.Add(new GameStateManager.InventoryItemState
            {
                uniqueId = item.uniqueId,
                slotIndex = item.inventorySlotIndex
            });
        }
    }

    public void RestoreInventoryState(GameStateManager gameState, string sceneName)
    {
        if (gameState.scenesState.ContainsKey(sceneName))
        {
            var sceneState = gameState.scenesState[sceneName];

            foreach (var item in allItemsData)
            {
                item.isInInventory = false;
                item.inventorySlotIndex = -1;
            }
            inventoryItems.Clear();

            foreach (var invItem in sceneState.inventoryItems)
            {
                var itemData = allItemsData.FirstOrDefault(item => item.uniqueId == invItem.uniqueId);
                if (itemData != null)
                {
                    itemData.isInInventory = true;
                    itemData.inventorySlotIndex = invItem.slotIndex;

                    while (inventoryItems.Count <= invItem.slotIndex)
                    {
                        inventoryItems.Add(null);
                    }
                    inventoryItems[invItem.slotIndex] = itemData;
                }
            }

            inventoryItems = inventoryItems.Where(item => item != null).ToList();
        }
    }

    public void ClearInventory()
    {
        foreach (var item in allItemsData)
        {
            item.isInInventory = false;
            item.inventorySlotIndex = -1;
        }
        inventoryItems.Clear();
    }

    public void CollectItemsFromCurrentScene()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        List<int> currentLevelIndices = null;
        if (currentScene == "Level1")
            currentLevelIndices = level1SpriteIndices;
        else if (currentScene == "Level2")
            currentLevelIndices = level2SpriteIndices;

        GameObject[] sceneItems = GameObject.FindGameObjectsWithTag("Item");
        Unboxing[] chests = FindObjectsByType<Unboxing>(FindObjectsSortMode.None);
        List<GameObject> chestItems = new List<GameObject>();

        foreach (var chest in chests)
        {
            foreach (Transform child in chest.transform)
            {
                if (child.CompareTag("ChestItem"))
                {
                    chestItems.Add(child.gameObject);
                }
            }
        }

        List<GameObject> allSceneItems = new List<GameObject>();
        allSceneItems.AddRange(sceneItems);
        allSceneItems.AddRange(chestItems);

        foreach (GameObject item in allSceneItems)
        {
            if (item == null) continue;

            bool exists = false;
            foreach (var existingItem in allItemsData)
            {
                if (existingItem.sceneName == currentScene)
                {
                    Unboxing parentChest = item.GetComponentInParent<Unboxing>();
                    string parentName = parentChest != null ? parentChest.gameObject.name : "";

                    if (existingItem.itemName == item.name && existingItem.parentChestName == parentName)
                    {
                        exists = true;
                        string existingKey = GetItemKey(existingItem);
                        itemsDictionary[existingKey] = existingItem;
                        break;
                    }
                }
            }

            if (exists) continue;

            string uniqueId = System.Guid.NewGuid().ToString();
            ItemData itemData = new ItemData();
            itemData.itemName = item.name;
            itemData.sceneName = currentScene;
            itemData.uniqueId = uniqueId;

            Unboxing parentChestComp = item.GetComponentInParent<Unboxing>();
            if (parentChestComp != null)
            {
                itemData.parentChestName = parentChestComp.gameObject.name;
            }
            else
            {
                itemData.parentChestName = "";
            }

            if (currentLevelIndices != null && currentLevelIndices.Count > 0)
            {
                int spriteIndex = currentLevelIndices[0];
                currentLevelIndices.RemoveAt(0);
                itemData.spriteIndex = spriteIndex;

                if (targetIndexToFrameMap.ContainsKey(spriteIndex))
                {
                    itemData.isTargetItem = true;
                    itemData.targetFrameIndex = targetIndexToFrameMap[spriteIndex];
                }
                else
                {
                    itemData.isTargetItem = false;
                    itemData.targetFrameIndex = -1;
                }
            }
            else
            {
                Debug.LogError($"No more unique sprites for scene {currentScene}! Item {item.name} will get random sprite.");
                if (itemSprites != null && itemSprites.Length > 0)
                {
                    itemData.spriteIndex = Random.Range(0, itemSprites.Length);
                }
                else
                {
                    itemData.spriteIndex = -1;
                }
                itemData.isTargetItem = false;
                itemData.targetFrameIndex = -1;
            }

            itemData.isCollected = false;
            itemData.isInInventory = false;
            itemData.inventorySlotIndex = -1;

            allItemsData.Add(itemData);

            string newKey = GetItemKey(itemData);
            itemsDictionary[newKey] = itemData;
        }
    }

    public void UpdateTargetFrames()
    {
        GameObject[] frames = GameObject.FindGameObjectsWithTag("TargetFrame");
        frames = frames.OrderBy(f => f.name).ToArray();

        if (frames.Length == 0 || targetAllIndices == null || targetAllIndices.Count == 0)
            return;

        for (int i = 0; i < frames.Length && i < targetAllIndices.Count; i++)
        {
            int spriteIndex = targetAllIndices[i];
            Transform itemTransform = frames[i].transform.Find("Item");
            if (itemTransform == null) continue;

            SpriteRenderer itemSprite = itemTransform.GetComponent<SpriteRenderer>();
            if (itemSprite == null) continue;

            if (itemSprites != null && spriteIndex >= 0 && spriteIndex < itemSprites.Length)
            {
                itemSprite.sprite = itemSprites[spriteIndex];
                itemSprite.enabled = true;
            }
        }
    }

    public void ApplyItemsState()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        GameObject[] sceneItems = GameObject.FindGameObjectsWithTag("Item");
        GameObject[] chestItems = GameObject.FindGameObjectsWithTag("ChestItem");

        List<GameObject> allSceneItems = new List<GameObject>();
        allSceneItems.AddRange(sceneItems);
        allSceneItems.AddRange(chestItems);

        foreach (GameObject item in allSceneItems)
        {
            if (item == null) continue;

            string itemKey = GetItemKey(item);

            if (itemsDictionary.ContainsKey(itemKey))
            {
                ItemData itemData = itemsDictionary[itemKey];

                SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && itemSprites != null && itemData.spriteIndex < itemSprites.Length)
                {
                    spriteRenderer.sprite = itemSprites[itemData.spriteIndex];
                }

                Unboxing parentChest = item.GetComponentInParent<Unboxing>();

                if (parentChest != null)
                {
                    if (parentChest.IsOpened && !itemData.isCollected && !itemData.isInInventory)
                    {
                        item.SetActive(true);
                        if (spriteRenderer != null) spriteRenderer.enabled = true;
                    }
                    else
                    {
                        item.SetActive(false);
                        if (spriteRenderer != null) spriteRenderer.enabled = false;
                    }
                }
                else
                {
                    if (itemData.isCollected || itemData.isInInventory)
                    {
                        item.SetActive(false);
                    }
                }
            }
        }
    }

    public string GetItemKey(ItemData itemData)
    {
        if (!string.IsNullOrEmpty(itemData.parentChestName))
        {
            return itemData.parentChestName + "_" + itemData.itemName;
        }
        return itemData.itemName;
    }

    private string GetItemKey(GameObject item)
    {
        Unboxing parentChest = item.GetComponentInParent<Unboxing>();
        if (parentChest != null)
        {
            return parentChest.gameObject.name + "_" + item.name;
        }
        return item.name;
    }

    public bool IsItemTarget(GameObject item)
    {
        string itemKey = GetItemKey(item);

        if (itemsDictionary.ContainsKey(itemKey))
        {
            return itemsDictionary[itemKey].isTargetItem;
        }
        return false;
    }

    public void MarkItemAsCollected(GameObject item)
    {
        string itemKey = GetItemKey(item);

        if (itemsDictionary.ContainsKey(itemKey))
        {
            ItemData itemData = itemsDictionary[itemKey];
            itemData.isCollected = true;

            if (itemData.isTargetItem)
            {
                UpdateTargetFrames();
            }
        }
    }

    public int GetTotalTargetItemsCount()
    {
        return allItemsData.Count(item => item.isTargetItem);
    }

    public int GetCollectedTargetItemsCount()
    {
        return allItemsData.Count(item => item.isTargetItem && item.isCollected);
    }

    public void SaveItemsStateToGameState(GameStateManager gameState, string sceneName)
    {
        var sceneItems = allItemsData.Where(item => item.sceneName == sceneName).ToList();

        if (!gameState.scenesState.ContainsKey(sceneName))
        {
            gameState.scenesState[sceneName] = new GameStateManager.SceneState();
        }

        var sceneState = gameState.scenesState[sceneName];

        foreach (var item in sceneItems)
        {
            string key = GetItemKey(item);
            sceneState.items[key] = !item.isCollected;
        }

        SaveInventoryState(gameState, sceneName);
    }

    public void RestoreItemsStateFromGameState(GameStateManager gameState, string sceneName)
    {
        if (gameState.scenesState.ContainsKey(sceneName))
        {
            var sceneState = gameState.scenesState[sceneName];

            foreach (var itemData in allItemsData.Where(i => i.sceneName == sceneName).ToList())
            {
                string key = GetItemKey(itemData);
                if (sceneState.items.ContainsKey(key))
                {
                    itemData.isCollected = !sceneState.items[key];
                }
            }

            RestoreInventoryState(gameState, sceneName);
        }
    }

    private void LoadSpritesFromResources()
    {
        itemSprites = Resources.LoadAll<Sprite>(spritesPath);

        if (isInitialized)
        {
            UpdateTargetFrames();
        }
    }

    public void ActivateAndSetupChestItems()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        Unboxing[] chests = FindObjectsByType<Unboxing>(FindObjectsSortMode.None);
        foreach (var chest in chests)
        {
            foreach (Transform child in chest.transform)
            {
                if (child.CompareTag("ChestItem"))
                {
                    GameObject item = child.gameObject;
                    string itemKey = GetItemKey(item);

                    if (itemsDictionary.ContainsKey(itemKey))
                    {
                        ItemData itemData = itemsDictionary[itemKey];
                        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();

                        if (spriteRenderer != null && itemSprites != null && itemData.spriteIndex < itemSprites.Length)
                        {
                            spriteRenderer.sprite = itemSprites[itemData.spriteIndex];

                            if (!chest.IsOpened || itemData.isCollected || itemData.isInInventory)
                            {
                                item.SetActive(false);
                                spriteRenderer.enabled = false;
                            }
                            else
                            {
                                item.SetActive(true);
                                spriteRenderer.enabled = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public void ResetForNewGame()
    {
        allItemsData.Clear();
        itemsDictionary.Clear();
        inventoryItems.Clear();
        isInitialized = false;

        level1SpriteIndices.Clear();
        level2SpriteIndices.Clear();
        targetAllIndices.Clear();
        targetIndexToFrameMap.Clear();
    }
}