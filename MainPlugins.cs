using BepInEx;
using UnityEngine;
using GorillaLocomotion;
using System.Reflection;

namespace HighPredsMod
{
    [BepInPlugin("com.yourname.highpredsmod", "High Preds 3 Sec Timer", "1.0.0")]
    public class MainPlugins : BaseUnityPlugin
    {
        private bool isTimerRunning = false;
        private float timer = 0f;
        private const float TIMER_DURATION = 3f;

        private const float HIGH_PREDICTION = 0.20f; // 200 мс
        private const float NORMAL_PREDICTION = 0.02f; // Норма

        private FieldInfo predField;

        void Awake()
        {
            // Получаем доступ к закрытому полю predictionTime через Reflection
            predField = typeof(Player).GetField("predictionTime", 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        void Update()
        {
            if (Player.Instance == null) return;

            // Проверка клика по правому джойстику
            bool rightStickClicked = ControllerInputPoller.instance != null && 
                                     ControllerInputPoller.instance.rightControllerPrimaryTwoAxisClick;

            if (rightStickClicked && !isTimerRunning)
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
            SetPrediction(HIGH_PREDICTION);
        }

        private void ResetPreds()
        {
            isTimerRunning = false;
            SetPrediction(NORMAL_PREDICTION);
        }

        private void SetPrediction(float value)
        {
            if (predField != null)
            {
                predField.SetValue(Player.Instance, value);
            }
            else
            {
                // Запасной вариант, если поле публичное
                Player.Instance.predictionTime = value;
            }
        }
    }
}
