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

        private const float HIGH_PREDICTION = 0.20f; // 200 мс
        private const float NORMAL_PREDICTION = 0.02f; // Дефолт

        void Update()
        {
            // Используем полный путь GorillaLocomotion.Player, чтобы избежать CS0246/CS0103
            if (GorillaLocomotion.Player.Instance == null) return;

            // Используем стандартные поля ControllerInputPoller без rightControllerPrimaryTwoAxisClick
            bool rightStickPressed = false;
            if (ControllerInputPoller.instance != null)
            {
                rightStickPressed = ControllerInputPoller.instance.rightControllerPrimaryButton || 
                                    ControllerInputPoller.instance.rightControllerSecondaryButton;
            }

            if (rightStickPressed && !isTimerRunning)
            {
                StartHighPreds();
            }

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
            GorillaLocomotion.Player.Instance.predictionTime = HIGH_PREDICTION;
        }

        private void ResetPreds()
        {
            isTimerRunning = false;
            GorillaLocomotion.Player.Instance.predictionTime = NORMAL_PREDICTION;
        }
    }
}
