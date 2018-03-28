using System.Collections.Generic;

namespace Climb.Requests.Sets
{
    public class MatchPutRequest
    {
        public int p1Score { get; set; }
        public int p2Score { get; set; }
        public List<int> p1Characters { get; set; }
        public List<int> p2Characters { get; set; }
        public int? stage { get; set; }
    }
}