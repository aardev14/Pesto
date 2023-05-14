/*
MIT License

Copyright (c) 2023 Freddie Ranieri

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

/*
Project:  Pesto 1.0.0
Author:   Freddie Ranieri
Date:     05/12/2023

Learn more on GitHub @ https://github.com/aardev14/Pesto.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PestoNet
{

    public class Pesto : IDisposable
    {
        private static List<String> NormalizedWordList = new List<String>(); //Filled during Init() and used by all future instances to Evaluate()
        private char ReplacementChar = '\uFFFD'; //Substitution character
        private bool disposedValue;

        /// <summary>
        /// //Call on launch of the application to ready all future instances of Pesto to use the static bad password list, normalized and ordered in descending length.
        /// </summary>
        /// <param name="resourceId">Set the properties of the BadPassword.csv file to EmbeddedResource, Do Not Copy. Pass in the resource ID of the file here.</param>
        public static void Init()
        {
            //Set the resource ID for BannedWords.csv.
            string resourceId = "[RESOURCE-ID-GOES-HERE]";
        
            //Read the BannedWords.csv file into a string.
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Pesto)).Assembly;
            Stream stream = assembly.GetManifestResourceStream(resourceId);
            
            //Make sure that the Resource ID is correct.
            if (stream == null)
            {
                throw new ArgumentException("Invalid Resource ID.", nameof(resourceId));
            }
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
           
            //Store the words as a list.
            var words = text.Split(',');
            List<String> banned = new List<String>();
            for (int i = 0; i < words.Length; ++i)
            {
                banned.Add(words[i].Replace("\n", "").Replace("\r", ""));
            }

            //Only load words with 3 characters or more into the list.
            //Get rid of leetspeak for every word in the list.
            //Make every word in the list lowercase.
            NormalizeAll(banned);

            //Order the list in descending order by length.
            OrderNormalizedList();
        }


        /// <summary>
        /// Call when you want to evaluate the strength of a password. You should call this in a using statement for proper memory management.
        /// </summary>
        /// <param name="password">A char array of the password that you want to evaluate</param>
        /// <param name="perfectScore">The number of match points needed for Pesto to score a 4 if all complexity points are awarded</param>
        /// <param name="minimumLength">The minimum length of the password needed to be awarded the minimum length complexity point</param>
        /// <param name="requireUppercase">If true, require an uppercase letter to earn the uppercase complexity point. Else, automatically award it.</param>
        /// <param name="requireLowercase">If true, require an lowercase letter to earn the lowercase complexity point. Else, automatically award it.</param>
        /// <param name="requireSymbol">If true, require a symbol to earn the symbol complexity point. Else, automatically award it.</param>
        /// <param name="requireNumber">If true, require a number to earn the number complexity point. Else, automatically award it.</param>
        /// <param name="debug">Print out a bad password if it is found during the estimation.</param>
        /// <returns></returns>
        public int Evaluate(char[] password, int perfectScore, int minimumLength, bool requireUppercase, bool requireLowercase, bool requireSymbol, bool requireNumber, bool debug)
        {
            char[] originalPassword = new char[password.Length];
            Array.Copy(password, 0, originalPassword, 0, password.Length);

            //Normalize password.
            for (int i = 0; i < password.Length; ++i)
            {
                if (Char.IsLetter(password[i])) password[i] = Char.ToLower(password[i]);

                //L33t substitutions
                switch (password[i])
                {
                    case '4':
                        password[i] = 'a';
                        break;
                    case '@':
                        password[i] = 'a';
                        break;
                    case '8':
                        password[i] = 'b';
                        break;
                    case '(':
                        password[i] = 'c';
                        break;
                    case '{':
                        password[i] = 'c';
                        break;
                    case '[':
                        password[i] = 'c';
                        break;
                    case '<':
                        password[i] = 'c';
                        break;
                    case '3':
                        password[i] = 'e';
                        break;
                    case '6':
                        password[i] = 'g';
                        break;
                    case '9':
                        password[i] = 'g';
                        break;
                    case '1':
                        password[i] = 'i';
                        break;
                    case '!':
                        password[i] = 'i';
                        break;
                    case '|':
                        password[i] = 'i';
                        break;
                    case '0':
                        password[i] = 'o';
                        break;
                    case '$':
                        password[i] = 's';
                        break;
                    case '5':
                        password[i] = 's';
                        break;
                    case '+':
                        password[i] = 't';
                        break;
                    case '7':
                        password[i] = 't';
                        break;
                    case '%':
                        password[i] = 'x';
                        break;
                    case '2':
                        password[i] = 'z';
                        break;
                    default:
                        break;
                }
            }

            //Track the banned word count.
            int bannedWordCount = 0;
            int goodChars = 0;

            //Traverse the normalized password.
            for (int passwordIndex = 0; passwordIndex < password.Length; ++passwordIndex)
            {
                //Check the normalized password against all normalized words.
                for (int wordListIndex = 0; wordListIndex < NormalizedWordList.Count; ++wordListIndex)
                {

                    if (password[passwordIndex] == NormalizedWordList[wordListIndex][0]) //We find the first letter of the word at the current index we are checking in the password.
                    {
                        int fuzz = 0; //We have found the first letter of the word in the password! Let's check for a match and track the fuzz.

                        if (password.Length - passwordIndex >= NormalizedWordList[wordListIndex].Length) //Are there enough characters left in the password for the word to even fit?
                        {

                            //Traverse the bad normalized password located in the list.
                            List<int> usedIndexes = new List<int>(); //Track the indexes we might mark as used if the result isn't too fuzzy
                            for (int wordIndex = 0; wordIndex < NormalizedWordList[wordListIndex].Length; ++wordIndex)
                            {
                                if (password[passwordIndex + wordIndex] == NormalizedWordList[wordListIndex][wordIndex])
                                {
                                    usedIndexes.Add(passwordIndex + wordIndex);
                                }
                                else
                                {
                                    fuzz++;
                                    if (fuzz > 1)
                                    {
                                        break; //The password is different enough. Let's keep looking!
                                    }
                                    else
                                    {
                                        if (wordIndex > 0) //1 fuzz counts as used space, so if this is fuzz, then count it. First char can't be fuzz, because there is no previous char to be used.
                                        {
                                            if (NormalizedWordList[wordListIndex][wordIndex - 1] == password[passwordIndex + wordIndex - 1]) //if previous char is used, this is fuzz
                                            {
                                                usedIndexes.Add(passwordIndex + wordIndex);
                                            }
                                        }
                                    }
                                }
                            }

                            if (fuzz <= 1) //We found a bad password inside the password!
                            {
                                if (debug) Debug.WriteLine("Bad Word Found: " + NormalizedWordList[wordListIndex]);

                                bannedWordCount++;

                                //Find the max index in the used word count. Anything less than that and not in the used word count is a good char.
                                int max = 0;
                                foreach (int usedIndex in usedIndexes) //Find max
                                {
                                    if (usedIndex > max) max = usedIndex;
                                }

                                for (int i = 0; i < max; ++i) //Increment good chars for score and mark them as used.
                                {
                                    if (!usedIndexes.Contains(i))
                                    {
                                        goodChars++;
                                        usedIndexes.Add(i);
                                    }
                                }

                                //Now let's remove the used chars from the char array
                                foreach (int usedIndex in usedIndexes) //Set the removables to SubstituteChar
                                {
                                    password[usedIndex] = ReplacementChar;
                                }
                                usedIndexes.Clear();

                                char[] temp = new char[password.Length];
                                int tempIndex = 0;
                                for (int i = 0; i < password.Length; ++i)//store the new password in temp
                                {
                                    if (password[i] != ReplacementChar)
                                    {
                                        temp[tempIndex] = password[i];
                                        tempIndex++;
                                    }
                                }
                                Array.Clear(password, 0, password.Length);
                                password = new char[tempIndex];
                                Array.Copy(temp, 0, password, 0, tempIndex);
                                Array.Clear(temp, 0, temp.Length);


                                //Now let's keep looking!

                                passwordIndex = -1; //Let's start the search over again.
                                break;
                            }
                            else //Too much fuzz
                            {
                                //do nothing!
                            }

                        }
                        else //Not even enough characters in the password left to check...let's go to the next word in the list.
                        {
                            //do nothing!
                        }

                    }
                }

            }

            //Calculate match points.
            int value = bannedWordCount + goodChars + password.Length;
            Array.Clear(password, 0, password.Length);

            //Calculate complexity points.
            int rule = 0;
            if (originalPassword.Length >= minimumLength)
            {
                rule++;
            }
            if (requireLowercase)
            {
                foreach (char c in originalPassword)
                {
                    if (c >= 'a' && c <= 'z')
                    {
                        rule++;
                        break;
                    }
                }
            }
            else rule++;
            if (requireUppercase)
            {
                foreach (char c in originalPassword)
                {
                    if (c >= 'A' && c <= 'Z')
                    {
                        rule++;
                        break;
                    }
                }
            }
            else rule++;
            if (requireNumber)
            {
                foreach (char c in originalPassword)
                {
                    if (c >= '0' && c <= '9')
                    {
                        rule++;
                        break;
                    }
                }
            }
            else rule++;
            if (requireSymbol)
            {
                String alphanumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                foreach (char c in originalPassword)
                {
                    if (!alphanumeric.Contains(c))
                    {
                        rule++;
                        break;
                    }
                }
            }
            else rule++;

            //Score the password based on match points and complexity points.
            Array.Clear(originalPassword, 0, originalPassword.Length);
            int score = 0;
            if (value > perfectScore && rule > 4)
            {
                score = 4;
            }
            else if (value > perfectScore - 1 && rule > 3)
            {
                score = 3;
            }
            else if (value > perfectScore - 2 && rule > 2)
            {
                score = 2;
            }
            else if (value > perfectScore - 3 && rule > 1)
            {
                score = 1;
            }
            else
            {
                score = 0;
            }

            return score;
        }

        /// <summary>
        /// Normalizes all banned words to get rid of leetspeak.
        /// </summary>
        /// <param name="bannedWordList">The </param>
        private static void NormalizeAll(List<String> bannedWordList)
        {
            NormalizedWordList = new List<String>();

            for (int i = 0; i < bannedWordList.Count; ++i)
            {
                //Make the word lowercase.
                String word = bannedWordList[i].ToLower();

                //Perform leet substitutions.
                word = word.Replace('4', 'a');
                word = word.Replace('@', 'a');
                word = word.Replace('8', 'b');
                word = word.Replace('(', 'c');
                word = word.Replace('{', 'c');
                word = word.Replace('[', 'c');
                word = word.Replace('<', 'c');
                word = word.Replace('3', 'e');
                word = word.Replace('6', 'g');
                word = word.Replace('9', 'g');
                word = word.Replace('1', 'i');
                word = word.Replace('!', 'i');
                word = word.Replace('|', 'i');
                word = word.Replace('0', 'o');
                word = word.Replace('$', 's');
                word = word.Replace('5', 's');
                word = word.Replace('+', 't');
                word = word.Replace('7', 't');
                word = word.Replace('%', 'x');
                word = word.Replace('2', 'z');

                //Only use words with 3 or more characters.
                if (word.Length > 2)
                {
                    NormalizedWordList.Add(word);
                }
            }
        }

        /// <summary>
        /// Orders the normalized word list by descending length.
        /// </summary>
        private static void OrderNormalizedList()
        {
            NormalizedWordList = new List<String>(NormalizedWordList.OrderByDescending(x => x.Length));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

