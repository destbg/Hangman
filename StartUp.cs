namespace Hangman {
    class StartUp {
        static void Main() {
            System.Console.Title = "Hangman";
            while (true) new Game();
        }
    }
}