// using System.Collections;
// using UnityEngine;

// public class PlayerCamera : MonoBehaviour {
// 	//TODO: Impliment camera collision detection and avoidence: https://discussions.unity.com/t/complete-camera-collision-detection-third-person-games/593233
//     private Transform _player;
// 	private PlatformerController _platformer;
//     [SerializeField] private Transform _source;
//     // [SerializeField] private Transform this.transform;
// 	public CameraInput CamInputs;

//     [Header("Settings")]
// 	public PlayerCameraSettings Settings;
// 	//? The last known settings hash, 
// 	//? if this changes, we need to update all settings that are assigned on init
// 	private float _settingsHeightCache; 
	
//     [Header("Min/Max")]
//     [Range(-90, 90)] public float YMin;
//     [Range(-90, 90)] public float YMinDampStart; //? the value at which to start dampening the sensitivity and start zooming to offset distance
//     [Range(0, -12)] public float YMinDistanceOffset; //? The distance offset to lerp to based on how close currentY gets to YMin

//     [Space]

//     [Range(-90, 90)] public float YMax;
//     [Range(-90, 90)] public float YMaxDampStart; //? the value at which to start dampening the sensitivity and start zooming to offset distance
//     [Range(0, -12)] public float YMaxDistanceOffset; //? The distance offset to lerp to based on how close currentY gets to YMax
    
//     [Space]
// 	[Tooltip("The max amount of sensitivity reduction to allow\nFormula: Clamp(value, Sensitivity * MaxSensDamp, Sensitivity)")]
// 	[Range(0, 1)] public float MaxSensDamp = 0.25f;

// 	[Tooltip("The amount of compensation the X sensitivity will be influenced by a lower Y sens when dampening occurs\nFormula: Sensitivity - (Sensitivity - sensitivityY) * XSensCompensation")]
// 	[Range(0, 1)] public float XSensCompensation = 0.25f;

// 	[Header("Editor Settings")]
// 	public LayerMask OcclusionLayers;
// 	public float MinimumDistance = 2; //! Camera still breaks if too close to origin
// 	//TODO Try and see if its okey to just make every object (so also wall and ground) react the way like trees do when the cam is between them

//     // [Header("Debug")]
// 	[SerializeField] private Vector3 _velocity;
//     private Vector2 _mouseAxis;
//     private Vector3 _currentDirection;
//     private Quaternion _targetRotation;
//     private float _currentDistance;
// 	[Space]
//     private float _currentX;
//     private float _currentY;
// 	[Space]
//     private float _currentSens;
//     private float _distanceOffset = 0;

//     [Space]
//     private float _sensitivityY;
//     private float _currentSmoothing;


// 	[Header("Source Position")]
// 	[SerializeField, Range(0, 1)] private float _sourceFollowSpeed = 0.25f;
// 	[SerializeField, Range(0, 5f)] private float _sourceMaxDistance = 2; //? the max 100% to get to that will trigger max acceleration
// 	// [SerializeField, Range(0, 0.5f)] private float _sourceDistanceAcceleration = 0.05f; //? This value is added up to the speed in a precentage compared to how close the distance is to max distance
// 	[SerializeField] private AnimationCurve _sourceFollowAccelCurve;
// 	[SerializeField, ReadOnly] private float _sourceFollowSpeedCurrent; //? this is with acceleration
// 	[SerializeField, ReadOnly] private float _sourceFollowSpeedTarget; //? This is the target speed but its lerped towards to prevent jitters
// 	[Space]
// 	[Tooltip("The amount of height that needs to be traveled by the player for the camera to start following the player y.")]
// 	[SerializeField] private float _heightOffsetThreshold = 2; //? the lookat source needs to first reach past this value before its going to start to follow the player in y levels
// 	[SerializeField] private float _heightVelocityThreshold = 10; //? the lookat source needs to first reach past this value before its going to start to follow the player in y levels
// 	// [SerializeField, Range(0,1)] private float _heightCorrectionSpeed = 0.5f; //? once the threshold is reached, how fast should the source correct itself again towards the desired position
// 	[SerializeField, ReadOnly] private bool _heightOffsetCorrection = false; //? if this is true, the source is currently trying to return to the desired height

// 	[Header("Debug")]
// 	[SerializeField, ReadOnly] private float groundTimer;
// 	[SerializeField, ReadOnly] private float groundDistance;
// 	[SerializeField, ReadOnly] private float spinHeight;

// 	private void Awake() {
// 		_settingsHeightCache = Settings.Height;
// 		CamInputs = new CameraInput();
// 		_currentSmoothing = Settings.Smoothing;
// 		_sourceFollowSpeedCurrent = _sourceFollowSpeed;
// 	}
	
//     public IEnumerator TargetPlayer() {
//         yield return new WaitForSeconds(1.5f);
//         _player = Runtime.LocalPlayer.TargetHolder;
// 		_platformer = Runtime.LocalPlayer.controller;
// 		//? needs to be sphere ish object so that the distance doesnt jitter because its not a smooth surface it moves over
//         // _source = Runtime.LocalPlayer.controller.upCheckTransform; //? The orbit source object that the camera is moving around

// 		CamInputs.Enable();
//         Runtime.InMenu = false;
// 		UpdateSettings();
// 		if (Application.isEditor) {
// 			HandleCursorLock();

// 			//TODO teleport to 000
// 			// yield return new WaitUntil(() => Runtime.Initialized == true);
			
// 			yield return new WaitForSeconds(5f);
// 			_player.GetComponent<PlatformerController>().enabled = false;
// 			_player.position = Vector3.zero;
// 			yield return new WaitForSeconds(0.1f);
// 			_player.GetComponent<PlatformerController>().enabled = true;
// 		}
//     }
// 	private void UpdateSettings() {
// 		Debug.Log("Updating Camera Settings");
// 		_source.position = new Vector3(_source.position.x, _player.position.y + Settings.Height, _source.position.z);
// 	}
// 	private void OnDisable() {
// 		CamInputs.Disable();
// 	}

// 	private void FixedUpdate() {
// 		if(!Runtime.Initialized || !_source || !_player) return;
// 		if(UserInterfaceBehaviour.singleton.m_CanvasManager.activeCanvasType == OriginLabs.MenuType.Settings) return;
// 		if (Settings.Height != _settingsHeightCache) { //? update settings that only apply on init
// 			UpdateSettings();
// 		}

// 		_velocity = _platformer.controller.velocity;
// 		groundTimer = _platformer.groundTimer;
// 		groundDistance = _platformer.currentGroundDistance;
// 		spinHeight = _platformer.spinJumpHeight;
// 		UpdateSourcePosition();
// 	}

//     private void LateUpdate() {
//         if(!Runtime.Initialized || !_source || !_player) return;
		
// 		if (Input.GetKeyDown(KeyCode.Escape)) {
// 			Runtime.InMenu = !Runtime.InMenu;
// 			HandleCursorLock();
// 		}
// 		if(UserInterfaceBehaviour.singleton.m_CanvasManager.activeCanvasType == OriginLabs.MenuType.Settings) return;

// 		//TODO
// 		//! found out that cam movement speed changes based on frame rate 
// 		_mouseAxis = CamInputs.Main.MouseAxis.ReadValue<Vector2>();

//         _distanceOffset = 0;
// 		_currentSens = Settings.Sensitivity;
// 		_currentSmoothing = Settings.Smoothing; //? reset smoothing but allow the occlusion check to overwrite it again

// 		float dampPercent = CalculateYMinMax();
// 		if (dampPercent > 0 || (_currentY == YMin || _currentY == YMax)) {
// 			_sensitivityY = Mathf.Clamp(Settings.Sensitivity * dampPercent, Settings.Sensitivity * MaxSensDamp, Settings.Sensitivity);
// 			_currentSens -= (Settings.Sensitivity - _sensitivityY) * XSensCompensation; //? Adjust a little to compensate for the damp speed and not make it feel weird
// 		}
		
//         _currentX += _mouseAxis.x * _currentSens * Time.smoothDeltaTime;
//         _currentY += _mouseAxis.y * _sensitivityY * Time.smoothDeltaTime * ((Settings.InverY) ? -1 : 1);
//         _currentY = Mathf.Clamp(_currentY, YMin, YMax);

//         _currentDirection = new Vector3(0, 0, -(Settings.Distance + _distanceOffset));

//         _targetRotation = Quaternion.Euler(_currentY, _currentX, 0);
// 		Quaternion rotation = Quaternion.Lerp(this.transform.rotation, _targetRotation, _currentSmoothing); //? lerpt the rotation, not the position
		
// 		Vector3 newPos = CheckCameraOcclusion(_source.position + _targetRotation * _currentDirection, _source.position + rotation * _currentDirection);
		
// 		_currentDistance = Vector3.Distance(newPos, _source.position);

// 		this.transform.position = newPos; //? apply the actual position with the lerped rotation

//         this.transform.LookAt(_source);

//     	// Force Unity to apply changes immediately by setting the lockState again
//         // Cursor.lockState = Cursor.lockState;
//         // Cursor.visible = Cursor.visible;
//     }

// 	private void UpdateSourcePosition() {
// 		float zeroY = _player.position.y + Settings.Height;
// 		Vector3 zero = new Vector3(_player.position.x, zeroY, _player.position.z);
// 		Vector3 zeroXZ = new Vector3(zero.x, _source.position.y, zero.z);
		
// 		float distance = Vector3.Distance(_source.position, zero);
// 		float distanceXZ = Vector3.Distance(_source.position, zeroXZ);
// 		float distanceY = Mathf.Abs(_source.position.y - zeroY);
// 		Vector3 newPos = zero;
		
// 		if (distance > 0.01f) {
// 			_sourceFollowSpeedTarget = _sourceFollowSpeed + _sourceFollowAccelCurve.Evaluate(Mathf.Clamp01(distance / _sourceMaxDistance)) * (1 - _sourceFollowSpeed);
// 			// _sourceFollowSpeedCurrent = Mathf.Round(Mathf.LerpUnclamped(_sourceFollowSpeedCurrent, _sourceFollowSpeedTarget, 0.25f) * 1000) / 1000;
// 			_sourceFollowSpeedCurrent = Mathf.LerpUnclamped(_sourceFollowSpeedCurrent, _sourceFollowSpeedTarget, 0.25f);
// 		}
// 		else {
// 			_sourceFollowSpeedCurrent = _sourceFollowSpeed;
// 		}
		
// 		if (distanceXZ > 0.01f) {
// 			Vector3 newPosXZ = Vector3.Lerp(_source.position, zeroXZ, _sourceFollowSpeedCurrent);
// 			_source.position = new Vector3(newPosXZ.x, _source.position.y, newPosXZ.z);
// 		}
		
// 		if (!_heightOffsetCorrection && (
// 			distanceY > _heightOffsetThreshold ||
// 			(distanceY > 0.01f && _platformer.isGrounded && _platformer.groundTimer > 0.25f) ||
// 			(Mathf.Abs(_velocity.y) > _heightVelocityThreshold && _platformer.currentGroundDistance > 0.22f)
// 		)) {
// 			_heightOffsetCorrection = true;
// 		}

// 		if (_heightOffsetCorrection) {
// 			if (distanceY < 0.01f && _source.position.y > (zeroY - 0.1f) && _platformer.isGrounded) {
// 				_heightOffsetCorrection = false;
// 				_source.position = new Vector3(_source.position.x, zeroY, _source.position.z); //? ensure its properly reset
// 			}
// 			else {
// 				// float speed = _sourceFollowSpeed + (Mathf.Clamp01(yDistance / _sourceMaxDistance) * (1 - _sourceFollowSpeed));
// 				float newY = Mathf.Lerp(_source.position.y, zeroY, _sourceFollowSpeedCurrent);
// 				_source.position = new Vector3(_source.position.x, newY, _source.position.z);
// 			}
// 		}
// 	}

// 	private float CalculateYMinMax() {
// 		float dampPercent = 0;

// 		if (_currentY <= YMinDampStart) {
//             float curr = _currentY - YMin;
//             float steps = YMinDampStart - YMin;
//             dampPercent = (curr / steps);

//             _distanceOffset += YMinDistanceOffset * (1 - dampPercent);
//         }
//         else if (_currentY >= YMaxDampStart) {
//             float curr = YMax - _currentY;
//             float steps = YMax - YMaxDampStart;
//             dampPercent = (curr / steps);

//             _distanceOffset += YMaxDistanceOffset * (1 - dampPercent);
//         }

// 		return dampPercent;
// 	}

// 	//? returns the correct position after checking if it might have gotten occluded, and if so, return the no smoothed
// 	private Vector3 CheckCameraOcclusion(Vector3 desiredPosition, Vector3 currentPosition) {
// 		Vector3 direction = desiredPosition - _source.position;
// 		float distance = direction.magnitude;
// 		direction.Normalize();

// 		// Use SphereCast for smoother collision detection
// 		float sphereRadius = 0.2f; // Adjust based on your camera's needs
// 		if (Physics.SphereCast(_source.position, sphereRadius, direction, out RaycastHit hit, distance, OcclusionLayers))
// 		{
// 			// Position the camera just before the collision point
// 			float adjustedDistance = Mathf.Max(hit.distance - 0.2f, MinimumDistance);
// 			return _source.position + direction * adjustedDistance;
// 		}

// 		return desiredPosition;
// 	}


// 	public void SetCameraSensitivity(float value = 0.25f) {
// 		Debug.Log($"Set Camera Sensitivity: {value} = {value * 40}");
// 		this.Settings.Sensitivity = Mathf.Clamp(value * 40, 1, 40); //? 40 is max sensitivity
// 	}

//     private void HandleCursorLock() {
// 		if (!Application.isEditor) return;
        
//         if (Runtime.InMenu) {
//             Cursor.lockState = CursorLockMode.None;
//             Cursor.visible = true;
//         } else {
//             Cursor.lockState = CursorLockMode.Locked;
//             Cursor.visible = false;
//         }
//     }
// }