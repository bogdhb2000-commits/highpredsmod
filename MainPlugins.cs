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
        
        // Поле для сохранения оригинального значения из настроек игры/SteamVR
        private float originalPrediction = 0.02f;
        private bool originalSaved = false;

        void Update()
        {
            // Прямое обращение к игроку без использования FindObjectsOfType
            if (Player.Instance == null) return;

            // Сохраняем дефолтный предикшен при первом старте
            if (!originalSaved)
            {
                originalPrediction = Player.Instance.predictionTime;
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
            // Возвращаем исходное значение из настроек игры/SteamVR
            SetPrediction(originalPrediction); 
        }

        private void SetPrediction(float value)
        {
            if (Player.Instance != null)
            {
                Player.Instance.predictionTime = value;
            }
        }
    }
}
