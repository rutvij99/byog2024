using System;
using UnityEngine;

namespace Bosses
{
    public class BossTarget : MonoBehaviour
    {
        public static BossTarget Target { get; private set; }

        private void Awake()
        {
            Target = this;
        }
    }
}