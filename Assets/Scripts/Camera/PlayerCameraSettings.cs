using UnityEngine;

[CreateAssetMenu(fileName = "Player Camera Settings", menuName = "ScriptableObjects/PlayerCameraSettings")]
public class PlayerCameraSettings : ScriptableObject {
	[Header("Movement")]
	[Range(1, 40)] public float Sensitivity = 10;
	[Range(0, 1)] public float Smoothing = 0.25f;
	public bool InverY = true;
	
	[Header("Position")]
	public float Distance = 15;
	[Range(0.725f, 2)] public float Height = 1;
}
