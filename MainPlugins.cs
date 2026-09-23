using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

namespace HighPredsMod
{
    [BepInPlugin("com.highpreds.lefthandpush", "HighPredsMod", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static bool PushEnabled = false;
        private static float _lastAPressTime = -1f;
        private static bool _wasAPressed = false;

        void Awake()
        {
            try
            {
                var harmony = new Harmony("com.highpreds.lefthandpush");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Logger.LogInfo("HighPredsMod loaded.");
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
                        Logger.LogInfo("Push " + (PushEnabled ? "ON" : "OFF"));
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

    [HarmonyPatch(typeof(GTPlayer), "FixedUpdate")]
    class GTPlayerFixedUpdatePatch
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

        static bool Prefix(GTPlayer __instance)
        {
            try
            {
                if (__instance == null) return true;
                if (!Plugin.PushEnabled) return true;

                // ← левая рука через метод GetHandPosition(false)
                Vector3 leftHandPos = __instance.GetHandPosition(false);
                if (leftHandPos == Vector3.zero) return true;

                if (!_initialized)
                {
                    _lastHandPosition = leftHandPos;
                    _initialized = true;
                    return true;
                }

                Vector3 handVelocity = Vector3.zero;
                if (Time.fixedDeltaTime > 0f)
                    handVelocity = (leftHandPos - _lastHandPosition) / Time.fixedDeltaTime;
                _lastHandPosition = leftHandPos;

                // ← тело через метод BodyCollider()
                Collider bodyCollider = __instance.BodyCollider();
                if (bodyCollider == null) return true;

                var rb = bodyCollider.attachedRigidbody;
                if (rb == null) return true;

                if (_isPushing)
                {
                    _pushVelocity.y -= 9.81f * Time.fixedDeltaTime;
                    rb.velocity = _pushVelocity;
                    _pushVelocity *= Decay;

                    bool grounded = Physics.Raycast(
                        bodyCollider.transform.position,
                        Vector3.down,
                        0.3f,
                        Physics.DefaultRaycastLayers,
                        QueryTriggerInteraction.Ignore
                    );
                    return false;
                }

                if (_cooldown > 0f)
                {
                    _cooldown -= Time.fixedDeltaTime;
                    _leftHandWasTouching = false;
                    return true;
                }

                bool onGround = Physics.Raycast(
                    bodyPos, Vector3.down, 0.5f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore
                );
                if (!onGround) return true;

                bool leftHandTouching = Physics.Raycast(
                    leftHandPos, Vector3.down, 0.12f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore
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
                Debug.LogError("[HighPredsMod] " + e);
                return true;
            }
        }
    }
}
