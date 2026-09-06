using System;
using BepInEx;
using UnityEngine;

namespace MySilentPullMod
{
    [BepInPlugin("com.username.silentpullmod", "Silent Pull Mod", "1.0.0")]
    public class SilentPullMod : BaseUnityPlugin
    {
        // Настройки Playspace Abuse
        public static float playspaceAbusePower = 0.08f;

        void Update()
        {
            // Активация при нажатии на ПРАВЫЙ джойстик (Right Controller Click)
            if (ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightControllerPrimary2DAxisClick)
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
