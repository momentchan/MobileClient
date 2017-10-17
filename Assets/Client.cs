using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour {

	GUIStyle buttonStyle, textStyle;
	bool send = false;


	bool gyroEnabled;
	bool accEnabled;
	Gyroscope gyro;
	Vector3 gyroValue;
	Vector3 accValue;


	void Start () {
		gyroEnabled = EnableGyro ();
		accEnabled = SystemInfo.supportsAccelerometer;
	}
	

	void Update () 
	{
		if(gyroEnabled)
			gyroValue = gyro.attitude.eulerAngles;
		
		if (accEnabled)
			accValue = Input.acceleration;

		MasterServer.RequestHostList("GYROCONNECT");
		if(send)
			GetComponent<NetworkView>().RPC("MobileSensing", RPCMode.Others, gyroValue, accValue);
	}

	void OnGUI()
	{
		// Style
		buttonStyle = new GUIStyle ("button");
		buttonStyle.fontSize = 50;
		GUI.skin.button = buttonStyle;
		textStyle = new GUIStyle ();
		textStyle.fontSize = 50;
		textStyle.normal.textColor = Color.white;

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Gyrostate:", textStyle);
		GUILayout.Space(5);
		GUILayout.Label (gyroEnabled.ToString(), textStyle);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Accstate:", textStyle);
		GUILayout.Space(5);
		GUILayout.Label (accEnabled.ToString(), textStyle);
		GUILayout.EndHorizontal();

		GUI.Label (new Rect (Screen.width/8, Screen.height/2, 100, 50), "Gyro Value: " + gyroValue.ToString(), textStyle);
		GUI.Label (new Rect (Screen.width/8, Screen.height/2+50, 100, 50), "Acc   Value: " + accValue.ToString(), textStyle);

	

		HostData[] data = MasterServer.PollHostList();
		foreach (HostData element in data)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(50);
			var name = element.gameName;
			GUILayout.Label(name, textStyle);
			GUILayout.Space(5);

			string hostInfo;
			hostInfo = "[";
			foreach (string host in element.ip)
				hostInfo = hostInfo + host + ":" + element.port + " ";
			hostInfo = hostInfo + "]";

			GUILayout.Space(5);
			GUILayout.Label(hostInfo, textStyle);
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Connect", buttonStyle))
			{
				Network.Connect(element);
				send = true;
			}
		}

	}

	bool EnableGyro()
	{
		if (SystemInfo.supportsGyroscope) 
		{
			gyro = Input.gyro;
			gyro.enabled = true;
			return true;
		}
		return false;
	}

	

	[RPC]
	private void MobileSensing(Vector3 gyro, Vector3 acc)
	{
		gyroValue = gyro;
		accValue  = acc;
	}
}

