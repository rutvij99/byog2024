using UnityEngine;

namespace Entity
{
    [CreateAssetMenu(fileName = "EntityDataAsset", menuName = "IRIS/Entity/Create Entity Data Asset", order = 0)]
    public class EntityDataAsset: ScriptableObject
    {
        public string EntityName;
        public int MaxHealth=100;
    }
}