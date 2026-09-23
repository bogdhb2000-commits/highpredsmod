using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

namespace LeftHandPushMod
{
    [BepInPlugin("com.yourname.lefthandpush", "LeftHandPush", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static bool PushEnabled = false;
        private static float _lastAPressTime = -1f;
        private static bool _wasAPressed = false;

        void Awake()
        {
            try
            {
                var harmony = new Harmony("com.yourname.lefthandpush");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Logger.LogInfo("LeftHandPush loaded.");
            }
            catch (Exception e)
            {
                Logger.LogError("Harmony patch failed: " + e);
            }
        }

        void Update()
        {
            try
            {
                InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                if (!leftHand.isValid) return;

                leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed);

                if (aPressed && !_wasAPressed)
                {
                    if (Time.time - _lastAPressTime <= 0.25f)
                    {
                        PushEnabled = !PushEnabled;
                        _lastAPressTime = -1f;
                    }
                    else
                    {
                        _lastAPressTime = Time.time;
                    }
                }
                _wasAPressed = aPressed;
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(Player), "FixedUpdate")]
    class PlayerFixedUpdatePatch
    {
        private static Vector3 _pushVelocity = Vector3.zero;
        private static bool _isPushing = false;
        private static float _cooldown = 0f;
        private const float CooldownTime = 0.2f;
        private const float PushSpeed = 8f;
        private const float Decay = 0.97f;
        private const float MinSpeed = 0.5f;

        private static bool _leftHandWasTouching = false;
        private static Vector3 _lastHandPosition = Vector3.zero;
        private static bool _initialized = false;

        static bool Prefix(Player __instance)
        {
            try
            {
                if (__instance == null) return true;
                if (!Plugin.PushEnabled) return true;

                Transform leftHand = __instance.leftHandTransform;
                if (leftHand == null) return true;

                if (!_initialized)
                {
                    _lastHandPosition = leftHand.position;
                    _initialized = true;
                    return true;
                }

                Vector3 handVelocity = Vector3.zero;
                if (Time.fixedDeltaTime > 0f)
                    handVelocity = (leftHand.position - _lastHandPosition) / Time.fixedDeltaTime;
                _lastHandPosition = leftHand.position;

                var rb = __instance.bodyCollider != null
                    ? __instance.bodyCollider.attachedRigidbody
                    : null;
                if (rb == null) return true;

                // === РЕЖИМ РЫВКА ===
                if (_isPushing)
                {
                    _pushVelocity.y -= 9.81f * Time.fixedDeltaTime;
                    rb.velocity = _pushVelocity;
                    _pushVelocity *= Decay;

                    bool grounded = Physics.Raycast(
                        __instance.bodyCollider.transform.position,
                        Vector3.down,
                        0.3f,
                        __instance.locomotionEnabledLayers
                    );

                    if (_pushVelocity.magnitude < MinSpeed || (grounded && _pushVelocity.y <= 0f))
                    {
                        _isPushing = false;
                        _cooldown = CooldownTime;
                    }

                    return false;
                }
                // === КУЛДАУН ===
                if (_cooldown > 0f)
                {
                    _cooldown -= Time.fixedDeltaTime;
                    _leftHandWasTouching = false;
                    return true;
                }

                // === ПРОВЕРКА ЗЕМЛИ ПОД ТЕЛОМ ===
                bool onGround = Physics.Raycast(
                    __instance.bodyCollider.transform.position,
                    Vector3.down,
                    0.5f,
                    __instance.locomotionEnabledLayers
                );
                if (!onGround) return true;

                // === КАСАНИЕ ЛЕВОЙ РУКОЙ ===
                bool leftHandTouching = Physics.Raycast(
                    leftHand.position,
                    Vector3.down,
                    0.12f,
                    __instance.locomotionEnabledLayers
                );

                if (leftHandTouching && !_leftHandWasTouching)
                {
                    Vector3 pushDir = handVelocity.normalized;
                    if (pushDir.magnitude > 0.1f && handVelocity.magnitude > 1.5f)
                    {
                        _pushVelocity = pushDir * PushSpeed;
                        _isPushing = true;
                    }
                }

                _leftHandWasTouching = leftHandTouching;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("[LeftHandPush] " + e);
                return true;
            }
        }
    }
}
