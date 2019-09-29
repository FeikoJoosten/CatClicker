using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static long ElapsedTime()
	{
		if (Application.platform != RuntimePlatform.Android) return 0;
		AndroidJavaClass systemClock = new AndroidJavaClass("android.os.SystemClock");
		return systemClock.CallStatic<long>("elapsedRealtime");
	}
}
