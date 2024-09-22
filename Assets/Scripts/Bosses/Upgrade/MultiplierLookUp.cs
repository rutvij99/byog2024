using System.Collections.Generic;
using UnityEngine;

namespace Bosses.Upgrade
{
    [CreateAssetMenu(fileName = "MultiplierLookUp", menuName = "IRIS/MultiplierLookUp", order = 0)]
    public class MultiplierLookUp : ScriptableObject
    {
        public List<float> Table;
    }
}