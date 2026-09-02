using BepInEx;
using UnityEngine;

namespace HighPredsMod
{
    [BepInPlugin("com.yourname.highpredsmod", "High Preds 3 Sec Timer", "1.0.0")]
    public class MainPlugins : BaseUnityPlugin
    {
        private bool isTimerRunning = false;
        private float timer = 0f;
        private const float TIMER_DURATION = 3f;

        private const float HIGH_PREDICTION = 0.20f;
        private const float NORMAL_PREDICTION = 0.02f;

        private Component playerInstance;
        private System.Reflection.FieldInfo predField;

        void Update()
        {
            // Ленивая инициализация: ищем игрока только до тех пор, пока не найдем
            if (playerInstance == null)
            {
                FindPlayer();
                if (playerInstance == null) return;
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

        private void FindPlayer()
        {
            var allObjects = Object.FindObjectsOfType<MonoBehaviour>();
            foreach (var obj in allObjects)
            {
                if (obj.GetType().Name == "Player" && obj.GetType().Namespace == "GorillaLocomotion")
                {
                    playerInstance = obj;
                    predField = obj.GetType().GetField("predictionTime", 
                        System.Reflection.BindingFlags.Public | 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    break;
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
            SetPrediction(NORMAL_PREDICTION);
        }

        private void SetPrediction(float value)
        {
            if (predField != null && playerInstance != null)
            {
                predField.SetValue(playerInstance, value);
            }
        }
    }
}
