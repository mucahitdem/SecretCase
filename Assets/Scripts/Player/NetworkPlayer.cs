using System;
using System.Collections;
using System.Collections.Generic;
using HealthService;
using InventoryService;
using UI;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 14f;
        [SerializeField] private float deceleration = 18f;
        [SerializeField] private float gravity = -18f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float mouseSensitivity = 120f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;
        [SerializeField] private Camera playerCamera;

        private CharacterController _controller;
        private Vector3 _currentVelocity;
        private float _verticalVelocity;
        private float _pitch;
        private bool _movementEnabled = true;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public override void OnNetworkSpawn()
        {
            _controller.enabled = false;
            
            StartCoroutine(EnableControllerNextFrame());

            var networkTransform = GetComponent<NetworkTransform>();
            if (networkTransform != null && networkTransform.CanCommitToTransform)
                networkTransform.Teleport(transform.position, transform.rotation, transform.localScale);

            if (playerCamera != null)
                playerCamera.gameObject.SetActive(IsOwner);

            if (!IsOwner) return;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            var hud = FindAnyObjectByType<HUDController>();
            if (hud != null)
                hud.BindLocalPlayer(GetComponent<HealthComponent>(), GetComponent<PlayerInventoryController>());

            var inventoryPanel = FindAnyObjectByType<InventoryPanelUI>();
            if (inventoryPanel != null)
                inventoryPanel.BindLocalPlayer(GetComponent<PlayerInventoryController>());
            
            
        }
        public void SetMovementEnabled(bool enabled)
        {
            _movementEnabled = enabled;
            _controller.enabled = enabled;
            if (!enabled) _currentVelocity = Vector3.zero;
        }
        
        
        private IEnumerator EnableControllerNextFrame()
        {
            yield return null;
            _controller.enabled = true;
        }
        private void Update()
        {
            if (!IsOwner) return;

            HandleCursorToggle();
            HandleMouseLook();
            if (_movementEnabled) HandleOwnerMovement();
        }
        private void HandleCursorToggle()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return;

            var locked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = locked;
        }
        private void HandleMouseLook()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            var mouse = Mouse.current;
            if (mouse == null) return;

            var delta = mouse.delta.ReadValue() * (mouseSensitivity * 0.01f);

            transform.Rotate(Vector3.up, delta.x, Space.World);

            if (playerCamera != null)
            {
                _pitch = Mathf.Clamp(_pitch - delta.y, minPitch, maxPitch);
                playerCamera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }
        private void HandleOwnerMovement()
        {
            if(!_controller.enabled)
                return;
            
            var keyboard = Keyboard.current;
            var input = Vector2.zero;

            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed) input.y += 1f;
                if (keyboard.sKey.isPressed) input.y -= 1f;
                if (keyboard.aKey.isPressed) input.x -= 1f;
                if (keyboard.dKey.isPressed) input.x += 1f;
            }

            var wishDirection = transform.right * input.x + transform.forward * input.y;
            if (wishDirection.sqrMagnitude > 1f) wishDirection.Normalize();

            var targetVelocity = wishDirection * moveSpeed;
            var rate = targetVelocity.sqrMagnitude > _currentVelocity.sqrMagnitude ? acceleration : deceleration;
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVelocity, rate * Time.deltaTime);

            if (_controller.isGrounded)
            {
                _verticalVelocity = -0.5f;
                if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }

            var motion = _currentVelocity;
            motion.y = _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);
        }
    }
}
