using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public Sprite icon;
    public string description;
    public EquipmentType type;
    public bool canEquip => type != EquipmentType.None;
}

public enum EquipmentType
{
    None,
    Weapon,
    Armor,
    Accessory
}
