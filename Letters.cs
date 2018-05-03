namespace Hangman {
    public class Letters {
        private char letter;
        public bool Perms { get; set; }
        public char Letter {
            get => letter;
        }
        public Letters(char letter, bool perms) {
            this.letter = letter;
            Perms = perms;
        }
    }
}
