using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour 
{	
	private void Start()
	{
		StartCoroutine(WaitForEndOfFrame());
	}

	private IEnumerator WaitForEndOfFrame()
	{
		yield return new WaitForEndOfFrame();

		gameObject.SetActive(false);
	}
}
