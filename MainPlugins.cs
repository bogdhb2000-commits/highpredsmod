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

        private const float HIGH_PREDICTION = 0.20f;
        private const float NORMAL_PREDICTION = 0.02f;

        void Update()
        {
            // Получаем инстанс игрока без прямого обращения к GorillaLocomotion.Player
            var playerInstance = GorillaLocomotion.GorillaPlayer.Instance;
            if (playerInstance == null) return;

            // Проверка кнопки правого контроллера
            bool rightPressed = false;
            if (ControllerInputPoller.instance != null)
            {
                rightPressed = ControllerInputPoller.instance.rightControllerPrimaryButton || 
                               ControllerInputPoller.instance.rightControllerSecondaryButton;
            }

            if (rightPressed && !isTimerRunning)
            {
                isTimerRunning = true;
                timer = TIMER_DURATION;
                playerInstance.predictionTime = HIGH_PREDICTION;
            }

            if (isTimerRunning)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    isTimerRunning = false;
                    playerInstance.predictionTime = NORMAL_PREDICTION;
                }
            }
        }
    }
}
