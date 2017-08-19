using System;
using System.Collections.Generic;
using Climb.Core.Extensions;

namespace Climb.Core
{
    public struct Set
    {
        public readonly int player1;
        public readonly int player2;

        public Set(int player1, int player2)
        {
            this.player1 = player1;
            this.player2 = player2;
        }
    }

    public class Round
    {
        public readonly DateTime dueDate;
        public readonly HashSet<Set> sets = new HashSet<Set>();

        public Round(DateTime dueDate)
        {
            this.dueDate = dueDate;
        }
    }

    public class Schedule
    {
        public readonly List<Round> rounds = new List<Round>();
    }

    public static class ScheduleGenerator
    {
        public const int Bye = -1;

        public static Schedule Generate(int rounds, ICollection<int> users)
        {
            var participants = GetParticipants(users);

            int fullSeasonSegments = rounds / (participants.Count - 1);
            int partialRounds = rounds % (participants.Count - 1);

            var schedule = new Schedule();
            for (int i = 0; i < fullSeasonSegments; i++)
            {
                schedule.rounds.AddRange(GenerateRounds(participants.Count - 1, participants));
            }
            schedule.rounds.AddRange(GenerateRounds(partialRounds, participants));

            return schedule;
        }

        private static List<int> GetParticipants(ICollection<int> users)
        {
            var participants = new List<int>(users);
            if (participants.Count % 2 == 1)
            {
                participants.Add(Bye);
            }

            return participants;
        }

        private static IEnumerable<Round> GenerateRounds(int roundCount, List<int> participants)
        {
            var rounds = new List<Round>();

            participants.Shuffle();

            for (int i = 0; i < roundCount; i++)
            {
                var halfCount = participants.Count / 2;
                var firstHalf = participants.GetRange(0, halfCount);
                var secondHalf = participants.GetRange(halfCount, halfCount);
                secondHalf.Reverse();

                var round = new Round(DateTime.Now);
                for (int j = 0; j < halfCount; j++)
                {
                    var set = new Set(firstHalf[j], secondHalf[j]);
                    round.sets.Add(set);
                }

                rounds.Add(round);

                participants.Insert(1, participants[participants.Count - 1]);
                participants.RemoveAt(participants.Count - 1);
            }

            return rounds;
        }

    }
}
