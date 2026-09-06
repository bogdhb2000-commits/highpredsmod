using System;
using BepInEx;
using UnityEngine;
using GorillaLocomotion;

namespace MySilentPullMod
{
    [BepInPlugin("com.username.silentpullmod", "Silent Pull Mod", "1.0.0")]
    public class SilentPullMod : BaseUnityPlugin
    {
        // Настройки Silent Pull
        public static float pullStrength = 8f;

        // Настройки Playspace Abuse
        public static float playspaceAbusePower = 0.05f;

        void Update()
        {
            // 1. Silent Pull на кнопку B (Правая рука - Secondary Button)
            if (ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                VRRig target = GetClosestPlayer();
                if (target != null)
                {
                    Vector3 direction = (target.transform.position - GorillaTagger.Instance.bodyCollider.transform.position).normalized;
                    
                    // Явно указываем пространство имён GorillaLocomotion
                    Rigidbody playerRigidbody = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>();
                    if (playerRigidbody != null)
                    {
                        playerRigidbody.velocity = direction * pullStrength;
                    }
                }
            }

            // 2. Playspace Abuse на кнопку X (Левая рука - Primary Button)
            if (ControllerInputPoller.instance != null && ControllerInputPoller.instance.leftControllerPrimaryButton)
            {
                PlayspaceAbuse();
            }
        }

        private void PlayspaceAbuse()
        {
            GorillaTagger.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * playspaceAbusePower;
        }

        private VRRig GetClosestPlayer()
        {
            VRRig closest = null;
            float minDistance = float.MaxValue;
            Vector3 myPos = GorillaTagger.Instance.bodyCollider.transform.position;

            foreach (VRRig rig in UnityEngine.Object.FindObjectsOfType<VRRig>())
            {
                if (rig != null && rig != GorillaTagger.Instance.offlineVRRig && !rig.isOfflineVRRig)
                {
                    float dist = Vector3.Distance(myPos, rig.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closest = rig;
                    }
                }
            }

            return closest;
        }
    }
}
