using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Shop/Item")]
public class ItemStats : ScriptableObject
{
    public int itemID;
    public string itemName;
    public int price;
    public string description;
    public Sprite icon;
    public ItemType type;

    public enum ItemType
    {
        Consumable,
        Equipment,
        Talisman,
        Other
    }
}
