using BepInEx;
using UnityEngine;
using System.Reflection;

namespace HighPredsMod
{
    [BepInPlugin("com.yourname.highpredsmod", "High Preds 3 Sec Timer", "1.0.0")]
    public class MainPlugins : BaseUnityPlugin
    {
        private bool isTimerRunning = false;
        private float timer = 0f;
        private const float TIMER_DURATION = 3f;
        private const float HIGH_PREDICTION = 0.1f;

        private float originalPrediction = 0.02f;
        private bool originalSaved = false;

        private PropertyInfo predictionProp;
        private object playerInstance;

        void Update()
        {
            // Получаем ссылку на игрока без прямой жесткой зависимости от пространств имён
            if (playerInstance == null)
            {
                var playerType = System.Type.GetType("GorillaLocomotion.Player, Assembly-CSharp");
                if (playerType != null)
                {
                    var instanceProp = playerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        playerInstance = instanceProp.GetValue(null);
                    }
                    predictionProp = playerType.GetProperty("predictionTime", BindingFlags.Public | BindingFlags.Instance);
                }
            }

            if (playerInstance == null || predictionProp == null) return;

            // Сохраняем начальное значение
            if (!originalSaved)
            {
                originalPrediction = (float)predictionProp.GetValue(playerInstance);
                originalSaved = true;
            }

            // Проверка ввода
            bool rightPressed = false;
            if (ControllerInputPoller.instance != null)
            {
                rightPressed = ControllerInputPoller.instance.rightControllerPrimaryButton || 
                               ControllerInputPoller.instance.rightControllerSecondaryButton;
            }

            // Активация на 3 секунды
            if (rightPressed && !isTimerRunning)
            {
                isTimerRunning = true;
                timer = TIMER_DURATION;
                predictionProp.SetValue(playerInstance, HIGH_PREDICTION);
            }

            // Таймер сброса
            if (isTimerRunning)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    isTimerRunning = false;
                    predictionProp.SetValue(playerInstance, originalPrediction);
                }
            }
        }
    }
}
