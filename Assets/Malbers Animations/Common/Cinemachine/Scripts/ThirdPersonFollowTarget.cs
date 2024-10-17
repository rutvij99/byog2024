using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;


using Unity.Cinemachine;
using UnityEngine;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Camera/Third Person Follow Target (CM3)")]
    public class ThirdPersonFollowTarget : MonoBehaviour
    {
        /// <summary> List of all the scene Third Person Follow Cameras (using the same brain)! </summary>
        public static HashSet<ThirdPersonFollowTarget> TPFCameras;

        [Tooltip("Cinemachine Brain Camera")]
        public CinemachineBrain Brain;

        [Tooltip("Update mode for Camera Position Logic")]
        public UpdateType updateMode = UpdateType.FixedUpdate;

        [Tooltip("The Camera can rotate independent of the Game Time")]
        public BoolReference unscaledTime = new(true);

        [Tooltip("Default Priority of this Cinemachine camera")]
        public int priority = 10;
        [Tooltip("Changes the Camera Side parameter on the Third Person Camera")]
        [Range(0f, 1f), SerializeField]
        private float cameraSide = 1f;
        [Tooltip("Default Camera Distance set to the Third Person Cinemachine Camera")]
        public FloatReference CameraDistance = new(6);

        [Tooltip("What object to follow")]
        public TransformReference Target;
        [Tooltip("Reference of a Transform to get the Up Vector, so the camera can be aligned with it vector")]
        public TransformReference upVector;

        public Transform CamPivot { get; set; }

        [Tooltip("Camera Input Values (Look X:Horizontal, Look Y: Vertical)")]
        public Vector2Reference look = new();

        [Header("Camera Properties")]
        [Tooltip("Sensitivity to rotate the X Axis")]
        public FloatReference XMultiplier = new(1);
        [Tooltip("Sensitivity to rotate the Y Axis")]
        public FloatReference YMultiplier = new(1);
        [Tooltip("How far in degrees can you move the camera up")]
        public FloatReference TopClamp = new(70.0f);
        [Tooltip("How far in degrees can you move the camera down")]
        public FloatReference BottomClamp = new(-30.0f);

        [Tooltip("Lerp Rotation to smooth out the movement of the camera while rotating.")]
        public FloatReference LerpRotation = new(15f);
        [Tooltip("Lerp Position to smooth out the movement of the camera while following the target.")]
        public FloatReference lerpPosition = new(0);
        [Tooltip("Invert X Axis of the Look Vector")]
        public BoolReference invertX = new();
        [Tooltip("Invert Y Axis of the Look Vector")]
        public BoolReference invertY = new();

        [Header("Mouse Keyboard and GamePad")]
        [Tooltip("Is the camera using Mouse Input (true) or a Gamepad (False)")]
        public BoolReference UsingMouse = new(true);
        [Tooltip("Extra Multiplier for the Rotation sensitivity when using a gamepad")]
        public FloatReference GamepadMult = new(100);

        public BoolEvent OnActiveCamera = new();

        #region Properties
        private float InvertX => invertX.Value ? -1 : 1;
        private float InvertY => invertY.Value ? 1 : -1;

        public float XSensibility { get => XMultiplier; set => XMultiplier.Value = value; }
        public float YSensibility { get => YMultiplier; set => YMultiplier.Value = value; }
        public float LerpPosition { get => lerpPosition; set => lerpPosition.Value = value; }
        public Transform UpVector { get => upVector; set => upVector.Value = value; }
        public bool UnScaledTime { get => unscaledTime; set => unscaledTime.Value = value; }


        /// <summary>  Active Camera using the same Cinemachine Brain </summary>
        public ThirdPersonFollowTarget ActiveThirdPersonCamera { get; set; }



        public float CameraSide { get => cameraSide; set => cameraSide = value; }


        #endregion

        private ICinemachineCamera ThisCamera;
        private CinemachineThirdPersonFollow CM3PFollow;

        // cinemachine

        [Disable] public float _cinemachineTargetYaw;
        [Disable] public float _cinemachineTargetPitch;


        private const float _threshold = 0.00001f;


        public void SetMouse(bool value) => UsingMouse.Value = value;
        public bool SetInvertX(bool value) => invertX.Value = value;
        public bool SetInvertY(bool value) => invertY.Value = value;
        void Awake()
        {
            if (Brain == null)
                Brain = FindAnyObjectByType<CinemachineBrain>();

            CM3PFollow = this.FindComponent<CinemachineThirdPersonFollow>();

            if (CM3PFollow != null)
            {
                CM3PFollow.CameraDistance = CameraDistance;
                CM3PFollow.CameraSide = CameraSide;
            }

            UsingMouse.Value = true;
        }


        private void OnEnable()
        {
            //Brain.m_CameraActivatedEvent.AddListener(CameraChanged);
            TPFCameras ??= new(); //Initialize the Cameras
            TPFCameras.Add(this);


            //Search on the other TFP cameras to see if we are using the same Target...
            //if we are using the same Target use their Cam Pivot instead
            if (CamPivot == null)
            {
                foreach (var c in TPFCameras)
                {
                    if (c == this) continue; //Skip itself

                    //If another Camera is using the same 
                    if (c.Target.Value == Target.Value && c.CamPivot != null)
                    {
                        CamPivot = c.CamPivot; //Use the same Cam Pivot
                        break;
                    }
                }
            }


            if (CamPivot == null) //There's no CamPivot after searching in all other Cameras, let's create one
            {
                CamPivot = new GameObject($"CamPivot - [{(Target.Value != null ? Target.Value.name : name)}]").transform;
                CamPivot.ResetLocal();
                CamPivot.parent = null;
                //  CamPivot.hideFlags = HideFlags.HideInHierarchy; //Hide it we do not need to see it
            }

            //Find the Cinemachine camera Target
            if (TryGetComponent(out ThisCamera))
                (ThisCamera as CinemachineCamera).Target.TrackingTarget = CamPivot.transform;


            transform.position = CamPivot.position;

            CameraPosition(0, 0);
            StartCameraLogic();

            //Set the Up Vector to the Camera Brain
            this.Delay_Action(1,
                () =>
                Brain.WorldUpOverride = UpVector);
        }

        private void OnDisable()
        {
            // Brain.m_CameraActivatedEvent.RemoveListener(CameraChanged);
            StopAllCoroutines();

            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            TPFCameras.Remove(this);
        }


        public void TargetTeleport()
        {
            //Update the Active Camera if we are the active camera
            if (ThisCamera == Brain.ActiveVirtualCamera)
            {
                //Remove the damping for 5 frames so the camera can teleport correctly
                var OldDamp = CM3PFollow.Damping;
                CM3PFollow.Damping = Vector3.zero;
                this.Delay_Action(5, () => CM3PFollow.Damping = OldDamp);
            }
        }


        private void StartCameraLogic()
        {
            if (updateMode == UpdateType.FixedUpdate)
                StartCoroutine(ICameraPositionFixed()); //Position Only (Fixed Update)
            else
                StartCoroutine(ICameraPositionUpdate()); //Position Only (Fixed Update)


            StartCoroutine(ICameraRotation()); //Rotation Only (Late Update)
        }

        private IEnumerator ICameraPositionFixed()
        {
            while (true)
            {
                CameraPos(UnScaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator ICameraPositionUpdate()
        {
            while (true)
            {
                CameraPos(UnScaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
                //yield return new WaitForEndOfFrame();
                yield return null;
            }
        }

        private IEnumerator ICameraRotation()
        {
            while (true)
            {
                CameraRotation(UnScaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
                //yield return new WaitForEndOfFrame();
                yield return null;
            }
        }

        private bool active;

        private void CameraPos(float deltaTime)
        {
            //Update the Active Camera if we are the active camera
            if (ThisCamera == Brain.ActiveVirtualCamera)
            {
                if (!active)
                {
                    if (ActiveThirdPersonCamera != null)
                        ActiveThirdPersonCamera.active = false; //Old Camera set it to false

                    ActiveThirdPersonCamera = this;
                    active = true;
                    OnActiveCamera.Invoke(active);
                    CameraPosition(LerpPosition, deltaTime);

                    return;     //Skip this cycle
                }
            }
            else
            {
                //Make sure this one is disabled
                if (active)
                {
                    active = false;
                    OnActiveCamera.Invoke(active);
                }
            }

            if (!active) return;


            //Skip if the TimeScale is zero
            if (!UnScaledTime && Time.timeScale == 0)
            {
                look.Value = Vector2.zero;
                return;
            }

            if (ActiveThirdPersonCamera == this)
            {
                CameraPosition(LerpPosition, deltaTime);
                // CameraRotation(deltaTime);
                SetCameraSide(CameraSide);
            }
        }


        private void UpdateAllCamerasYawPitch()
        {
            foreach (var c in TPFCameras)
            {
                if (c.Target.Value != Target.Value) continue; //Skip if the camera is using different pivots

                //Update Rotation Values to all other cameras
                c._cinemachineTargetYaw = _cinemachineTargetYaw;
                c._cinemachineTargetPitch = _cinemachineTargetPitch;
            }
        }

        public void SetLookX(float x) => look.x = x;
        public void SetLookY(float y) => look.y = y;
        public void SetLook(Vector2 look) => this.look.Value = look;

        private void CameraPosition(float lerp, float deltatime)
        {
            if (Target.Value)
            {
                if (lerp == 0)
                {
                    CamPivot.transform.position = Target.position;
                }
                else
                {
                    CamPivot.transform.position = Vector3.Lerp(CamPivot.transform.position, Target.position, lerp * deltatime);
                }
            }
        }

        private void CameraRotation(float deltaTime)
        {
            if (ActiveThirdPersonCamera == this)
            {
                // if there is an input and camera position
                if (look.Value.sqrMagnitude >= _threshold)
                {
                    //Don't multiply mouse input by Time.deltaTime;
                    float deltaTimeMultiplier = UsingMouse ? 1.0f : (deltaTime * GamepadMult);

                    _cinemachineTargetYaw += look.x * InvertX * XMultiplier * deltaTimeMultiplier;
                    _cinemachineTargetPitch += look.y * InvertY * YMultiplier * deltaTimeMultiplier;
                }

                // clamp our rotations so our values are limited 360 degrees
                _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Cinemachine will follow this target
                var TargetRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);

                if (UpVector) TargetRotation = Quaternion.FromToRotation(Vector3.up, UpVector.up) * TargetRotation;

                CamPivot.rotation = Quaternion.Lerp(CamPivot.rotation, TargetRotation, deltaTime * LerpRotation); //NEEDED FOR SMOOTH CAMERA MOVEMENT

                UpdateAllCamerasYawPitch();
            }
        }

        public void SetTarget(Transform target) => Target.Value = target;

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }


        public void SetPriority(bool value)
        {
            if (ThisCamera is CinemachineCamera cam)
            {
                if (value)
                {
                    cam.Priority.Value = priority;
                    cam.Priority.Enabled = true;
                }
                else
                {
                    cam.Priority.Value = -1;
                    cam.Priority.Enabled = false;
                }
            }
        }

        public void SetCameraSide(bool value) => SetCameraSide(value ? 1 : 0);

        public void SetCameraSide(float value)
        {
            if (CameraSide != value)
            {
                CameraSide = value;
                CM3PFollow.CameraSide = CameraSide;
            }
        }

        private void CheckRotation()
        {
            var EulerAngles = Brain.transform.eulerAngles; //Get the Brain Rotation to save the movement 

            _cinemachineTargetYaw = ClampAngle(EulerAngles.y, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = EulerAngles.x > 180 ? EulerAngles.x - 360 : EulerAngles.x; //HACK!!!
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CamPivot.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);

            UpdateAllCamerasYawPitch();
        }


#if UNITY_EDITOR

        private void OnValidate()
        {
            if (Application.isPlaying && CM3PFollow != null)
                CM3PFollow.CameraSide = CameraSide;
        }
        private void Reset()
        {
            Target.UseConstant = false;
            Target.Variable = MTools.GetInstance<TransformVar>("Camera Target");


            if (CamPivot == null)
            {
                CamPivot = new GameObject("Pivot").transform;
                CamPivot.parent = transform;
                CamPivot.ResetLocal();
            }
        }
#endif
    }
}
