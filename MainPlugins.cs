using System;
using BepInEx;
using UnityEngine;
using UnityEngine.XR;

namespace MySilentPullMod
{
    [BepInPlugin("com.username.silentpullmod", "Silent Pull Mod", "1.0.0")]
    public class SilentPullMod : BaseUnityPlugin
    {
        // Сила смещения
        public static float playspaceAbusePower = 0.05f;

        void Update()
        {
            // Проверяем наклон левого джойстика
            Vector2 joystick = GetJoystickInput(XRNode.LeftHand);

            // Если левый стик наклонен вперед (значение по Y больше 0.5)
            if (joystick.y > 0.5f)
            {
                PlayspaceAbuse();
            }
        }

        private void PlayspaceAbuse()
        {
            GorillaTagger.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * playspaceAbusePower;
        }

        // Получаем точный вектор наклона джойстика через XR Input
        private Vector2 GetJoystickInput(XRNode node)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(node);
            if (device.isValid && device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
            {
                return axis;
            }
            return Vector2.zero;
        }
    }
}
