using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    // 보관 아이템 목록 (초기화 필수)
    public List<ItemData> storageItems = new List<ItemData>();

    // 장착 중인 아이템
    public ItemData equippedWeapon;
    public ItemData equippedArmor;
    public ItemData equippedAccessory;

    /// <summary>
    /// 선택한 아이템을 장착 슬롯으로 이동시키고,
    /// 기존 장착 아이템은 보관함으로 되돌립니다.
    /// </summary>
    public void EquipItem(ItemData item)
    {
        if (item == null) return;

        // 기존 장착 아이템 백업
        switch (item.type)
        {
            case EquipmentType.Weapon:
                if (equippedWeapon != null) storageItems.Add(equippedWeapon);
                equippedWeapon = item;
                break;
            case EquipmentType.Armor:
                if (equippedArmor != null) storageItems.Add(equippedArmor);
                equippedArmor = item;
                break;
            case EquipmentType.Accessory:
                if (equippedAccessory != null) storageItems.Add(equippedAccessory);
                equippedAccessory = item;
                break;
        }

        // 보관함에서 제거
        storageItems.Remove(item);
    }
}
