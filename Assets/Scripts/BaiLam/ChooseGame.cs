using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseGame : MonoBehaviour,IMenu
{
    [Header("Buttons")]
    [SerializeField] private Button btnModeLastGame; // Ví dụ: Chế độ cũ
    [SerializeField] private Button btnModeNewGame; // Ví dụ: Chế độ mới 

    private UIMainManager m_mngr;

    private void Awake()
    {
        
        btnModeLastGame.onClick.AddListener(OnClickMode1);
        btnModeNewGame.onClick.AddListener(OnClickMode2);
    }

    private void OnDestroy()
    {
        btnModeLastGame.onClick.RemoveAllListeners();
        btnModeNewGame.onClick.RemoveAllListeners();
    }



    private void OnClickMode1()
    {
       
        m_mngr.PlayGame(GameManager.eLevelMode.MOVES);
    }

    private void OnClickMode2()
    {
        m_mngr.PlayGame(GameManager.eLevelMode.TIMER);
    }

    

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
