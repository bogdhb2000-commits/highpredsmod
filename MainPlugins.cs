using BepInEx;
using UnityEngine;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;

namespace HighPredsMod
{
    [BepInPlugin("com.yourname.highpreds", "High Preds 3 Sec Timer", "1.0.0")]
    public class MainPlugin : BaseUnityPlugin
    {
        private float defaultPredictionTime = 0.02f;
        private bool isInitialized = false;

        private bool isModActive = false;
        private float timer = 0f;
        private bool wasJoystickPressed = false;

        void Update()
        {
            if (GorillaLocomotion.Player.Instance == null) return;

            if (!isInitialized)
            {
                defaultPredictionTime = GorillaLocomotion.Player.Instance.predictionTime;
                isInitialized = true;
            }

            // Считываем нажатие правого джойстика
            bool isJoystickPressedNow = GorillaLocomotion.ControllerInputPoller.instance.rightControllerPrimary2DAxisClick;

            // Если кликнули по джойстику и мод еще не запущен
            if (isJoystickPressedNow && !wasJoystickPressed && !isModActive)
            {
                isModActive = true;
                timer = 3f; // Задаем время работы в секундах
                
                // Легкая вибрация в правый контроллер
                GorillaTagger.Instance.StartVibration(false, 0.2f, 0.1f);
            }

            wasJoystickPressed = isJoystickPressedNow;

            // Логика работы таймера
            if (isModActive)
            {
                // Пока таймер идет, держим High Preds
                GorillaLocomotion.Player.Instance.predictionTime = 0.5f;

                // Уменьшаем таймер каждую секунду
                timer -= Time.deltaTime;

                // Если 3 секунды истекли
                if (timer <= 0f)
                {
                    isModActive = false;
                    GorillaLocomotion.Player.Instance.predictionTime = defaultPredictionTime; // Возвращаем обычную физику
                    
                    // Двойная короткая вибрация
                    GorillaTagger.Instance.StartVibration(false, 0.1f, 0.05f);
                }
            }
        }
    }
}
