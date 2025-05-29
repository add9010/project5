using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public string description;
    public EquipmentType type;
    public string iconPath;

    [System.NonSerialized]
    public Sprite icon;  // 런타임에 로드됨

    public bool canEquip => type != EquipmentType.None;
}
public enum EquipmentType
{
    None,
    Weapon,
    Armor,
    Accessory
}
