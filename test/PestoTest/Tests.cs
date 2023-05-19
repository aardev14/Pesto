using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using PestoNet;

namespace PestoTests
{
    public static class Tests
    {
        /// <summary>
        /// This will compare the scoring of Zxcvbn and Pesto using various parameters. Using higher match points and higher minimum characters will result in a stricter password estimator.
        /// The results will be printed to the console. You can plot these in Excel and create a visual such as a bar graph to better illustrate the data.
        /// </summary>
        /// <param name="matchPoints">The match points parameter passed to the Evaluate() function</param>
        /// <param name="minChars">The minimum characters parameter passed to the Evaluate() function</param>
        public static void CompareZxcvbnScoring(int matchPoints, int minChars)
        {
            //Load all the passwords into the password list.
            List<string> passwords = new List<string>();
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Tests)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("PestoTest.000webhost.csv");
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            var leakedwWords = text.Split(',');
            for (int i = 0; i < leakedwWords.Length; ++i)
            {
                passwords.Add(leakedwWords[i].Replace("\n", "").Replace("\r", ""));
            }

            //Set up the 5x5 table.
            int[][] scores = new int[5][];
            for (int i = 0; i < 5; i++)
            {
                scores[i] = new int[5];
            }

            //Evaluate every password with Zxcvbn and Pesto.
            int savedPercentComplete = 0;
            Console.WriteLine("Percentage Complete: 0%");
            for (int i = 0; i < passwords.Count; ++i)
            {
                //Get Zxcvbn score.
                int zxcvbnScore = Zxcvbn.Core.EvaluatePassword(passwords[i]).Score;

                //Get Pesto score.
                int pestoScore = 0;
                using (var pesto = new Pesto())
                {
                    pestoScore = pesto.Evaluate(passwords[i].ToCharArray(), matchPoints, minChars, true, true, true, true, false);
                }

                //Increment the counter.
                scores[zxcvbnScore][pestoScore]++;

                //Print out a progress bar.
                int percentComplete = (int)((((double)i) / passwords.Count)*100);
                if(savedPercentComplete != percentComplete) {
                    Console.WriteLine("Percentage Complete: " + percentComplete + "%");
                    savedPercentComplete = percentComplete;
                }
            }
            Console.WriteLine("Percentage Complete: 100%");

            //Show results.
            Console.WriteLine("=======================================\n");
            Console.WriteLine("Zxcvbn vs Pesto Scoring Test - On 720,000 Passwords)");
            Console.WriteLine("Match Points: " + matchPoints + " | Minimum Characters: " + minChars + "\n\n");
            for (int z = 0; z < 5; z++)
            {
                for (int t = 0; t < 5; t++)
                {
                    Console.WriteLine("Zxcvbn: " + z + " | Pesto: " + t + " | Count: " + scores[z][t]);
                }
            }
            Console.WriteLine("\n=======================================");
        }

        /// <summary>
        /// These tests show how long is takes in milliseconds to perform a Pesto password evaluation. It should take around 2-4 ms.
        /// The results will be printed to the console.
        /// </summary>
        /// <param name="numPasswords">The number of passwords youd like to evaluate to get an average latency</param>
        public static void TestLatency(int numPasswords)
        {
            int matchPoints = 5;
            int minChars = 18;

            //Load all the passwords into the password list.
            List<string> passwords = new List<string>();
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Tests)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("PestoTest.000webhost.csv");
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            var leakedwWords = text.Split(',');
            for (int i = 0; i < leakedwWords.Length; ++i)
            {
                passwords.Add(leakedwWords[i].Replace("\n", "").Replace("\r", ""));
            }

            //Evaluate passwords with Zxcvbn and Pesto.
            double pestoTime = 0.0;
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < numPasswords; ++i)
            {
                //Get Pesto score.
                int pestoScore = 0;
                using (var pesto = new Pesto())
                {
                    pestoScore = pesto.Evaluate(passwords[i].ToCharArray(), matchPoints, minChars, true, true, true, true, false);
                }
            }
            watch.Stop();

            //Calculate the average time.
            pestoTime = watch.Elapsed.TotalMilliseconds / numPasswords;
            
            //Show results.
            Console.WriteLine("=======================================\n");
            Console.WriteLine("Pesto Latency Test - Average Time Per Evaluation (In Milliseconds)\n\n");
            Console.WriteLine("Number of Passwords Tested: " + numPasswords);
            Console.WriteLine("Pesto Latency: " + pestoTime + " ms");
            Console.WriteLine("\n=======================================");
        }

        /// <summary>
        /// Run Pesto with Drowsy settings - 3 match points and 12 minimum characters. They can be changed in code if needed.
        /// </summary>
        public static void RunPesto()
        {
            Console.WriteLine("This test will run Drowsy Pesto (3 match points and 12 minimum characters) by default.");
            Console.WriteLine("Change the parameters in code if needed.");
            Console.WriteLine("Enter a password: ");
            String password = Console.ReadLine(); //In production - use a keyboard UI that stores the password in a char array

            int matchPoints = 3;
            int minChars = 12;

            int pestoScore = 0;
            using (var pesto = new Pesto())
            {
                pestoScore = pesto.Evaluate(password.ToCharArray(), matchPoints, minChars, true, true, true, true, true);
            }

            //Evaluate the password with Drowsy Pesto. You can adjust the match points and minimum characters as needed. Refer to the Github for more info.
            Console.WriteLine("Pesto Score: " + pestoScore);
        }
    }
}

