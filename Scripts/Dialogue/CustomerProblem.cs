using System.Collections.Generic;

namespace PharmaCat.Scripts
{
    public class CustomerProblem
    {
        public string Id { get; set; } = "";
        public List<string> Dialogues { get; set; } = new List<string>();
        public List<string> AcceptedPotions { get; set; } = new List<string>();
    }
}