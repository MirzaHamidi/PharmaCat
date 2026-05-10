using System;
using System.Collections.Generic;

namespace PharmaCat.Scripts
{
    public class Customers
    {
        private static Random random = new Random();

        public string WantedPotion { get; private set; }
        public int MaxPrice { get; private set; }
        public string CurrentDialogue { get; private set; }

        private List<string> greetings = new List<string>()
        {
            "Hi there!",
            "Greetings!",
            "GEE WEEES!!",
            "AHHH I am dying",
            "Are you a Freaking Cat??",
            "ayyy man how are you?",
            "Hello it's Selman here, I need some help.",
            "Hey, I need some help with a problem.",
            "Hey did you hear about Mirza? They say he is a great game maker.",
            "I heard you have some good stuff for sale.",
            "Hello, have you seen Anıl's new game? I heard it's really good like the gravity thing woaw anyways.",
          
        };

        private Dictionary<string, List<string>> potionDialogues =
            new Dictionary<string, List<string>>()
            {
                {
                    "Sleep Potion",
                    new List<string>()
                    {
                        "I need something for sleep...",
                        "I need something to cure my insomnia.",
                        "I haven’t slept for days."
                    }
                },

                {
                    "Memory Potion",
                    new List<string>()
                    {
                        "I can't find my wallet.",
                        "I forgot where I buried my gold.",
                        "My memories feel blurry lately.",
                        "I can't remember my own name sometimes.",
                        "I forgot my anniversary, I need a potion to fix this.",
                    }
                },

                {
                    "Love Potion",
                    new List<string>()
                    {
                        "Do you sell love potions? I want to win over my crush.",
                        "I need help with romance...",
                        "Someone special refuses to notice me.",
                        "My wife is cheating on me with my step dad",
                        "I need to make my self fell in love with myself again :(",
                        "I have a friend named Usame... I want him to be my boyfriend but he doesn't like me back :(",
                    }
                },

                {
                    "Anti-Curse Potion",
                    new List<string>()
                    {
                        "I think my neighbor cursed me.",
                        "Something is scratching my roof at night.",
                        "My shadow started moving on its own.",
                        "I started to see usame's face everywhere, I think it's a curse.",
                    }
                }
            };

        public Customers()
        {
            GenerateDialogue();
        }

        private void GenerateDialogue()
        {
            string[] potionTypes =
            {
                "Sleep Potion",
                "Memory Potion",
                "Love Potion",
                "Anti-Curse Potion"
            };

            WantedPotion =
                potionTypes[random.Next(potionTypes.Length)];

            MaxPrice =
                random.Next(10, 31);

            string greeting =
                greetings[random.Next(greetings.Count)];

            List<string> selectedDialoguePool =
                potionDialogues[WantedPotion];

            string dialogue =
                selectedDialoguePool[
                    random.Next(selectedDialoguePool.Count)
                ];

            CurrentDialogue =
                greeting + "\n" + dialogue;
        }
    }
}