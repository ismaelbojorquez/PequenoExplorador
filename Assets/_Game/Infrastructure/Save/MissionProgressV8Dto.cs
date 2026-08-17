using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class MissionProgressV8Dto
    {
        [SerializeField] private string id;
        [SerializeField] private int status;
        [SerializeField] private long activationSequence;
        [SerializeField] private MissionObjectiveProgressV8Dto[] objectives;
        public string Id => id;
        public int Status => status;
        public long ActivationSequence => activationSequence;
        public MissionObjectiveProgressV8Dto[] Objectives => objectives;
        public static MissionProgressV8Dto Create(string missionId, int missionStatus, long sequence,
            MissionObjectiveProgressV8Dto[] objectiveProgress) => new MissionProgressV8Dto
            { id = missionId, status = missionStatus, activationSequence = sequence, objectives = objectiveProgress };
    }
}
