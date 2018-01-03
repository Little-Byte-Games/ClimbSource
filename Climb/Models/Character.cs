using Newtonsoft.Json;

namespace Climb.Models
{
    public class Character
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int GameID { get; set; }
        public string PicKey { get; set; }

        [JsonIgnore]
        public Game Game { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}