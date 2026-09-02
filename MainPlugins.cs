using BepInEx;
using UnityEngine;
using GorillaLocomotion;

namespace HighPredsTimer
{
    [BepInPlugin("com.yourname.highpredstimer", "High Preds 3 Sec Timer", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool isTimerRunning = false;
        private float timer = 0f;
        private const float TIMER_DURATION = 3f;

        // Значения предикшена (настрой под свои нужды)
        private const float HIGH_PREDICTION = 0.05f; // Значение во время таймера
        private const float NORMAL_PREDICTION = 0.02f; // Исходное значение

        void Update()
        {
            // Проверяем доступность локального игрока
            if (Player.Instance == null) return;

            // Кликом по правому джойстику запускаем процесс
            bool rightStickClicked = ControllerInputPoller.instance != null && 
                                     ControllerInputPoller.instance.rightControllerPrimaryTwoAxisClick;

            if (rightStickClicked && !isTimerRunning)
            {
                StartHighPreds();
            }

            // Отсчет 3 секунд
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

            // Включаем повышенный предикшен
            Player.Instance.predictionTime = HIGH_PREDICTION;
            Logger.LogInfo($"High Preds включен ({HIGH_PREDICTION}) на {TIMER_DURATION} сек.");
        }

        private void ResetPreds()
        {
            isTimerRunning = false;

            // Возвращаем обычный предикшен
            Player.Instance.predictionTime = NORMAL_PREDICTION;
            Logger.LogInfo($"Предикшен возвращен к норме ({NORMAL_PREDICTION}).");
        }
    }
}
