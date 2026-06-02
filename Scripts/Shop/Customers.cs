using System;
using System.Collections.Generic;

namespace PharmaCat.Scripts
{
    public class Customers
    {
        private static readonly Random random = new Random();
        public string WantedPotion { get; private set; }

        public List<string> AcceptablePotions { get; private set; } = new List<string>();
        public int MaxPrice { get; private set; }
        public string CurrentDialogue { get; private set; }
        public string ProblemId { get; private set; }

        private HashSet<string> boughtPotions = new HashSet<string>();

        public Customers()
        {
            GenerateDialogue();
        }

        private void GenerateDialogue()
        {
            CustomerProblem selectedProblem = CustomerDialogueDatabase.GetRandomProblem();

            ProblemId = selectedProblem.Id;
            AcceptablePotions = new List<string>(selectedProblem.AcceptedPotions);
            WantedPotion = AcceptablePotions.Count > 0 ? AcceptablePotions[0] : "";

            MaxPrice = random.Next(20, 150);

            string greeting = CustomerDialogueDatabase.GetRandomGreeting();
            string dialogue = CustomerDialogueDatabase.GetRandomDialogue(selectedProblem);

            CurrentDialogue = greeting + "\n" + dialogue;
        }

        public bool AcceptsPotion(string potionName)
        {
            if (string.IsNullOrEmpty(potionName))
                return false;

            return AcceptablePotions.Contains(potionName);
        }

        public bool HasBoughtPotion(string potionName)
        {
            return boughtPotions.Contains(potionName);
        }

        public void MarkPotionBought(string potionName)
        {
            if (!string.IsNullOrEmpty(potionName))
                boughtPotions.Add(potionName);
        }

        public string GetAcceptablePotionText()
        {
            return string.Join(", ", AcceptablePotions);
        }
    }
}
