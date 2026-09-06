using System;
using BepInEx;
using UnityEngine;

namespace MyVelocityPredictionMod
{
    [BepInPlugin("com.username.velocityprediction", "Velocity Prediction Mod", "1.0.0")]
    public class VelocityPredictionMod : BaseUnityPlugin
    {
        // Сила предикшена
        public static float predCount = 0.4f;

        private GameObject lvT;
        private GameObject rvT;

        void Start()
        {
            // Создаем трекеры скорости
            lvT = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(lvT.GetComponent<BoxCollider>());
            lvT.GetComponent<Renderer>().enabled = false;
            lvT.AddComponent<GorillaVelocityTracker>();

            rvT = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(rvT.GetComponent<BoxCollider>());
            rvT.GetComponent<Renderer>().enabled = false;
            rvT.AddComponent<GorillaVelocityTracker>();
        }

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

            // Расчет позиции трекеров
            lvT.transform.position = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position;
            rvT.transform.position = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position;

            // Смещение рук
            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position -= lvT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0) * predCount;
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position -= rvT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0) * predCount;
        }

        private void OnDestroy()
        {
            if (lvT != null) Destroy(lvT);
            if (rvT != null) Destroy(rvT);
        }
    }
}
