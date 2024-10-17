using Unity.Cinemachine;
using UnityEngine;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Camera/Cinemachine Priority")]
    public class CMPriority : MonoBehaviour
    {
        public CinemachineCamera Camera;
        public int priority = 15;

        public void SetPriority(bool value)
        {
            if (value)
            {
                Camera.Priority.Value = priority;
                Camera.Priority.Enabled = true;
            }
            else
            {
                Camera.Priority.Value = -1;
                Camera.Priority.Enabled = false;
            }
        }

        private void Reset()
        {
            Camera = GetComponent<CinemachineCamera>();
        }
    }
}