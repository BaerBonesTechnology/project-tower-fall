using UnityEngine;
using UnityEngine.Serialization;

namespace Baerhous.Games.Towerfall
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Locomotion")]
        [Tooltip("Move speed of the character in m/s")]
        public float moveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float rotationSmoothTime = 0.12f;
        
        [FormerlySerializedAs("SpeedChangeRate")] [Tooltip("Acceleration and deceleration")]
        public float speedChangeRate = 10.0f;

        [FormerlySerializedAs("LandingAudioClip")] public AudioClip landingAudioClip;
        [FormerlySerializedAs("FootstepAudioClips")] public AudioClip[] footstepAudioClips;
        [FormerlySerializedAs("FootstepAudioVolume")] [Range(0, 1)] public float footstepAudioVolume = 0.5f;

        [FormerlySerializedAs("JumpHeight")]
        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float jumpHeight = 1.2f;

        [FormerlySerializedAs("Gravity")] [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float gravity = -15.0f;

        [FormerlySerializedAs("JumpTimeout")]
        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float jumpTimeout = 0.50f;

        [FormerlySerializedAs("FallTimeout")] [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float fallTimeout = 0.15f;

        [FormerlySerializedAs("Grounded")]
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool grounded = true;

        [FormerlySerializedAs("GroundedOffset")] [Tooltip("Useful for rough ground")]
        public float groundedOffset = -0.14f;

        [FormerlySerializedAs("GroundedRadius")] [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundedRadius = 0.28f;

        [FormerlySerializedAs("GroundLayers")] [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayers;

        [FormerlySerializedAs("CinemachineCameraTarget")]
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject cinemachineCameraTarget;

        [FormerlySerializedAs("TopClamp")] [Tooltip("How far in degrees can you move the camera up")]
        public float topClamp = 70.0f;

        [FormerlySerializedAs("BottomClamp")] [Tooltip("How far in degrees can you move the camera down")]
        public float bottomClamp = -30.0f;

        [FormerlySerializedAs("CameraAngleOverride")] [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
        public float cameraAngleOverride;

        [FormerlySerializedAs("LockCameraPosition")] [Tooltip("For locking the camera position on all axis")]
        public bool lockCameraPosition;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        // private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
       // private float _terminalVelocity = 53.0f;

        // timeout delta time
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
    }
}
