namespace DUJAL.Systems.Dialogue 
{
    using System;
    using UnityEngine;
    [Serializable]
    public class GroupSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}
