using BepInEx;
using UnityEngine;

namespace MyVelocityPredictionMod
{
    [BepInPlugin(
        "com.username.velocityprediction",
        "Velocity Prediction Mod",
        "1.0.0"
    )]
    public class MainPlugins : BaseUnityPlugin
    {
        public static float predCount = 0.9f;

        private Vector3 lastLeftPos;
        private Vector3 lastRightPos;
        private bool initialized;

        private void Update()
        {
            if (GorillaTagger.Instance == null)
                return;

            Transform leftHand = GorillaTagger.Instance.leftHandTransform;
            Transform rightHand = GorillaTagger.Instance.rightHandTransform;

            if (leftHand == null || rightHand == null)
                return;

            // Первый кадр — только запоминаем позиции
            if (!initialized)
            {
                lastLeftPos = leftHand.position;
                lastRightPos = rightHand.position;
                initialized = true;
                return;
            }

            float deltaTime = Time.deltaTime;

            if (deltaTime <= 0f)
                return;

            // Скорость рук
            Vector3 leftVelocity =
                (leftHand.position - lastLeftPos) / deltaTime;

            Vector3 rightVelocity =
                (rightHand.position - lastRightPos) / deltaTime;

            // Запоминаем позиции для следующего кадра
            lastLeftPos = leftHand.position;
            lastRightPos = rightHand.position;

            // A на правом контроллере
            if (ControllerInputPoller.instance != null &&
                ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                if (GorillaTagger.Instance.offlineVRRig == null)
                    return;

                GorillaTagger.Instance.offlineVRRig.leftHand
                    .rigTarget.transform.position += leftVelocity * predCount;

                GorillaTagger.Instance.offlineVRRig.rightHand
                    .rigTarget.transform.position += rightVelocity * predCount;
            }
        }
    }
}
