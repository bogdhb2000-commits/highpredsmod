using BepInEx;
using UnityEngine;
using GorillaLocomotion;

namespace HighPredsMod
{
    [BepInPlugin("com.yourname.highpredsmod", "High Preds 3 Sec Timer", "1.0.0")]
    public class MainPlugins : BaseUnityPlugin
    {
        private bool isTimerRunning = false;
        private float timer = 0f;
        private const float TIMER_DURATION = 3f;
        
        // 100 предикшена (0.1f = 100ms в секундах)
        private const float HIGH_PREDICTION = 0.1f; 
        
        // Значение по умолчанию из настроек игры / SteamVR
        private float originalPrediction = 0.02f;
        private bool originalSaved = false;

        void Update()
        {
            // Используем полное имя GorillaLocomotion.Player
            if (GorillaLocomotion.Player.Instance == null) return;

            // Сохраняем стандартный предикшен при первом кадре
            if (!originalSaved)
            {
                originalPrediction = GorillaLocomotion.Player.Instance.predictionTime;
                originalSaved = true;
            }

            // Опрос кнопок контроллера
            bool rightPressed = false;
            if (ControllerInputPoller.instance != null)
            {
                rightPressed = ControllerInputPoller.instance.rightControllerPrimaryButton || 
                               ControllerInputPoller.instance.rightControllerSecondaryButton;
            }

            // Старт таймера
            if (rightPressed && !isTimerRunning)
            {
                StartHighPreds();
            }

            // Отсчет времени
            if (isTimerRunning)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    ResetPreds();
                }
            }
        }

        private void StartHighPreds()
        {
            isTimerRunning = true;
            timer = TIMER_DURATION;
            SetPrediction(HIGH_PREDICTION);
        }

        private void ResetPreds()
        {
            isTimerRunning = false;
            SetPrediction(originalPrediction); 
        }

        private void SetPrediction(float value)
        {
            if (GorillaLocomotion.Player.Instance != null)
            {
                GorillaLocomotion.Player.Instance.predictionTime = value;
            }
        }
    }
}
