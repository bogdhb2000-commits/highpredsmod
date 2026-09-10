using System;
using BepInEx;
using UnityEngine;

namespace MySilentPullMod
{
    [BepInPlugin("com.username.silentpullmod", "Silent Pull Mod", "1.0.0")]
    public class SilentPullMod : BaseUnityPlugin
    {
        // Сила смещения
        public static float playspaceAbusePower = 0.3f;

        void Update()
        {
            // Проверка нажатия кнопки X на левом контроллере
            if (ControllerInputPoller.instance != null && ControllerInputPoller.instance.leftControllerPrimaryButton)
            {
                PlayspaceAbuse();
            }
        }

        private void PlayspaceAbuse()
        {
            GorillaTagger.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * playspaceAbusePower;
        }
    }
}
