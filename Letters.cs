namespace Hangman {
    public class Letters {
        public bool Perms { get; }
        public char Letter { get; }

        public Letters(char letter, bool perms) {
            Letter = letter;
            Perms = perms;
        }
    }
}
