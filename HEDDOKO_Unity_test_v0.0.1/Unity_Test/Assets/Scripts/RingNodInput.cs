using UnityEngine;
using System.Collections;

public class RingNodInput : MonoBehaviour 
{
	public GUIText guiText;
	
	#if UNITY_ANDROID
	AndroidJavaClass androidClass = null;
	AndroidJavaObject androidActivity = null;
	#endif

	// Use this for initialization
	void Start () 
	{
		#if UNITY_ANDROID
		AndroidJNI.AttachCurrentThread();
		androidClass = new AndroidJavaClass("com.Heddoko.HeddokoDemo.NodRingUnityPlugin");
		androidActivity = androidClass.GetStatic<AndroidJavaObject>("mContext");
		#endif
	}

	public void HelloFromAndroid(string dataReceived) 
	{
		Debug.Log("Received data from Android plugin: " + dataReceived);
		guiText.text = dataReceived;
	}

	// Update is called once per frame
	void Update () 
	{
		if (androidClass != null && androidActivity != null) 
		{
			androidActivity.Call("nonStaticMethod");
		}
	}
}

/*using UnityEngine;
using System.Collections;

public class NativeBridge : MonoBehaviour {
	
	public GUIText guiText;
	
	#if UNITY_ANDROID
	AndroidJavaClass androidClass;
	#endif
	
	void Start () 
	{
		#if UNITY_ANDROID
		AndroidJNI.AttachCurrentThread();
		androidClass = new AndroidJavaClass("com.hello.world.UnityBridge");
		#endif
	}
	
	#if UNITY_ANDROID
	public void checkMyIntOnJava(string message){
		if (message == "READY") {
			string myInt = androidClass.CallStatic<int>("getMyInt");
			guiText.text = "My Int: " + myInt;
		}
	}
	#endif
	
	void Update()
	{		
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log ("Pressed left click.");
			#if UNITY_ANDROID
			object[] args = new[] { Random.Range(1,100), this.gameObject.name, "checkMyIntOnJava" };
			androidClass.CallStatic("callbackToUnityMethod", args);
			#endif
		}
	}
}*/
