using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ItemObject : StoreItem
{
	[SerializeField] private Image itemIconImage = null;
	[SerializeField] private Text itemNameText = null;
	[SerializeField] private Text itemDescriptionText = null;
	[SerializeField] private Text purchaseCountText = null;
	[SerializeField] private Text itemCostText = null;

	private CultureInfo systemLanguage;

	private void Start()
	{
		systemLanguage = ScreenManager.GetCultureDefualt(Application.systemLanguage);

		onPurchase += OnPurchased;
		ProcessSavedInformation();
	}

	public override void UpdateUI()
	{
		itemIconImage.sprite = storeInformation.ItemIcon;
		itemNameText.text = storeInformation.ItemName;
		itemDescriptionText.text = storeInformation.ItemDescription;

		purchaseCountText.text = GameManager.GetInstance().GetStoreManager().GetSavedInformation(this).ToString("N0", systemLanguage);
		itemCostText.text = Mathf.Round(purchaseCost).ToString("N1", systemLanguage);
	}

	private void OnPurchased()
	{
		switch (storeInformation.ItemType)
		{
			case ItemType.AutoCP:
				GameManager.GetInstance().GetScoreManager().AddAutoCPS(storeInformation.ValueToAddOnPurchase);
			break;
			case ItemType.TapIncrease:
				FindObjectOfType<ClickObject>().AddBaseValue(storeInformation.ValueToAddOnPurchase);
			break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}
