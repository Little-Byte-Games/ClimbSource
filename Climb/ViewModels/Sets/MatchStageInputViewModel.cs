using System.Collections.Generic;
using Climb.Models;

namespace Climb.ViewModels.Sets
{
    public class MatchStageInputViewModel
    {
        public readonly int matchIndex;
        public readonly int stageIndex;
        public readonly IEnumerable<Stage> stages;

        public MatchStageInputViewModel(int matchIndex, int stageIndex, IEnumerable<Stage> stages)
        {
            this.matchIndex = matchIndex;
            this.stageIndex = stageIndex;
            this.stages = stages;
        }
    }
}