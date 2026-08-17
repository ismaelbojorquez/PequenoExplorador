using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class MissionObjectiveProgressV8Dto
    {
        [SerializeField] private string id;
        [SerializeField] private int count;
        public string Id => id;
        public int Count => count;
        public static MissionObjectiveProgressV8Dto Create(string objectiveId, int value) =>
            new MissionObjectiveProgressV8Dto { id = objectiveId, count = value };
    }
}
