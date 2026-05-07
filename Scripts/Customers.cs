using System;
using System.Collections.Generic;

namespace PharmaCat.Scripts
{
    public class Customers
    {
        private static Random random = new Random();

        public string CurrentDialogue { get; private set; }

        private List<string> greetings = new List<string>()
        {
            "Hi there!",
            "Greetings!",
            "GEE WEEES!!",
            "AHHH I am dying",
            "Are you a Freaking Cat??",
            "I heard you have potions for sale.",
            "Hello it's Selman here, I need some help.",
            "Hey, I need some help with a problem.",
            "Hey did you hear about Mirza? They say he is a great game maker.",
            "I heard you have some good stuff for sale."
        };

        private List<string> dialoguePool = new List<string>()
        {
            "I need something for sleep...",
            "My bird doesn't squawk anymore, can you help?",
            "My friend said he needs a potion for his wife's birthday.",
            "Do you sell memory potions?",
            "I think my neighbor cursed me.",
            "Something is scratching my roof at night.",
            "I have a headache, do you have anything for that?",
            "Do you sell love potions? I want to win over my crush.",
            "I need something to cure my insomnia."
        };

        public Customers()
        {
            GenerateDialogue();
        }

        private void GenerateDialogue()
        {
            string greeting =
                greetings[random.Next(greetings.Count)];

            string dialogue =
                dialoguePool[random.Next(dialoguePool.Count)];

            CurrentDialogue = greeting + "\n" + dialogue;
        }
    }
}