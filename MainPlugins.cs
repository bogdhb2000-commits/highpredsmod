using BepInEx;
using UnityEngine;
using System;

namespace HighPredsMod
{
    [BepInPlugin("com.yourname.highpreds", "High Preds 3 Sec Timer", "1.0.0")]
    public class MainPlugin : BaseUnityPlugin
    {
        private bool isModActive = false;
        private float timer = 0f;

        void Update()
        {
            // Считываем нажатие правого триггера (курка) на контроллере Gorilla Tag через стандартный Unity Input
            bool isTriggerPressed = false;
            
            // Проверяем стандартную ось правого курка в Unity
            if (Input.GetAxis("RightTrigger") > 0.5f || Input.GetMouseButton(0)) 
            {
                isTriggerPressed = true;
            }

            // Если курок нажат и мод еще не активен — включаем таймер
            if (isTriggerPressed && !isModActive)
            {
                isModActive = true;
                timer = 3f;
            }

            if (isModActive)
            {
                // Находим объект игрока напрямую в движке Gorilla Tag
                var player = FindObjectOfType(Type.GetType("GorillaLocomotion.Player, Assembly-CSharp"));
                if (player != null)
                {
                    // Меняем predictionTime на 0.5
                    var field = player.GetType().GetField("predictionTime");
                    if (field != null)
                    {
                        field.SetValue(player, 0.5f);
                    }
                }

                // Отсчитываем 3 секунды
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    isModActive = false;
                    
                    // Когда время вышло, возвращаем стандартную физику (0.02)
                    var playerReset = FindObjectOfType(Type.GetType("GorillaLocomotion.Player, Assembly-CSharp"));
                    if (playerReset != null)
                    {
                        var field = playerReset.GetType().GetField("predictionTime");
                        if (field != null)
                        {
                            field.SetValue(playerReset, 0.02f);
                        }
                    }
                }
            }
        }
    }
}
