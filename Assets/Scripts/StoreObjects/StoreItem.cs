using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItem : MonoBehaviour
{
	public IItemData storeInformation = null;
	public delegate void OnPurchase();
	public OnPurchase onPurchase;
	protected float purchaseCost = 0;

	public float PurchaseCose
	{
		get { return purchaseCost; }
	}

	private void Awake()
	{
		storeInformation.onFinishedGettingRemoteSettings += UpdateUI;
		GameManager.GetInstance().GetStoreManager().onCloudDataReceived += UpdateUI;
	}

	protected void ProcessSavedInformation()
	{
		storeInformation.Initialize();
		purchaseCost = storeInformation.ItemCost;

		int purchasedAmount = GameManager.GetInstance().GetStoreManager().GetSavedInformation(this);

		if (purchasedAmount <= 0) return;

		for (int i = 0; i < purchasedAmount; i++)
			PurchaseObject();
	}

	private void PurchaseObject()
	{
		IncreaseBaseCost();
		UpdateUI();

		onPurchase();
	}

	private void IncreaseBaseCost()
	{
		purchaseCost *= storeInformation.ItemCostMultiplier;
	}

	public void OnBuy()
	{
		if (GameManager.GetInstance().GetStoreManager().BuyItem(this))
		{
			PurchaseObject();
			return;
		}

		Debug.Log("Something went wrong when we tried to buy something.");
	}

	public virtual void UpdateUI() { }
}