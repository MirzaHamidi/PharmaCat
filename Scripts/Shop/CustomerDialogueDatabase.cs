using System;
using System.Collections.Generic;

namespace PharmaCat.Scripts
{
    public static class CustomerDialogueDatabase
    {
        private static readonly Random random = new Random();

        public static readonly List<string> Greetings = new List<string>
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

        public static readonly List<CustomerProblem> Problems = new List<CustomerProblem>
        {
            new CustomerProblem
            {
                Id = "Fear Insomnia",
                Dialogues = new List<string>
                {
                    "I can't sleep because I feel cursed.",
                    "Something is scratching my roof at night and I haven't slept for days.",
                    "I am scared of the dark and I need to sleep."
                },
                AcceptedPotions = new List<string>
                {
                    "Sleep Potion", "Anti-Curse Potion", "Calm Potion", "Ward Potion", "Holy Water Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Normal Insomnia",
                Dialogues = new List<string>
                {
                    "I need something for sleep...",
                    "I need something to cure my insomnia.",
                    "I haven't slept for days."
                },
                AcceptedPotions = new List<string>
                {
                    "Sleep Potion", "Relaxation Potion", "Calm Potion", "Soothing Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Memory Problem",
                Dialogues = new List<string>
                {
                    "Esin and I have a big exam coming up, I need to remember all my notes!",
                    "I forgot where I buried my gold.",
                    "My memories feel blurry lately.",
                    "I can't remember my own name sometimes."
                },
                AcceptedPotions = new List<string>
                {
                    "Memory Potion", "Focus Potion", "Clarity Potion", "Wisdom Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Romance Problem",
                Dialogues = new List<string>
                {
                    "Do you sell love potions? I want to win over my crush.",
                    "I need help with romance...",
                    "Someone special refuses to notice me.",
                    "I have a friend named Usame... I want him to be my boyfriend but he doesn't like me back :("
                },
                AcceptedPotions = new List<string>
                {
                    "Love Potion", "Charm Potion", "Mystic Romance Potion", "Passion Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Curse Problem",
                Dialogues = new List<string>
                {
                    "I think my neighbor cursed me.",
                    "Something is scratching my roof at night.",
                    "My shadow started moving on its own.",
                    "I started to see Usame's face everywhere, I think it's a curse."
                },
                AcceptedPotions = new List<string>
                {
                    "Anti-Curse Potion", "Holy Water Potion", "Purification Potion", "Ward Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Pain Problem",
                Dialogues = new List<string>
                {
                    "My wrist hurts so much from 3D modeling all day.",
                    "My head is pounding from looking at code all day.",
                    "I stubbed my toe on a table, I need a painkiller."
                },
                AcceptedPotions = new List<string>
                {
                    "Pain Relief Potion", "Soothing Potion", "Relaxation Potion", "Vitality Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Stress Problem",
                Dialogues = new List<string>
                {
                    "I have a presentation tomorrow and I'm freaking out.",
                    "Midterms are coming, I just need to chill for a second.",
                    "ZBrush just crashed before I could save my 3D model... please, just give me something to calm down.",
                    "My teammates are driving me crazy, I need to not yell at them."
                },
                AcceptedPotions = new List<string>
                {
                    "Relaxation Potion", "Calm Potion", "Soothing Potion", "Clarity Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Motivation Problem",
                Dialogues = new List<string>
                {
                    "I lost my motivation to draw on my iPad.",
                    "I need to feel excited about digital game design again.",
                    "Tolga and I are doing a Game Jam and we haven't slept in 48 hours. Help!"
                },
                AcceptedPotions = new List<string>
                {
                    "Passion Potion", "Vitality Potion", "Focus Potion", "Bright Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Darkness Problem",
                Dialogues = new List<string>
                {
                    "My room is too dark and I'm scared of the dark.",
                    "I want to glow like a star!",
                    "I saw a demon in my fridge."
                },
                AcceptedPotions = new List<string>
                {
                    "Bright Potion", "Holy Water Potion", "Ward Potion", "Anti-Curse Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Exam Focus Problem",
                Dialogues = new List<string>
                {
                    "I have a Game Engines exam tomorrow and I keep watching cat videos.",
                    "I need to lock in and finish my Unreal Engine project.",
                    "I have an exam on Game Programming and I didn't study!"
                },
                AcceptedPotions = new List<string>
                {
                    "Focus Potion", "Wisdom Potion", "Memory Potion", "Clarity Potion", "Enlightenment Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Persuasion Problem",
                Dialogues = new List<string>
                {
                    "I need to convince my teacher to give me an A.",
                    "My boss won't give me a raise, help me manipulate him.",
                    "I have an interview and I need maximum charisma."
                },
                AcceptedPotions = new List<string>
                {
                    "Persuasion Potion", "Charm Potion", "Enlightenment Potion"
                }
            },
            new CustomerProblem
            {
                Id = "Anger Problem",
                Dialogues = new List<string>
                {
                    "I need to destroy my enemies.",
                    "I want to scream and break things but I need more energy for it.",
                    "My code is full of bugs, I need to cleanse my computer."
                },
                AcceptedPotions = new List<string>
                {
                    "Rage Potion", "Calm Potion", "Purification Potion", "Focus Potion"
                }
            }
        };

        public static string GetRandomGreeting()
        {
            return Greetings[random.Next(Greetings.Count)];
        }

        public static CustomerProblem GetRandomProblem()
        {
            return Problems[random.Next(Problems.Count)];
        }

        public static string GetRandomDialogue(CustomerProblem problem)
        {
            return problem.Dialogues[random.Next(problem.Dialogues.Count)];
        }
    }
}
