using Newtonsoft.Json;

namespace Climb.Models
{
    public class Stage
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int GameID { get; set; }

        [JsonIgnore]
        public Game Game { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
