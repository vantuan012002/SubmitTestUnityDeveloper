using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class thuần C#, không kế thừa MonoBehaviour
public class BottomBarControler
{
    public int maxSlots = 7;
    public float spacing = 1.2f; // Khoảng cách giữa các slot

    // Vị trí cố định của thanh bar (Bạn có thể sửa số này để nâng/hạ độ cao)
    private float yPosition = -4.0f;

    private List<NormalItem> collectedItems = new List<NormalItem>();
    public Action OnLoseGame;
    public Action OnMatchSuccess;

    public bool IsPressing { get; private set; }
    public bool IsFull => collectedItems.Count >= maxSlots;

    private Vector3[] calculatedSlotPositions;

    // [THAY ĐỔI 1] Dùng Constructor thay cho Start
    public BottomBarControler()
    {
        CalculateSlotPositions();

        // Khoi tao o slot hien thi 
        CreateVisualSlot();

    }

    private void CalculateSlotPositions()
    {
        calculatedSlotPositions = new Vector3[maxSlots];

        // [THAY ĐỔI 2] Tính toán dựa trên tọa độ giả định (X=0 là giữa màn hình)
        float totalWidth = (maxSlots - 1) * spacing;
        float startX = 0 - (totalWidth / 2); // Căn giữa màn hình (0)

        for (int i = 0; i < maxSlots; i++)
        {
            float xPos = startX + (i * spacing);
            // Dùng yPosition cố định thay vì transform.position.y
            calculatedSlotPositions[i] = new Vector3(xPos, yPosition, 0);
        }
    }
    // Ham Tao 7 o slot hien thi tren bottom bar
    private void CreateVisualSlot()
    {
        GameObject slotPrefab = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slot = GameObject.Instantiate(slotPrefab);
            slot.transform.position = calculatedSlotPositions[i];
            var sprite = slot.GetComponent<SpriteRenderer>();
            if(sprite != null)
            {
                sprite.color = Color.gray; // Mau sac o slot la xam
                sprite.sortingOrder = 0; // Dat o duoi item
            }
        }
    }

    public void AddItem(NormalItem item, Action onComplete)
    {
        if (IsFull) return;
        IsPressing = true;

        collectedItems.Add(item);

        // SẮP XẾP: Gom các item cùng loại lại gần nhau
        collectedItems = collectedItems.OrderBy(x => x.ItemType).ToList();

        UpdateVisuals(() =>
        {
            CheckMatch();
            onComplete?.Invoke();
        });
    }

    private void UpdateVisuals(Action onVisualsComplete)
    {
        int moveCount = 0;
        for (int i = 0; i < collectedItems.Count; i++)
        {
            var item = collectedItems[i];
            Vector3 targetPos = calculatedSlotPositions[i];

            // item.View là Transform của GameObject thật, nên vẫn DOTween được bình thường
            item.View.DOMove(targetPos, 0.3f).OnComplete(() =>
            {
                moveCount++;
                if (moveCount >= collectedItems.Count) onVisualsComplete?.Invoke();
            });

            item.View.DOScale(Vector3.one * 0.8f, 0.3f);
            item.View.GetComponent<SpriteRenderer>().sortingOrder = 10;
            item.SetSortingLayerHigher();
        }

        //  Nếu list rỗng thì phải báo complete ngay
        if (collectedItems.Count == 0) onVisualsComplete?.Invoke();
    }

    private void CheckMatch()
    {
        var matchGroup = collectedItems.GroupBy(x => x.ItemType).FirstOrDefault(g => g.Count() >= 3);

        if (matchGroup != null)
        {
            // --- MATCH 3 ---
            var itemsToRemove = matchGroup.Take(3).ToList();
            foreach (var item in itemsToRemove)
            {
                collectedItems.Remove(item);
                item.ExplodeView();
            }
            OnMatchSuccess?.Invoke();
            UpdateVisuals(() => { IsPressing = false; });
        }
        else
        {
            // --- KHÔNG MATCH ---
            if (collectedItems.Count >= maxSlots)
            {
                OnLoseGame?.Invoke();
            }
            else
            {
                IsPressing = false;
            }
        }
    }

    
}