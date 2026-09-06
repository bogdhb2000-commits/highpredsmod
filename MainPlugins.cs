using System;
using BepInEx;
using UnityEngine;

namespace MyVelocityPredictionMod
{
    [BepInPlugin("com.username.velocityprediction", "Velocity Prediction Mod", "1.0.0")]
    public class MainPlugins : BaseUnityPlugin
    {
        public static float predCount = 0.4f;

        void Update()
        {
            // Проверка нажатия кнопки A на правом контроллере
            if (ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                ApplyPrediction();
            }
        }

        private void ApplyPrediction()
        {
            if (GorillaTagger.Instance == null) return;

            // Получаем скорость рук напрямую через Rigidbody игроков
            Vector3 leftVelocity = GorillaTagger.Instance.leftHandTransform.GetComponent<Rigidbody>()?.velocity ?? Vector3.zero;
            Vector3 rightVelocity = GorillaTagger.Instance.rightHandTransform.GetComponent<Rigidbody>()?.velocity ?? Vector3.zero;

            // Применяем смещение
            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position -= leftVelocity * predCount;
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position -= rightVelocity * predCount;
        }
    }
}
