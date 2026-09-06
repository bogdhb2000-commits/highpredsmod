using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using UnityEngine.XR;
using GorillaLocomotion;

namespace MySilentPullMod
{
    [BepInPlugin("com.username.silentpullmod", "Silent Pull Mod", "1.0.0")]
    public class SilentPullMod : BaseUnityPlugin
    {
        // Переменные для Silent Pull
        public static float pullStrength = 15f;
        private VRRig currentTarget = null;

        // Переменные для Playspace Abuse
        public static float playspaceAbusePower = 0.004f;

        void Update()
        {
            // 1. Silent Pull на кнопку B (SecondaryButton на Правой руке)
            bool isBPressed = IsButtonPressed(XRNode.RightHand, CommonUsages.secondaryButton);
            if (isBPressed)
            {
                if (currentTarget == null)
                {
                    currentTarget = GetClosestPlayer();
                }

                if (currentTarget != null)
                {
                    Vector3 myPosition = GorillaTagger.Instance.bodyCollider.transform.position;
                    currentTarget.transform.position = Vector3.Lerp(
                        currentTarget.transform.position, 
                        myPosition, 
                        Time.deltaTime * pullStrength
                    );
                }
            }
            else
            {
                currentTarget = null;
            }

            // 2. Playspace Abuse на кнопку X (PrimaryButton на Левой руке)
            bool isXPressed = IsButtonPressed(XRNode.LeftHand, CommonUsages.primaryButton);
            if (isXPressed)
            {
                PlayspaceAbuse();
            }
        }

        // Логика смещения
        private void PlayspaceAbuse()
        {
            GorillaTagger.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * playspaceAbusePower;
        }

        // Поиск ближайшего игрока
        private VRRig GetClosestPlayer()
        {
            VRRig closest = null;
            float minDistance = float.MaxValue;
            Vector3 myPos = GorillaTagger.Instance.bodyCollider.transform.position;
            VRRig[] allRigs = UnityEngine.Object.FindObjectsOfType<VRRig>();

            foreach (VRRig rig in allRigs)
            {
                if (rig != null && rig != GorillaTagger.Instance.offlineVRRig)
                {
                    float dist = Vector3.Distance(myPos, rig.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closest = rig;
                    }
                }
            }

            return closest;
        }

        // Проверка нажатия заданной кнопки
        private bool IsButtonPressed(XRNode node, InputFeatureUsage<bool> button)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(node);
            if (device.isValid && device.TryGetFeatureValue(button, out bool isPressed))
            {
                return isPressed;
            }

            return false;
        }
    }
}
