using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public InvisibleBlock invisibleBlock;
    public int rows = 5;
    public int columns = 8;
    public float blockSize = 1f;
    public GameObject[] blockPrefabs;
    public int A = 4;
    public int B = 6;
    public int C = 8;
    private Block[,] grid;
    private BlockPool blockPool;
    private bool isProcessing = false;

    private void Awake()
    {
        // Singleton yapýsý: Sadece bir GameController örneði olmalý
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // BlockPool'u baþlat ve grid'i oluþtur
        blockPool = BlockPool.Instance;
        if (blockPool == null)
        {
            return;
        }

        blockPool.blockPrefabs = blockPrefabs;
        blockPool.poolSizePerPrefab = 20;
        blockPool.SetActiveColorCount(blockPrefabs.Length);

        SetColorCount(4);
        InitializeGrid();
        StartCoroutine(CheckAndResolveDeadlockOnStart());
        ResetAndCheckGroups();
    }

    // Grid boyutunu ayarla
    public void SetGridSize(int newRows, int newColumns)
    {
        if (newRows < 2 || newRows > 10 || newColumns < 2 || newColumns > 12)
        {
            return;
        }

        rows = newRows;
        columns = newColumns;

        if (invisibleBlock != null)
        {
            invisibleBlock.UpdateInvisibleBlock(rows, columns);
        }

        InitializeGrid();
        StartCoroutine(CheckAndResolveDeadlockOnStart());
    }

    // Renk sayýsýný ayarla
    public void SetColorCount(int newColorCount)
    {
        if (newColorCount < 1 || newColorCount > blockPrefabs.Length)
        {
            return;
        }

        ClearOldColorsFromGridAndPool(newColorCount);
        blockPool.SetActiveColorCount(newColorCount);
        InitializeGrid();
    }

    // Eski renklerin bloklarýný grid ve havuzdan temizle
    private void ClearOldColorsFromGridAndPool(int newColorCount)
    {
        if (grid != null)
        {
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    if (grid[row, col] != null)
                    {
                        blockPool.ReturnBlock(grid[row, col].gameObject);
                        grid[row, col] = null;
                    }
                }
            }
        }

        for (int i = newColorCount; i < blockPrefabs.Length; i++)
        {
            blockPool.ClearBlocksByColorID(i);
        }
    }

    // Grid'i baþlat ve bloklarý yerleþtir
    private void InitializeGrid()
    {
        ClearGrid();
        grid = new Block[rows, columns];

        float startX = -(columns / 2f) * blockSize + blockSize / 2f;
        float startY = -(rows / 2f) * blockSize + blockSize / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                CreateBlockAtPositionWithoutAnimation(row, col, startX, startY);
            }
        }

        if (invisibleBlock != null)
        {
            invisibleBlock.UpdateInvisibleBlock(rows, columns);
        }
    }

    // Grid'deki tüm bloklarý temizle
    private void ClearGrid()
    {
        if (grid != null)
        {
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    if (grid[row, col] != null)
                    {
                        blockPool.ReturnBlock(grid[row, col].gameObject);
                        grid[row, col] = null;
                    }
                }
            }
        }
    }

    // Animasyon olmadan belirli bir pozisyona blok oluþtur
    private void CreateBlockAtPositionWithoutAnimation(int row, int col, float startX, float startY)
    {
        int randomIndex = UnityEngine.Random.Range(0, blockPool.ActiveColorCount);
        Vector3 targetPosition = new Vector3(startX + col * blockSize, startY + (rows - 1 - row) * blockSize, 0);

        GameObject blockObject = blockPool.GetBlock(randomIndex);
        if (blockObject != null)
        {
            Block block = blockObject.GetComponent<Block>();
            block.colorID = randomIndex;
            block.row = row;
            block.col = col;

            grid[row, col] = block;
            blockObject.transform.position = targetPosition;
            ResetAndCheckGroups();
        }
    }

    // Deadlock (çýkmaz) durumunu kontrol et
    private bool IsDeadlock()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Block currentBlock = grid[row, col];
                if (currentBlock == null) continue;

                if (col < columns - 1)
                {
                    Block rightBlock = grid[row, col + 1];
                    if (rightBlock != null && rightBlock.colorID == currentBlock.colorID)
                        return false;
                }

                if (row < rows - 1)
                {
                    Block bottomBlock = grid[row + 1, col];
                    if (bottomBlock != null && bottomBlock.colorID == currentBlock.colorID)
                        return false;
                }
            }
        }
        return true;
    }

    // Oyun baþladýðýnda deadlock kontrolü yap ve çöz
    private IEnumerator CheckAndResolveDeadlockOnStart()
    {
        yield return new WaitForSeconds(0.1f);

        if (IsDeadlock())
        {
            yield return StartCoroutine(ResolveDeadlockSmart());
        }
        ResetAndCheckGroups();
    }

    // Akýllý deadlock çözümü: Bloklarý birbirine yaklaþtýrarak çöz
    private IEnumerator ResolveDeadlockSmart()
    {
        int maxIterations = 100;
        int currentIteration = 0;

        while (IsDeadlock() && currentIteration < maxIterations)
        {
            currentIteration++;

            Block randomBlock = GetRandomBlock();
            if (randomBlock == null) break;

            Block nearestSameColorBlock = FindNearestSameColorBlock(randomBlock);
            if (nearestSameColorBlock == null) break;

            yield return StartCoroutine(MoveBlocksCloserWithAnimation(randomBlock, nearestSameColorBlock));
        }

        if (IsDeadlock())
        {
            yield return StartCoroutine(ResolveDeadlockByReplacingBlock());
        }
    }

    // Bloklarý animasyonla birbirine yaklaþtýr
    private IEnumerator MoveBlocksCloserWithAnimation(Block block1, Block block2)
    {
        Block blockToSwap = FindAdjacentBlock(block2);
        if (blockToSwap == null) yield break;

        Vector3 block1Target = blockToSwap.transform.position;
        Vector3 blockToSwapTarget = block1.transform.position;

        yield return StartCoroutine(MoveBlockToPositionWithEasing(block1, block1Target, 0.15f));
        yield return StartCoroutine(MoveBlockToPositionWithEasing(blockToSwap, blockToSwapTarget, 0.15f));

        grid[block1.row, block1.col] = blockToSwap;
        grid[blockToSwap.row, blockToSwap.col] = block1;

        int tempRow = block1.row;
        int tempCol = block1.col;
        block1.row = blockToSwap.row;
        block1.col = blockToSwap.col;
        blockToSwap.row = tempRow;
        blockToSwap.col = tempCol;
    }

    // Bloðu belirli bir pozisyona hareket ettir (easing ile)
    private IEnumerator MoveBlockToPositionWithEasing(Block block, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = block.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = Mathf.SmoothStep(0f, 1f, t);
            block.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        block.transform.position = targetPosition;
    }

    // Rastgele bir blok seç
    private Block GetRandomBlock()
    {
        List<Block> blocks = new List<Block>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (grid[row, col] != null)
                {
                    blocks.Add(grid[row, col]);
                }
            }
        }

        if (blocks.Count == 0) return null;
        return blocks[UnityEngine.Random.Range(0, blocks.Count)];
    }

    // Ayný renkteki en yakýn bloðu bul
    private Block FindNearestSameColorBlock(Block targetBlock)
    {
        Block nearestBlock = null;
        float nearestDistance = float.MaxValue;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Block currentBlock = grid[row, col];
                if (currentBlock != null && currentBlock.colorID == targetBlock.colorID && currentBlock != targetBlock)
                {
                    float distance = Mathf.Abs(targetBlock.row - row) + Mathf.Abs(targetBlock.col - col);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestBlock = currentBlock;
                    }
                }
            }
        }

        return nearestBlock;
    }

    // Yanýndaki bir bloðu bul
    private Block FindAdjacentBlock(Block block)
    {
        List<Block> adjacentBlocks = new List<Block>();

        if (block.row > 0 && grid[block.row - 1, block.col] != null)
            adjacentBlocks.Add(grid[block.row - 1, block.col]);
        if (block.row < rows - 1 && grid[block.row + 1, block.col] != null)
            adjacentBlocks.Add(grid[block.row + 1, block.col]);
        if (block.col > 0 && grid[block.row, block.col - 1] != null)
            adjacentBlocks.Add(grid[block.row, block.col - 1]);
        if (block.col < columns - 1 && grid[block.row, block.col + 1] != null)
            adjacentBlocks.Add(grid[block.row, block.col + 1]);

        if (adjacentBlocks.Count == 0) return null;
        return adjacentBlocks[UnityEngine.Random.Range(0, adjacentBlocks.Count)];
    }

    // Deadlock çözümü için bir bloðu deðiþtir
    private IEnumerator ResolveDeadlockByReplacingBlock()
    {
        Block randomBlock = GetRandomBlock();
        if (randomBlock == null) yield break;

        Block adjacentBlock = FindAdjacentBlock(randomBlock);
        if (adjacentBlock == null) yield break;

        ReplaceBlockWithColor(adjacentBlock, randomBlock.colorID);
        yield return new WaitForSeconds(0.3f);

        if (IsDeadlock())
        {
        }
    }

    // Bloðu belirli bir renkle deðiþtir
    private void ReplaceBlockWithColor(Block block, int newColorID)
    {
        if (block == null) return;

        blockPool.ReturnBlock(block.gameObject);

        GameObject newBlockObject = blockPool.GetBlock(newColorID);
        if (newBlockObject != null)
        {
            Block newBlock = newBlockObject.GetComponent<Block>();
            newBlock.row = block.row;
            newBlock.col = block.col;
            newBlock.colorID = newColorID;

            grid[block.row, block.col] = newBlock;
            newBlockObject.transform.position = GridToWorldPosition(block.row, block.col);
        }
    }

    // Grid pozisyonunu dünya pozisyonuna çevir
    private Vector3 GridToWorldPosition(int row, int col)
    {
        float startX = -(columns / 2f) * blockSize + blockSize / 2f;
        float startY = -(rows / 2f) * blockSize + blockSize / 2f;
        return new Vector3(startX + col * blockSize, startY + (rows - 1 - row) * blockSize, 0);
    }

    // Blok týklama iþlemini iþle
    public void HandleBlockClick(Block clickedBlock)
    {
        if (isProcessing) return;

        List<Block> group = FindGroup(clickedBlock);

        if (group.Count >= 2)
        {
            StartCoroutine(DestroyAndRefill(group));
        }
    }

    // Grup bul (BFS kullanarak)
    private List<Block> FindGroup(Block startBlock)
    {
        List<Block> group = new List<Block>();
        bool[,] visited = new bool[rows, columns];
        Queue<Block> queue = new Queue<Block>();

        queue.Enqueue(startBlock);
        visited[startBlock.row, startBlock.col] = true;

        while (queue.Count > 0)
        {
            Block currentBlock = queue.Dequeue();
            group.Add(currentBlock);

            foreach (Vector2Int direction in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                int newRow = currentBlock.row + direction.y;
                int newCol = currentBlock.col + direction.x;

                if (IsValidPosition(newRow, newCol) &&
                    !visited[newRow, newCol] &&
                    grid[newRow, newCol] != null &&
                    grid[newRow, newCol].colorID == currentBlock.colorID)
                {
                    visited[newRow, newCol] = true;
                    queue.Enqueue(grid[newRow, newCol]);
                }
            }
        }

        return group;
    }

    // Bloklarý yok et ve boþluklarý doldur
    private IEnumerator DestroyAndRefill(List<Block> group)
    {
        isProcessing = true;

        DestroyBlocks(group);
        yield return StartCoroutine(FillEmptySpaces());

        if (IsDeadlock())
        {
            yield return StartCoroutine(ResolveDeadlockSmart());
        }

        ResetAndCheckGroups();
        isProcessing = false;
    }

    // Bloklarý yok et
    private void DestroyBlocks(List<Block> group)
    {
        foreach (Block block in group)
        {
            grid[block.row, block.col] = null;
            blockPool.ReturnBlock(block.gameObject);
        }
    }

    // Boþluklarý doldur
    private IEnumerator FillEmptySpaces()
    {
        List<Coroutine> moveCoroutines = new List<Coroutine>();

        for (int col = 0; col < columns; col++)
        {
            HandleFallingBlocksInColumn(col, moveCoroutines);
        }

        for (int col = 0; col < columns; col++)
        {
            AddNewBlocksToColumn(col, moveCoroutines);
        }

        foreach (Coroutine coroutine in moveCoroutines)
        {
            yield return coroutine;
        }
    }

    // Sütundaki bloklarý aþaðý kaydýr
    private void HandleFallingBlocksInColumn(int col, List<Coroutine> moveCoroutines)
    {
        for (int row = rows - 1; row >= 0; row--)
        {
            if (grid[row, col] == null)
            {
                for (int aboveRow = row - 1; aboveRow >= 0; aboveRow--)
                {
                    if (grid[aboveRow, col] != null)
                    {
                        Block block = grid[aboveRow, col];
                        grid[aboveRow, col] = null;
                        grid[row, col] = block;
                        block.row = row;

                        Vector3 targetPosition = GridToWorldPosition(row, col);
                        moveCoroutines.Add(StartCoroutine(MoveBlockToPositionWithEasing(block, targetPosition, 0.3f)));
                        break;
                    }
                }
            }
        }
    }

    // Sütuna yeni bloklar ekler 
    private void AddNewBlocksToColumn(int col, List<Coroutine> moveCoroutines)
    {
        int blocksToCreate = 0;

        for (int row = 0; row < rows; row++)
        {
            if (grid[row, col] == null)
            {
                blocksToCreate++;
            }
        }

        for (int i = 0; i < blocksToCreate; i++)
        {
            int row = blocksToCreate - 1 - i;
            float startX = -(columns / 2f) * blockSize + blockSize / 2f;
            float startY = (rows * blockSize) / 2f + (i * blockSize * 1.5f);

            CreateBlockAtPosition(row, col, startX, startY, moveCoroutines);
        }
    }

    // Yeni blok oluþtur ve animasyonla yerleþtir
    private void CreateBlockAtPosition(int row, int col, float startX, float startY, List<Coroutine> moveCoroutines)
    {
        int randomIndex = UnityEngine.Random.Range(0, blockPool.ActiveColorCount);
        Vector3 targetPosition = GridToWorldPosition(row, col);

        GameObject blockObject = blockPool.GetBlock(randomIndex);
        if (blockObject != null)
        {
            blockObject.SetActive(true);

            Block block = blockObject.GetComponent<Block>();
            block.colorID = randomIndex;
            block.row = row;
            block.col = col;

            grid[row, col] = block;

            float startHeight = startY + (blockSize / 2);
            Vector3 startPosition = new Vector3(targetPosition.x, startHeight, targetPosition.z);

            blockObject.transform.position = startPosition;
            moveCoroutines.Add(StartCoroutine(MoveBlockToPositionWithEasing(block, targetPosition, 0.5f)));
        }
    }

    // Pozisyonun geçerli olup olmadýðýný kontrol eder
    private bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < columns;
    }

    // Gruplarý kontrol et ve ikonlarý günceller
    private void ResetAndCheckGroups()
    {
        HashSet<Block> processedBlocks = new HashSet<Block>();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Block block = grid[row, col];
                if (block != null && !processedBlocks.Contains(block))
                {
                    List<Block> group = FindGroup(block);

                    if (group.Count >= 2)
                    {
                        UpdateIconsForGroup(group);

                        foreach (Block groupedBlock in group)
                        {
                            processedBlocks.Add(groupedBlock);
                        }
                    }
                    else
                    {
                        block.ResetIcon();
                    }
                }
            }
        }
    }

    // Grup ikonlarýný güncelle
    private void UpdateIconsForGroup(List<Block> group)
    {
        foreach (Block block in group)
        {
            block.UpdateIcon(group.Count);
        }
    }

    // A deðerini ayarla
    public void SetAValue(int newA)
    {
        if (newA > 0)
        {
            A = newA;
        }
    }

    // B deðerini ayarla
    public void SetBValue(int newB)
    {
        if (newB > 0)
        {
            B = newB;
        }
    }

    // C deðerini ayarla
    public void SetCValue(int newC)
    {
        if (newC > 0)
        {
            C = newC;
        }
    }
}