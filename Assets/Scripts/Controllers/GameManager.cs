using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    public enum eLevelMode
    {
        TIMER,
        MOVES
    }

    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        GAME_OVER,
    }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction(m_state);
        }
    }


    private GameSettings m_gameSettings;

    private ThreeMatchingControler m_threeMatchingControler;
    private BoardController m_boardController;

    private UIMainManager m_uiMenu;

    private LevelCondition m_levelCondition;

    private void Awake()
    {
        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

    void Update()
    {
        if (m_boardController != null) m_boardController.Update();
        if (m_threeMatchingControler != null) m_threeMatchingControler.Update();
    }


    internal void SetState(eStateGame state)
    {
        State = state;

        if (State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void LoadLevel(eLevelMode mode)
    {
        // test task2
        ClearLevel();
        //check task2
        if(mode == eLevelMode.TIMER)
        {
            m_threeMatchingControler = new GameObject("ThreeMatchingControler").AddComponent<ThreeMatchingControler>();
            m_threeMatchingControler.StartGame(this, m_gameSettings);

        }
        else//if mode is MOVES
        {
            m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
            m_boardController.StartGame(this, m_gameSettings);
        }





            

        if (mode == eLevelMode.MOVES)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelMoves>();
            m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), m_boardController);
        }
        else if (mode == eLevelMode.TIMER)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelTime>();
            m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), this);
        }

        m_levelCondition.ConditionCompleteEvent += GameOver;

        State = eStateGame.GAME_STARTED;
    }

    //test task2
    internal void ClearLevel()
    {
        // Dọn dẹp cả 2 loại controller
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }

        if (m_threeMatchingControler)
        {
            
            // Ở đây mình destroy gameobject chứa nó
            Destroy(m_threeMatchingControler.gameObject);
            m_threeMatchingControler = null;
        }

        
    }


    public void GameOver()
    {
        StartCoroutine(WaitBoardController());
    }

    //internal void ClearLevel()
    //{
    //    if (m_boardController)
    //    {
    //        m_boardController.Clear();
    //        Destroy(m_boardController.gameObject);
    //        m_boardController = null;
    //    }
    //}

    private IEnumerator WaitBoardController()
    {
        while (m_boardController != null && m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        //task 2
        while (m_threeMatchingControler != null && m_threeMatchingControler.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1f);

        State = eStateGame.GAME_OVER;
        if(m_uiMenu != null)
        {
            m_uiMenu.ShowMenu<UIPanelGameOver>();
        }
        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= GameOver;

            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }
}
