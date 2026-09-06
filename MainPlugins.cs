using System;
using BepInEx;
using UnityEngine;
using UnityEngine.XR;

namespace MySilentPullMod
{
    [BepInPlugin("com.username.silentpullmod", "Silent Pull Mod", "1.0.0")]
    public class SilentPullMod : BaseUnityPlugin
    {
        // Настройки Playspace Abuse
        public static float playspaceAbusePower = 0.05f;

        void Update()
        {
            // Активация при нажатии на ЛЕВЫЙ джойстик (клик по левому стику)
            if (IsJoystickClicked(XRNode.LeftHand))
            {
                PlayspaceAbuse();
            }
        }

        private void PlayspaceAbuse()
        {
            GorillaTagger.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * playspaceAbusePower;
        }

        // Проверка клика по джойстику через XR Input
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
