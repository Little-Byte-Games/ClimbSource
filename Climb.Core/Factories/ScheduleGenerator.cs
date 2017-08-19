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

    public static class ScheduleGenerator
    {
        public const int Bye = -1;

        public static List<Round> Generate(int roundCount, ICollection<int> users, DateTime startDate)
        {
            var participants = GetParticipants(users);

            int fullSeasonSegments = roundCount / (participants.Count - 1);
            int partialRounds = roundCount % (participants.Count - 1);

            var rounds = new List<Round>();
            IEnumerable<Round> createdRounds;
            for (int i = 0; i < fullSeasonSegments; i++)
            {
                createdRounds = GenerateRounds(participants.Count - 1, participants, ref startDate);
                rounds.AddRange(createdRounds);
            }

            createdRounds = GenerateRounds(partialRounds, participants, ref startDate);
            rounds.AddRange(createdRounds);

            return rounds;
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

        private static IEnumerable<Round> GenerateRounds(int roundCount, List<int> participants, ref DateTime startDate)
        {
            var rounds = new List<Round>();

            participants.Shuffle();

            for (int i = 0; i < roundCount; i++)
            {
                var halfCount = participants.Count / 2;
                var firstHalf = participants.GetRange(0, halfCount);
                var secondHalf = participants.GetRange(halfCount, halfCount);
                secondHalf.Reverse();

                startDate = startDate.AddDays(7);
                var round = new Round(startDate);
                for (int j = 0; j < halfCount; j++)
                {
                    var player1 = firstHalf[j];
                    var player2 = secondHalf[j];
                    var set = new Set(player1, player2);
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
