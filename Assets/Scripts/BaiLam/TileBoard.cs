using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard
{
    private int boardSizeX;
    private int boardSizeY;
    private Cell[,] m_cells;
    private Transform m_root;


    public TileBoard(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;
        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;
        m_cells = new Cell[boardSizeX, boardSizeY];

        CreateBoard();
    }

    private void CreateBoard()
    {
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
               
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);
                m_cells[x, y] = cell;
            }
        }
    }

    // --- Sinh Item theo bộ 3 ---
    public void FillTriplets()
    {
        int totalCells = boardSizeX * boardSizeY;
        List<NormalItem.eNormalType> spawnPool = new List<NormalItem.eNormalType>();

        // Tính số lượng bộ 3
        int tripletsCount = totalCells / 3;

        for (int i = 0; i < tripletsCount; i++)
        {
            NormalItem.eNormalType type = Utils.GetRandomNormalType();
            spawnPool.Add(type);
            spawnPool.Add(type);
            spawnPool.Add(type);
        }

        // Fill nốt phần dư 
        while (spawnPool.Count < totalCells)
        {
            spawnPool.Add(Utils.GetRandomNormalType());
        }

        // Trộn ngẫu nhiên
        spawnPool = ShuffleList(spawnPool);

        int index = 0;
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (index >= spawnPool.Count) break;

                Cell cell = m_cells[x, y];

                // Tạo item
                NormalItem item = new NormalItem();
                item.SetType(spawnPool[index]);
                item.SetView();
                item.SetViewRoot(m_root);

                cell.Assign(item);
                cell.ApplyItemPosition(false);
                index++;
            }
        }
    }

    private List<T> ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int rnd = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[rnd];
            list[rnd] = temp;
        }
        return list;
    }

    public void Clear()
    {
        if (m_cells == null) return;
        foreach (var cell in m_cells)
        {
            if (cell != null)
            {
                cell.Clear();
                
                if (cell.gameObject != null)
                {
                    GameObject.Destroy(cell.gameObject);
                }
            }
        }
    }
}