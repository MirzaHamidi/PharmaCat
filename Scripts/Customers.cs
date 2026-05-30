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
            "Ayyy man how are you?",
            "Hello it's Selman here, I need some help.",
            "Hey, I need some help with a problem.",
            "Hey did you hear about Mirza? They say he is a great game maker.",
            "I heard you have some good stuff for sale.",
            "Hello, have you seen Anil's new game? I heard it's really good like the gravity thing woaw anyways."
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
                        "I haven't slept for days."
                    }
                },
                {
                    "Memory Potion",
                    new List<string>()
                    {
                        "Esin and I have a big exam coming up, I need to remember all my notes!",
                        "I forgot where I buried my gold.",
                        "My memories feel blurry lately.",
                        "I can't remember my own name sometimes."
                    }
                },
                {
                    "Love Potion",
                    new List<string>()
                    {
                        "Do you sell love potions? I want to win over my crush.",
                        "I need help with romance...",
                        "Someone special refuses to notice me.",
                        "I have a friend named Usame... I want him to be my boyfriend but he doesn't like me back :("
                    }
                },
                {
                    "Anti-Curse Potion",
                    new List<string>()
                    {
                        "I think my neighbor cursed me.",
                        "Something is scratching my roof at night.",
                        "My shadow started moving on its own.",
                        "I started to see Usame's face everywhere, I think it's a curse."
                    }
                },
                {
                    "Pain Relief Potion",
                    new List<string>()
                    {
                        "My wrist hurts so much from 3D modeling all day.",
                        "My head is pounding from looking at code all day.",
                        "I stubbed my toe on a table, I need a painkiller."
                    }
                },
                {
                    "Persuasion Potion",
                    new List<string>()
                    {
                        "I need to convince my teacher to give me an A.",
                        "My boss won't give me a raise, help me manipulate him."
                    }
                },
                {
                    "Purification Potion",
                    new List<string>()
                    {
                        "My code is full of bugs, I need to cleanse my computer.",
                        "I accidentally touched grass, I need to cleanse myself."
                    }
                },
                {
                    "Relaxation Potion",
                    new List<string>()
                    {
                        "I have a presentation tomorrow and I'm freaking out.",
                        "Midterms are coming, I just need to chill for a second."
                    }
                },
                {
                    "Soothing Potion",
                    new List<string>()
                    {
                        "My throat hurts from yelling at my computer.",
                        "I need something to calm my nerves after that jump scare."
                    }
                },
                {
                    "Mystic Romance Potion",
                    new List<string>()
                    {
                        "Regular love isn't enough, I want that magical anime romance.",
                        "I want to date a ghost, got anything for that?"
                    }
                },
                {
                    "Holy Water Potion",
                    new List<string>()
                    {
                        "I saw a demon in my fridge.",
                        "My game engine is definitely cursed by an evil spirit, I need holy water to compile it."
                    }
                },
                {
                    "Heart Protection Potion",
                    new List<string>()
                    {
                        "My ex is trying to text me again, I need to protect my feelings.",
                        "I play League of Legends, my heart takes too much damage."
                    }
                },
                {
                    "Passion Potion",
                    new List<string>()
                    {
                        "I lost my motivation to draw on my iPad.",
                        "I need to feel excited about digital game design again."
                    }
                },
                {
                    "Vitality Potion",
                    new List<string>()
                    {
                        "Tolga and I are doing a Game Jam and we haven't slept in 48 hours. Help!",
                        "I want to run a marathon but I get tired walking to the kitchen."
                    }
                },
                {
                    "Focus Potion",
                    new List<string>()
                    {
                        "I have a Game Engines exam tomorrow and I keep watching cat videos.",
                        "I need to lock in and finish my Unreal Engine project."
                    }
                },
                {
                    "Enlightenment Potion",
                    new List<string>()
                    {
                        "I want to understand the universe. Or at least how C# works.",
                        "I'm trying to figure out how to make better e-commerce listings."
                    }
                },
                {
                    "Calm Potion",
                    new List<string>()
                    {
                        "ZBrush just crashed before I could save my 3D model... please, just give me something to calm down.",
                        "My teammates are driving me crazy, I need to not yell at them."
                    }
                },
                {
                    "Clarity Potion",
                    new List<string>()
                    {
                        "My brain is full of fog.",
                        "I need to see the truth behind the matrix."
                    }
                },
                {
                    "Charm Potion",
                    new List<string>()
                    {
                        "I want my boutique products to look irresistible to customers.",
                        "I have an interview and I need maximum charisma."
                    }
                },
                {
                    "Ward Potion",
                    new List<string>()
                    {
                        "I need to keep annoying NPCs away from me.",
                        "I feel like bad luck is following me."
                    }
                },
                {
                    "Wisdom Potion",
                    new List<string>()
                    {
                        "I have an exam on Game Programming and I didn't study! Give me a Wisdom Potion.",
                        "I want to be the smartest person in the room."
                    }
                },
                {
                    "Rage Potion",
                    new List<string>()
                    {
                        "I need to destroy my enemies.",
                        "I want to scream and break things but I need more energy for it."
                    }
                },
                {
                    "Bright Potion",
                    new List<string>()
                    {
                        "My room is too dark and I'm scared of the dark.",
                        "I want to glow like a star!"
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
                "Sleep Potion", "Memory Potion", "Love Potion", "Anti-Curse Potion",
                "Pain Relief Potion", "Persuasion Potion", "Purification Potion", 
                "Relaxation Potion", "Soothing Potion", "Mystic Romance Potion", 
                "Holy Water Potion", "Heart Protection Potion", "Passion Potion", 
                "Vitality Potion", "Focus Potion", "Enlightenment Potion", 
                "Calm Potion", "Clarity Potion", "Charm Potion", 
                "Ward Potion", "Wisdom Potion", "Rage Potion", "Bright Potion"
            };

            WantedPotion = potionTypes[random.Next(potionTypes.Length)];

            MaxPrice = random.Next(15, 46);

            string greeting = greetings[random.Next(greetings.Count)];

            List<string> selectedDialoguePool = potionDialogues[WantedPotion];

            string dialogue = selectedDialoguePool[random.Next(selectedDialoguePool.Count)];

            CurrentDialogue = greeting + "\n" + dialogue;
        }
    }
}