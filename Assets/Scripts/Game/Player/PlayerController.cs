using System;
using Baerhous.Games.Towerfall.Game.Player;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

namespace Baerhous.Games.Towerfall
{
    public class PlayerController : NetworkBehaviour
    {

        [Header("Player Information")] public string userName;
        
        public InputActionAsset inputs;
        
        [Header("Player Locomotion")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -9.81f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Header("Player Animations")] [Tooltip("Holds Basic Player Attack Combo")]
        public Animation[] BasicAttacks;

        [Tooltip("Enhanced Attack A")] public Animation enhancedAttack0Anim;
        [Tooltip("Enhanced Attack B")] public Animation enhancedAttack1Anim;
        [Tooltip("Enhanced Attack C")] public Animation enhancedAttack2Anim;
        [Tooltip("Enhanced Attack D")] public Animation enhancedAttack3Anim;

        [Header("Player Attributes")] [Tooltip("Player Spellbook")]
        public SpellbookController Spellbook;
        
        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        // TODO: Change into input system at later date
        private void Update()
        {
            if (!IsOwner) return;

            Vector3 moveDir = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
            if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
            if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
            if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

            transform.position += moveDir * (MoveSpeed * Time.deltaTime);
        }

        private void LookAround()
        {
            
        }
        
        private void Jump()
        {
            
        }

        private void Locomotion()
        {
            if (!IsOwner) return;
        }

        private void PlayerGravity()
        {
            
        }

        private void Sprint()
        {
            
        }

        private void Pause()
        {
            
        }

        private void UnPause()
        {
            
        }

        private void BaseComboAttack()
        {
            
        }

        private void PrepareSpell()
        {
            
        }

        private void RecordSpellInput()
        {
            
        }

        private void ActivateSpell()
        {
            
        }
    }

    enum SpellInputEntry
    {
        U,
        D,
        L,
        R,
    }
}
