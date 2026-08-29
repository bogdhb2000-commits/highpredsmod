using BepInEx;
using UnityEngine;

namespace HighPredsMod
{
    [BepInPlugin("com.yourname.highpreds", "High Preds 3 Sec Timer", "1.0.0")]
    public class MainPlugin : BaseUnityPlugin
    {
        private bool isModActive = false;
        private float timer = 0f;

        void Update()
        {
            // Самый надежный способ считать нажатие без использования ControllerInputPoller
            // Мод активируется, если зажаты правый Shift и пробел на клавиатуре (для теста), 
            // либо если сработает кнопка джойстика через Unity Input
            if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.RightShift) && !isModActive)
            {
                isModActive = true;
                timer = 3f;
            }

            if (isModActive)
            {
                // Находим объект игрока напрямую в движке Unity, обходя любые старые/новые библиотеки GorillaLocomotion
                var player = FindObjectOfType<Component>();
                if (player != null)
                {
                    // Ищем поле predictionTime через рефлексию движка Unity, чтобы не зависеть от версии игры
                    var field = player.GetType().GetField("predictionTime");
                    if (field != null)
                    {
                        field.SetValue(player, 0.5f);
                    }
                }

                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    isModActive = false;
                    var playerReset = FindObjectOfType<Component>();
                    if (playerReset != null)
                    {
                        var field = playerReset.GetType().GetField("predictionTime");
                        if (field != null)
                        {
                            field.SetValue(playerReset, 0.02f); // Возвращаем стандартное значение
                        }
                    }
                }
            }
        }
    }
}
