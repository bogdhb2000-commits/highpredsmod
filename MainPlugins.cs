using BepInEx;
using UnityEngine;
using UnityEngine.XR;
using GorillaLocomotion;

namespace MySilentPullMod
{
    [BepInPlugin("com.username.silentpullmod", "Silent Pull Mod", "1.0.0")]
    public class SilentPullMod : BaseUnityPlugin
    {
        public static float pullStrength = 15f; 
        private VRRig currentTarget = null;

        void Update()
        {
            bool isJoystickPressed = IsJoystickClicked(XRNode.RightHand);

            if (isJoystickPressed)
            {
                if (currentTarget == null)
                {
                    currentTarget = GetClosestPlayer();
                }

                if (currentTarget != null)
                {
                    Vector3 myPosition = GorillaTagger.Instance.bodyCollider.transform.position;
                    currentTarget.transform.position = Vector3.Lerp(
                        currentTarget.transform.position, 
                        myPosition, 
                        Time.deltaTime * pullStrength
                    );
                }
            }
            else
            {
                currentTarget = null;
            }
        }

        private VRRig GetClosestPlayer()
        {
            VRRig closest = null;
            float minDistance = float.MaxValue;
            Vector3 myPos = GorillaTagger.Instance.bodyCollider.transform.position;

            VRRig[] allRigs = Object.FindObjectsOfType<VRRig>();
            foreach (VRRig rig in allRigs)
            {
                if (rig != null && rig != GorillaTagger.Instance.offlineVRRig)
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

        private bool IsJoystickClicked(XRNode node)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(node);
            if (device.isValid && device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool isPressed))
            {
                return isPressed;
            }
            return false;
        }
    }
}
