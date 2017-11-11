using System;
using System.Collections.Generic;

namespace Climb.Core
{
    public class Round
    {
        public readonly DateTime dueDate;
        public readonly HashSet<Set> sets = new HashSet<Set>();

        public Round(DateTime dueDate)
        {
            this.dueDate = dueDate;
        }
    }
}