using System;
using System.Collections.Generic;
using static System.Console;

namespace Hangman {
    class Game {
        public Game() {
            Letters[] word;
            int length, startA = 0, endA = int.MaxValue;
            #region start
            Clear();
            WriteLine("Welcome to Hangman in C# console by destbg.\n" +
                "Use one of the following commands.\n" +
                "is <word> - to start the game with that word.\n" +
                "random - so the game chooses a word for you.\n");
            while (true) {
                Write("Command: ");
                string readline = ReadLine().Trim().ToLower();
                if (readline == "") continue;
                string[] read = readline.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (!(read.Length <= 2))
                    WriteLine($"'{read[0]}' is not one of the available commands.\n");
                else if (read[0] == "is" && read.Length == 2)
                    if (!(read[1].Length >= 3 && read[1].Length <= 32))
                        WriteLine("Game couldn't start. The word has to be between 3 and 32 letters.\n");
                    else if (!Any(read[1], char.IsLetter))
                        WriteLine("Game couldn't start. The word must have only letters.\n");
                    else {
                        char[] wordCheck = read[1].ToCharArray();
                        length = wordCheck.Length;
                        bool sA = true, eA = true;
                        for (int i = wordCheck[0], x = wordCheck[0]; sA || eA; i++, x--) {
                            if (char.IsLetter((char)i) && eA)
                                endA = i;
                            else eA = false;
                            if (char.IsLetter((char)x) && sA)
                                startA = x;
                            else sA = false;
                        }
                        bool error = false;
                        for (int i = 0; i < wordCheck.Length; i++)
                            if (wordCheck[i] < startA || wordCheck[i] > endA) {
                                error = true;
                                break;
                            }
                        if (error) {
                            WriteLine("An error accured while trying to start the game.\n");
                            continue;
                        }
                        word = new Letters[length];
                        for (int i = 0; i < length; i++)
                            word[i] = new Letters(wordCheck[i],
                                (i == 0 ? true : i == length - 1 ? true : false));
                        break;
                    }
                else if (read[0] == "random" && read.Length == 1) {
                    char[] wordCheck = GetRandomWord();
                    length = wordCheck.Length;
                    startA = 97;
                    endA = 122;
                    word = new Letters[length];
                    for (int i = 0; i < length; i++)
                        word[i] = new Letters(wordCheck[i],
                            (i == 0 ? true : i == length - 1 ? true : false));
                    break;
                }
                else WriteLine($"'{read[0]}' is not one of the available commands.\n");
            }
            #endregion
            #region game
            var alphabet = new HashSet<char>();
            string input = "meme";
            byte frame = 0;
            while (input != "exit") {
                string toSay = "\n", command = "Input has to be one letter!";
                char letter;
                bool finished = false, used = false, dontAdd = false;
                var where = new List<byte>();
                if (input.Length != 1) {
                    letter = '0';
                    dontAdd = true;
                }
                else {
                    letter = Convert.ToChar(input);
                    if (!char.IsLetter(letter)) {
                        command = "Input has to be a letter!";
                        dontAdd = true;
                    }
                    else if (letter < startA || letter > endA) {
                        command = "Input has the be from the same language as the word.";
                        dontAdd = true;
                    }
                    else {
                        command = "Couldn't find letter in word.";
                        if (alphabet.Contains(letter)) used = true;
                        alphabet.Add(letter);
                    }
                }
                toSay += "Word: ";
                for (int i = 0; i < length; i++) {
                    if (word[i].Letter == letter && !word[i].Perms) {
                        word[i] = new Letters(word[i].Letter, true);
                        where.Add((byte)(i + 1));
                    }
                    if (!word[i].Perms) finished = true;
                    toSay += (word[i].Perms ? word[i].Letter : '_');
                }
                toSay += "\n\nUsed letters: " +
                    (alphabet.Count != 0 ? string.Join(", ", alphabet) : "empty") + '\n';
                if (used)
                    command = "You have already used this letter.";
                else if (where.Count != 0)
                    command = $"Found letter {char.ToUpper(letter)} on {(where.Count > 1 ? "positions" : "position")} {string.Join(", ", where)}";
                else if (!dontAdd) frame++;
                if (!finished)
                    command = "End of game. You WON!";
                else if (frame == 6) {
                    toSay = "\nWord: ";
                    for (int i = 0; i < length; i++)
                        toSay += word[i].Letter;
                    toSay += "\n\nUsed letters: " + string.Join(", ", alphabet) + "\n";
                    command = "End of game. You LOST!";
                }
                toSay += GetFrame(frame);
                toSay += command;
                Clear();
                WriteLine(toSay);
                if (frame == 6 || !finished) break;
                Write("Letter: ");
                input = ReadLine().Trim().ToLower();
            }
            WriteLine("\nPress anything to continue...");
            ReadKey();
            #endregion
        }
        private char[] GetRandomWord() {
            string str;
            try {
                using (System.Net.WebClient wc = new System.Net.WebClient())
                    str = wc.DownloadString("http://api.wordnik.com/v4/words.json/randomWords?hasDictionaryDef=true&minCorpusCount=0&minLength=3&maxLength=32&limit=1&api_key=a2a73e7b926c924fad7001ca3111acd55af2ffabf50eb4ae5").ToLower();
            } catch {
                str = " ";
            }
            if (str.Contains(" ") || str.Contains("-") || str.Contains("'")) {
                string[] arr = { "imagination", "allay", "macro", "fairground", "yesterday", "genders" };
                return arr[new Random().Next(arr.Length)].ToCharArray();
            }
            int length = 0;
            for (int i = str.IndexOf("\"word\":") + 8; ; i++)
                if (str[i] != '"') length++;
                else break;
            char[] wordCheck = new char[length];
            for (int i = str.IndexOf("\"word\":") + 8, x = 0; i < str.Length - 3; i++, x++)
                wordCheck[x] = str[i];
            return wordCheck;
        }
        private bool Any(string str, Func<char, bool> func) {
            foreach (char c in str)
                if (!func.Invoke(c)) return false;
            return true;
        }
        private string GetFrame(byte frame) {
            switch (frame) {
                case 0:
                return "       _________\n" +
                        "       |       |\n" +
                        "               |\n" +
                        "               |\n" +
                        "               |\n" +
                        "               |\n" +
                        "               |\n" +
                        "               |\n" +
                        "               |\n" +
                        "               |\n\n";
                case 1:
                return "       _________\n" +
                        "      _|_      |\n" +
                        "     |*_*|     |\n" +
                        "     `````     |\n" +
                        "               |\n" +
                        "               |\n" +
                        "               |\n" +
                        "               |\n" +
                        "               |\n" +
                        "               |\n\n";
                case 2:
                return "       _________\n" +
                        "      _|_      |\n" +
                        "     |*_*|     |\n" +
                        "     ``|``     |\n" +
                        "       |       |\n" +
                        "       |       |\n" +
                        "       |       |\n" +
                        "       |       |\n" +
                        "               |\n" +
                        "               |\n\n";
                case 3:
                return "       _________\n" +
                        "      _|_      |\n" +
                        "     |*_*|     |\n" +
                        "     ``|``     |\n" +
                        "       |\\      |\n" +
                        "       | \\     |\n" +
                        "       |       |\n" +
                        "       |       |\n" +
                        "               |\n" +
                        "               |\n\n";
                case 4:
                return "       _________\n" +
                        "      _|_      |\n" +
                        "     |*_*|     |\n" +
                        "     ``|``     |\n" +
                        "      /|\\      |\n" +
                        "     / | \\     |\n" +
                        "       |       |\n" +
                        "       |       |\n" +
                        "               |\n" +
                        "               |\n\n";
                case 5:
                return "       _________\n" +
                        "      _|_      |\n" +
                        "     |*_*|     |\n" +
                        "     ``|``     |\n" +
                        "      /|\\      |\n" +
                        "     / | \\     |\n" +
                        "       |       |\n" +
                        "       |       |\n" +
                        "        \\      |\n" +
                        "         \\     |\n\n";
                case 6:
                return "       _________\n" +
                        "      _|_      |\n" +
                        "     |*_*|     |\n" +
                        "     ``|``     |\n" +
                        "      /|\\      |\n" +
                        "     / | \\     |\n" +
                        "       |       |\n" +
                        "       |       |\n" +
                        "      / \\      |\n" +
                        "     /   \\     |\n\n";
                default:
                return "An unknown error accured.";
            }
        }
    }
}
