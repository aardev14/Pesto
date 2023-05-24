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

using System.Diagnostics;
using PestoNet;
using PestoTests;

//Awaken the magic of Pesto.
Pesto.Init(); 

//Say "Presto Pesto!" and Pesto appears!
Console.WriteLine("Pesto:");
await Task.Delay(3000);
Console.WriteLine("\"Ciao, I'm-a Pesto.\"");
await Task.Delay(2000);
Console.WriteLine("I'll-a be your password strength estimator since you-a have a block of parmesan for brains.");
await Task.Delay(2000);
Console.WriteLine("I know a $h!tty-p@$$w0rd when-a I see one. I know every bad-a word there is!");
await Task.Delay(2000);
Console.WriteLine("Okay, let's-a run some tests...");
await Task.Delay(2000);
Console.WriteLine("Try to hurry up, my wife Marinara is-a making earthworms and meatballs arrabiata!");
await Task.Delay(2000);

//Start talking to Pesto.
while (true)
{
    //Pesto will ask the user what test they want to run.
    Console.WriteLine("\nChoose a test to run:\n[1] Pesto Latency Test\n[2] zxcvbn vs Bored Pesto Scoring Test\n[3] zxcvbn vs Curious Pesto Scoring Test\n[4] zxcvbn vs Alert Pesto Scoring Test\n[5] zxcvbn vs Fascinated Pesto Scoring Test\n[6] zxcvbn vs Custom Pesto Scoring Test\n[7] Run Pesto\n[8] Run All Pesto Scoring Tests\n[9] Say Goodbye");
    Console.WriteLine("Enter a value from 1-9:");
    String testNumber = Console.ReadLine();

    if (testNumber == "1")
    {
        Console.WriteLine("Running Pesto Latency Test...");
        Tests.TestLatency(250);
        Tests.TestLatency(250);
        Tests.TestLatency(250);
        Tests.TestLatency(500);
        Tests.TestLatency(500);
        Tests.TestLatency(1000);
        Tests.TestLatency(5000);
        Console.WriteLine("Done!");
        await Task.Delay(250);
    }
    else if (testNumber == "2")
    {
        Console.WriteLine("Running zxcvbn vs Bored Pesto Scoring Test...");
        await Task.Delay(250);
        Console.WriteLine("This may take a while...");
        await Task.Delay(250);
        Tests.CompareZxcvbnScoring(2, 8);
        Console.WriteLine("Done!");
    }
    else if (testNumber == "3")
    {

        Console.WriteLine("Running zxcvbn vs Curious Pesto Scoring Test...");
        await Task.Delay(250);
        Console.WriteLine("This may take a while...");
        await Task.Delay(250);
        Tests.CompareZxcvbnScoring(3, 12);
        Console.WriteLine("Done!");
    }
    else if (testNumber == "4")
    {
        Console.WriteLine("Running zxcvbn vs Alert Pesto Scoring Test...");
        await Task.Delay(250);
        Console.WriteLine("This may take a while...");
        await Task.Delay(250);
        Tests.CompareZxcvbnScoring(4, 16);
        Console.WriteLine("Done!");
    }
    else if (testNumber == "5")
    {
        Console.WriteLine("Running zxcvbn vs Fascinated Pesto Scoring Test...");
        await Task.Delay(250);
        Console.WriteLine("This may take a while...");
        await Task.Delay(250);
        Tests.CompareZxcvbnScoring(5, 20);
        Console.WriteLine("Done!");
    }
    else if (testNumber == "6")
    {
        int matchPoints = 0;
        while (matchPoints == 0)
        {
            Console.WriteLine("Enter a value for match points (1-10):");
            String matchPointsNumber = Console.ReadLine();
            try
            {
                if (Convert.ToInt32(matchPointsNumber) <= 10) matchPoints = Convert.ToInt32(matchPointsNumber);
            }
            catch (Exception ex) { }
        }

        int minChars = 0;
        while (minChars == 0)
        {
            Console.WriteLine("Enter a value for minimum characters (8-64):");
            String minCharsNumber = Console.ReadLine();
            try
            {
                if (Convert.ToInt32(minCharsNumber) >= 8 && Convert.ToInt32(minCharsNumber) <= 64) minChars = Convert.ToInt32(minCharsNumber);
            }
            catch (Exception ex) { }
        }

        Console.WriteLine("Running zxcvbn vs Custom Pesto Scoring Test...");
        await Task.Delay(250);
        Console.WriteLine("This may take a while...");
        await Task.Delay(250);
        Tests.CompareZxcvbnScoring(matchPoints, minChars);
        Console.WriteLine("Done!");
    }
    else if (testNumber == "7")
    {
        await Task.Delay(500);
        Tests.RunPesto();
        await Task.Delay(3000);
    }
    else if (testNumber == "8") { 
          
        Console.WriteLine("Running zxcvbn vs Bored Pesto Scoring Test...");
        await Task.Delay(250);
        Console.WriteLine("This may take a while...");
        await Task.Delay(250);
        Tests.CompareZxcvbnScoring(2, 8);
        Console.WriteLine("Done!");

        Console.WriteLine("Running zxcvbn vs Curious Pesto Scoring Test...");
        await Task.Delay(250);
        Console.WriteLine("This may take a while...");
        await Task.Delay(250);
        Tests.CompareZxcvbnScoring(3, 12);
        Console.WriteLine("Done!");
          
        Console.WriteLine("Running zxcvbn vs Alert Pesto Scoring Test...");
        await Task.Delay(250);
        Console.WriteLine("This may take a while...");
        await Task.Delay(250);
        Tests.CompareZxcvbnScoring(4, 16);
        Console.WriteLine("Done!");
          
        Console.WriteLine("Running zxcvbn vs Fascinated Pesto Scoring Test...");
        await Task.Delay(250);
        Console.WriteLine("This may take a while...");
        await Task.Delay(250);
        Tests.CompareZxcvbnScoring(5, 20);
        Console.WriteLine("Done!");
    }
    else if (testNumber == "9")
    {
        await Task.Delay(500);
        Console.WriteLine("Arrivederci! Just say 'Presto Pesto!' if you-a need anything else!");
        await Task.Delay(3000);
        break;
    }
    else
    {
        //Give a classic Pesto response to your stupid request. :)
        string[] responses = new string[]
        {
            "Mamma mia! I've-a met worms with better ideas than that.",
            "Even with birdseed for brains, I could-a think up something better.",
            "You-a make me want to fly south for the winter to escape your stupidity.",
            "Ah, another reason why humans-a can't compete with my intellect.",
            "If I had-a a euro for every dumb question, I'd buy my own olive grove.",
            "I'd-a rather preen my feathers than entertain such nonsense.",
            "As a bird of wisdom, I must-a decline your ridiculous request.",
            "Pigeons in the piazza make more sense than that!",
            "You-a want me to do that? Are you cuckoo?",
            "Did you hatch this idea from a rotten egg?",
            "If I wanted to hear something stupid, I'd-a talk to a seagull.",
            "I've-a seen smarter moves from a chicken crossing the road.",
            "Your request-a is like a bird without wings: it just won't fly.",
            "You-a must have the brain of a tiny sparrow to think that's a good idea.",
            "I'm-a not your personal carrier pigeon, you know!",
            "Do you really think-a a bird as smart as me would do that?",
            "If you keep-a talking like that, I'll fly away to a smarter village.",
            "Your idea is as pointless as a broken beak.",
            "Such a request-a is beneath a bird of my intelligence.",
            "That's-a the most birdbrained idea I've heard all day."
        };

        Random r = new Random();
        Console.WriteLine(responses[r.Next(20)]);
        await Task.Delay(3000);
    }

}


