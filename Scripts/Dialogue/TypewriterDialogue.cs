using Microsoft.Xna.Framework;

namespace PharmaCat.Scripts
{
    public enum DialogueMood
    {
        None,
        Intro,
        HappyResult,
        AngryResult
    }

    public class TypewriterDialogue
    {
        private string fullText = "";
        private string visibleText = "";

        private float typeTimer;
        private float faceTimer;
        private float waitTimer;

        private readonly float typeSpeed;
        private readonly float faceSwitchSpeed;
        private readonly float resultVisibleSeconds;

        private bool faceToggle;
        private bool waitsThenCloses;

        public DialogueMood Mood { get; private set; } = DialogueMood.None;
        public string VisibleText => visibleText;
        public bool IsActive => Mood != DialogueMood.None;
        public bool IsTyping => IsActive && visibleText.Length < fullText.Length;
        public bool FaceToggle => faceToggle;
        public bool FinishedWaiting { get; private set; }

        public TypewriterDialogue(float typeSpeed = 0.035f, float faceSwitchSpeed = 0.12f, float resultVisibleSeconds = 1f)
        {
            this.typeSpeed = typeSpeed;
            this.faceSwitchSpeed = faceSwitchSpeed;
            this.resultVisibleSeconds = resultVisibleSeconds;
        }

        public void Start(string text, DialogueMood mood, bool waitsThenCloses)
        {
            fullText = text ?? "";
            visibleText = "";

            Mood = mood;
            this.waitsThenCloses = waitsThenCloses;

            typeTimer = 0f;
            faceTimer = 0f;
            waitTimer = 0f;
            faceToggle = false;
            FinishedWaiting = false;
        }

        public void Clear()
        {
            fullText = "";
            visibleText = "";
            Mood = DialogueMood.None;

            typeTimer = 0f;
            faceTimer = 0f;
            waitTimer = 0f;
            faceToggle = false;
            FinishedWaiting = false;
            waitsThenCloses = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (visibleText.Length < fullText.Length)
            {
                typeTimer += dt;

                while (typeTimer >= typeSpeed && visibleText.Length < fullText.Length)
                {
                    visibleText += fullText[visibleText.Length];
                    typeTimer -= typeSpeed;
                }

                faceTimer += dt;

                if (faceTimer >= faceSwitchSpeed)
                {
                    faceTimer = 0f;
                    faceToggle = !faceToggle;
                }

                return;
            }

            faceToggle = false;

            if (!waitsThenCloses)
                return;

            waitTimer += dt;

            if (waitTimer >= resultVisibleSeconds)
            {
                FinishedWaiting = true;
            }
        }
    }
}
