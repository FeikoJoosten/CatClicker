using System;
using UnityEngine;

public enum ItemType {
	AutoCP,
	TapIncrease,
	TapIncreasePercentage,
	None
}

[CreateAssetMenu(menuName = "ScriptableObjects/ItemData")]
public class IItemData : ScriptableObject
{
	[Header("Purchase info")]
	[SerializeField] private float defaultItemCost = 0;
	[SerializeField] private float defaultItemCostMultiplier = 0;
	[SerializeField] private float defaultValueToAddOnPurchase = 0;
	[SerializeField] private ItemType itemType = ItemType.None;

	[Header("Item info")]
	[SerializeField] private string defaultItemName = string.Empty;
	[SerializeField] private string defaultItemDescription = string.Empty;
	[SerializeField] private Sprite itemIcon = null;

	public float ItemCost { get; private set; }
	public float ItemCostMultiplier { get; private set; }
	public float ValueToAddOnPurchase { get; private set; }
	public ItemType ItemType { get; private set; }
	public string ItemName { get; private set; }
	public string ItemDescription { get; private set; }
	public Sprite ItemIcon { get { return itemIcon;} }
	
	public delegate void OnFinishedGettingRemoteSettings();
	public OnFinishedGettingRemoteSettings onFinishedGettingRemoteSettings;

	public void Initialize()
	{
		ItemCost = defaultItemCost;
		ItemCostMultiplier = defaultItemCostMultiplier;
		ValueToAddOnPurchase = defaultValueToAddOnPurchase;
		ItemType = itemType;

		ItemName = defaultItemName;
		ItemDescription = defaultItemDescription;

		RemoteSettings.Updated += new RemoteSettings.UpdatedEventHandler(HandleRemoteUpdate);
	}

	public void HandleRemoteUpdate()
	{
		ItemCost = RemoteSettings.GetFloat(name + "ItemCost", defaultItemCost);
		ItemCostMultiplier = RemoteSettings.GetFloat(name + "ItemCostMultiplier", defaultItemCostMultiplier);
		ValueToAddOnPurchase = RemoteSettings.GetFloat(name + "ValueToAddOnPurchase", defaultValueToAddOnPurchase);
		ItemType = (ItemType) Enum.Parse(typeof(ItemType), RemoteSettings.GetString(name + "ItemType", ItemType.None.ToString()));

		ItemName = RemoteSettings.GetString(name + "ItemName", defaultItemName);
		ItemDescription = RemoteSettings.GetString(name + "ItemDescription", defaultItemDescription);

		if(onFinishedGettingRemoteSettings != null)
			onFinishedGettingRemoteSettings();
		else
			Debug.Log("The fuck:" + name);
	}
}
