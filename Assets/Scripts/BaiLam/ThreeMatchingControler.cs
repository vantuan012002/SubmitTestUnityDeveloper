using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ThreeMatchingControler : MonoBehaviour
{
    public bool IsBusy { get; private set; }

    
    private TileBoard m_tileBoard;

    private GameManager m_gameManager;
    private GameSettings m_gameSettings;
    private Camera m_cam;
    private BottomBarControler m_bottomBar;

    private int m_totalItemsOnBoard;
    private bool m_gameOver;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;
        m_gameSettings = gameSettings;
        m_bottomBar = new BottomBarControler();
        m_cam = Camera.main;

        // Tìm BottomBar trong scene
        m_bottomBar.OnLoseGame += OnLose;  
        if (m_bottomBar != null)
        {
            m_bottomBar.OnLoseGame += OnLose;
        }

        // Khởi tạo TileBoard (Bàn cờ kiểu mới)
        m_tileBoard = new TileBoard(this.transform, gameSettings);
        m_tileBoard.FillTriplets(); // Gọi hàm sinh bộ 3

        m_totalItemsOnBoard = gameSettings.BoardSizeX * gameSettings.BoardSizeY;

        m_gameManager.StateChangedAction += OnStateChange;
    }

    private void OnStateChange(GameManager.eStateGame state)
    {
        if (state == GameManager.eStateGame.GAME_OVER) m_gameOver = true;
    }

    private void OnLose()
    {
        m_gameOver = true;
        Debug.Log("Game Over: Khay đầy!");
        m_gameManager.GameOver();
    }

    public void Update()
    {
        if (m_gameOver || m_gameManager.State != GameManager.eStateGame.GAME_STARTED) return;
        if (m_bottomBar != null && m_bottomBar.IsPressing) return; // Chặn khi đang bay

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            Cell cell = hit.collider.GetComponent<Cell>();
            if (cell != null && !cell.IsEmpty && !m_bottomBar.IsFull)
            {
                NormalItem item = cell.Item as NormalItem;
                cell.Free(); // Gỡ khỏi bàn cờ

                // Đưa xuống khay
                m_bottomBar.AddItem(item, () => {
                    CheckWin();
                });
            }
        }
    }

    private void CheckWin()
    {
        m_totalItemsOnBoard--;
        if (m_totalItemsOnBoard <= 0)
        {
            m_gameOver = true;
            Debug.Log("YOU WIN!");
            m_gameManager.GameOver();
        }
    }

    private void OnDestroy()
    {
        if (m_bottomBar != null) m_bottomBar.OnLoseGame -= OnLose;
        if (m_tileBoard != null) m_tileBoard.Clear();
    }
}