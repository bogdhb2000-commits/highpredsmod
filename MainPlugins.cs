using System;
using BepInEx;
using UnityEngine;

namespace MyVelocityPredictionMod
{
    [BepInPlugin("com.username.velocityprediction", "Velocity Prediction Mod", "1.0.0")]
    public class MainPlugins : BaseUnityPlugin
    {
        public static float predCount = 0.9f;

        private Vector3 lastLeftPos;
        private Vector3 lastRightPos;

        void Update()
        {
            if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null) 
                return;

            // Находим текущее положение рук
            Transform leftTransform = GorillaTagger.Instance.leftHandTransform;
            Transform rightTransform = GorillaTagger.Instance.rightHandTransform;

            if (leftTransform == null || rightTransform == null) 
                return;

            // Высчитываем скорость движения рук (смещение за время кадра)
            Vector3 leftVelocity = (leftTransform.position - lastLeftPos) / Time.deltaTime;
            Vector3 rightVelocity = (rightTransform.position - lastRightPos) / Time.deltaTime;

            // Сохраняем позиции для следующего кадра
            lastLeftPos = leftTransform.position;
            lastRightPos = rightTransform.position;

            // Если зажата кнопка A — вытягиваем руки в сторону движения
            if (ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position += leftVelocity * (predCount * 0.1f);
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position += rightVelocity * (predCount * 0.1f);
            }
        }
    }
}
