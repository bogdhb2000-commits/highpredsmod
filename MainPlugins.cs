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
        // Переменные для Silent Pull (Правый джойстик)
        public static float pullStrength = 15f;
        private VRRig currentTarget = null;

        // Переменные для Playspace Abuse (Левый джойстик)
        public static float playspaceAbusePower = 0.004f;

        void Update()
        {
            // 1. Скрытый Silent Pull на правый джойстик (нажатие)
            bool isRightJoystickPressed = IsJoystickClicked(XRNode.RightHand);
            if (isRightJoystickPressed)
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

            // 2. Скрытый Playspace Abuse на левый джойстик (нажатие)
            bool isLeftJoystickPressed = IsJoystickClicked(XRNode.LeftHand);
            if (isLeftJoystickPressed)
            {
                PlayspaceAbuse();
            }
        }

        // Логика смещения без визуальных эффектов и текстов
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

        // Проверка нажатия джойстика
        private bool IsJoystickClicked(XRNode node)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(node);
            if (device.isValid && device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool isPressed))
            {
                return isPressed;
            }

            return false;
        }
    }
}
