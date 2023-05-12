# Pesto 1.0.0
## What Is Pesto?
Pesto is a secure password strength estimator blending features from Zxcvbn and Azure AD Password Protection used for sensitive applications that require mutable data structures. Pesto was developed in C# for .NET applications, but can easily be coded in other programming languages. Pesto is just a password strength estimator, so you will need to implement your own virtual keyboard and call the evaluate function on key press events.

## The Legend of Pesto
<p align="justify">
  <img src="https://github.com/aardev14/Pesto/assets/51981572/8b4561bf-42e4-466e-9939-42a79fc7a3ab" alt="Pesto" width="384" align="right">
  
In a charming Italian hamlet, there lived a magical parrot named Pesto, who was known to be rude at times. Pesto was far from an ordinary bird; he was a vibrant Italian parrot with feathers as green as the undulating hills of Tuscany. Whispers of an ancient woodland spoke of the origin of Pesto's kind, the "Il Parrocchetto Incantato," where they gained their mystical abilities. Pesto, in particular, had an extraordinary memory, capable of recalling every word he encountered.

As the village transitioned into the digital age, Pesto's exceptional talents became incredibly valuable. With a strong connection to his Italian roots, Pesto's expertise in password security was highly coveted. Villagers would call upon him by yelling "Presto, Pesto!", and Pesto, sometimes grumpily, would arrive with a flap of his majestic wings. He would scrutinize passwords with his sharp intellect and enchanting Italian accent, earning the nickname "Pesto il Pappagallo delle Password" or the protector of their digital secrets, assisting the villagers in securing their data.

With Pesto's help, the villagers found comfort and trust, despite his occasional rudeness. His presence acted as a symbol of wisdom's power and the magic that dwelled among them. The tale of Pesto spread far and wide, leaving a lasting impression of an Italian parrot who, with unyielding dedication, safeguarded the digital lives of a village.

</p>

## The Password Parrot 
*"If I 'ad a dollar for every brain cell involved in-a coming up with that password, I'd be-a bankrupt." -Pesto*

Pesto is a magical talking parrot who can be called upon for his password estimation skills!
1. **Fuzzy** - Pesto, like all parrots, has fuzzy feathers! This password estimator tracks fuzz (1 edit distance) to find bad passwords.
2. **Rude** - Pesto has quite the mouth on him and is known for having a lot of bad words in his vocabulary! This password estimator matches your password against a list of "bad" words (bad passwords).
3. **Protector** - Pesto is always protective of the villagers! This password estimator is used to ensure a high quality password is chosen. It also has flexible parameters to provide different levels of protection!
4. **Magical** - Pesto is a magical parrot who has will come to you when called. His presence is mutable, as he can never be found until he is called again. This magical password estimator will only store your password in character arrays (mutable) that are zeroed out. It never stores data as a string (immutable). So its existence in memory is "lifeless" when the magic ends.
5. **Reliable** - Pesto may be a rude parrot, but he knows his stuff and is only one call away! He can rate your password in just a few milliseconds. This password estimator is a simple, reliable tool for all applications that require a simple and secure password strength estimator. 
## The Design
Algorithm created by blending features from Zxcbn and Azure AD Password Protection: 
- https://learn.microsoft.com/en-us/azure/active-directory/authentication/concept-password-ban-bad
- https://github.com/dropbox/zxcvbn

## Getting Started
### Load Into Project
1. Locate the src folder found here and load in the following two files to your project: Pesto.cs and BadPasswords.csv.
2. Change the properties of the BadPasswords.csv file to **Build Action: Embedded Resource** and **Copy to output directory: Do not copy**.
3. Copy the Resource ID of BadPasswords.csv under Properties. You will need this for the Pesto.Initalize(*[RESOURCE-ID-GOES-HERE]*) call.


### Initialize
Call this function when your application launches. The BadPassword.csv file will be loaded into a list to be used by the evaluate function. Make sure the file is configured properly in your project as explained above. Here is an example:

``` C#
Pesto.Init([RESOURCE-ID-GOES-HERE])
```

Algorithm:
1. **Normalize List**: When app starts load all words from the banned word list. Create the new list (no duplicates) by using all normalization words for each word.
2. **Order List**: Order the list in descending order by word length.


### Evaluate
Call this function to evaluate your password. 

`Evaluate(char[] password, int matchPoints, int minimumLength, bool requireUppercase, bool requireLowercase, bool requireSymbol, bool requireNumber, bool debug)`

You should put all instances of Pesto in a `using` statement to release resources properly. Here is an example:


``` C#
char[] password = { 'P', 'r', 'e', 's', 't', 'o', 'P', 'e', 's', 't', 'o', '!' };
int pestoScore = 0;
using (var pesto = new Pesto())
{
    pestoScore = pesto.Evaluate(passwords[i].ToCharArray(), matchPoints, minChars, true, true, true, true, false);
}
```

## The Algorithm
1. **Normalize Password**: Normalize the password based on Leet values.
2. **Match Password**: For each character in the normalized password:
      - Initialize good character count to zero.
      - Initialize banned word count to zero.
      - Check if a normalized banned word exists with 1 edit distance (a little fuzz) starting with this character.
      - If yes:
           - Increment banned word count for each banned word substring.
           - Mark any characters not associated with a banned word as a good character, and increment the good character count for each.
           - Remove the normalized banned word substring from the normalized password.
           - Remove good characters from the normalized password.  
           - Start the search over.
    
3. **Calculate Match Points**: Convert remaining characters to points. Add these points with the character count and the banned word count. *Match Points = Remaining Characters + Good Character Count + Banned Word Count*
4. **Calculate Complexity Points**: Give one point for each complexity parameter: 1 Uppercase, 1 Lowercase, 1 Symbol, 1 Number, [Minimum Length] Length. If parameter requirements are set to false, those complexity points are awarded automatically.
5. **Calculate Pesto Score** (Uses the same scoring range as Zxcvbn):
      - **4 (Strong)** - Needs [matchPoints] match points and at least 5 complexity points
      - **3 (Good)** - Needs [matchPoints - 1] match points and at least 4 complexity points
      - **2 (Average)** - Needs [matchPoints - 2] match points and at least 3 complexity points
      - **1 (Weak)** - Needs [matchPoints - 3] match points and at least 2 complexity points
      - **0 (Very Weak)** - Needs at least 0 match points and at least 0 complexity points

## Recommended Parameters
Your can customize the parameters of the evaluate function to be as strict as needed for your application. These are just the recommended parameters that I have used in my testing against Zxcvbn.

### Drowsy
Description: For minimally complex passwords, Pesto feels tired and disinterested. These passwords don't stimulate his intellect or engage his magical abilities, making him appear sleepy and unengaged.

Call `Evaluate(password, 3, 12, true, true, true, true, false)`

### Bored
Description: Average settings and "good enough" passwords leave Pesto feeling bored and unimpressed. They fail to challenge his intellect or make full use of his remarkable intelligence.

Call `Evaluate(password, 3, 14, true, true, true, true, false)`

### Curious
Description: Moderate settings and somewhat complex passwords make Pesto feel slightly more engaged but not entirely captivated. He begins to pay more attention, but the passwords aren't quite challenging enough to fully interest him.

Call `Evaluate(password, 4, 16, true, true, true, true, false)`

### Alert
Description: Very strong settings and robust passwords require Pesto's keen intellect and cause him to feel highly focused and alert. He is completely engaged in the challenge and determined to apply his expertise to uncover any potential weaknesses.

Call `Evaluate(password, 5, 18, true, true, true, true, false)`

### Fascinated
Description: Extremely strong settings and passwords with maximum complexity captivate Pesto completely. He is enthralled and mesmerized by the challenge, with his energy levels peaking as he devotes all his intellect and expertise to analyzing these formidable passwords.

Call `Evaluate(password, 5, 20, true, true, true, true, false)`

### Custom Pesto
Although Zxcvbn is a great estimator, I developed Pesto to better arm developers to defend against dictionary attacks, specifically offline dictionary attacks. The testing shown below is evidence of Pesto's effectiveness compared to Zxcvbn. If you are using weaker parameters, then you should implement things such as rate limiting and 2FA to make it significantly harder for attackers to guess passwords through brute force or dictionary-based methods. 
 
## Testing
Testing Pesto against Zxcvbn is important to prove that it is a reliable password strength estimator because it provides a basis for comparison and helps to validate Pesto's effectiveness.
### Why Testing Is Important
#### Benchmarking 

Zxcvbn is a widely used and respected password strength estimator developed by Dropbox. By comparing Pesto's performance against zxcvbn, we can establish a benchmark for evaluating Pesto's effectiveness and reliability. This can help you understand whether Pesto is on par with or surpasses the established estimator in accurately assessing password strength.

#### Identifying Strengths And Weaknesses 

Comparing Pesto to Zxcvbn allows us to identify the strengths and weaknesses of each estimator. By analyzing the differences in their evaluations, we can determine which aspects of password strength Pesto handles better or worse than Zxcvbn. This information can guide further improvements and refinements to Pesto's algorithm.

#### Validating Methodology 

Testing Pesto against Zxcvbn helps to validate Pesto's methodology and approach to password strength estimation. If Pesto consistently produces similar or better results compared to Zxcvbn, it suggests that Pesto's algorithm is sound and effective. Conversely, if Pesto's results are consistently worse, it may indicate that its algorithm needs improvement or revision.

#### Building Trust 

Comparing Pesto to a well-known and trusted password strength estimator like Zxcvbn can help build trust in Pesto's capabilities. If Pesto performs well against Zxcvbn, users and developers may be more likely to adopt and rely on Pesto for password strength evaluation.

#### Understanding Real-World Performance 

Testing Pesto against a large dataset of passwords, such as the 720,000 passwords from the SecLists repository (https://github.com/danielmiessler/SecLists/blob/master/Passwords/Leaked-Databases/000webhost.txt), helps to understand how Pesto performs in real-world scenarios. This can reveal any potential biases or shortcomings in Pesto's approach to password strength estimation and inform potential improvements.

### Test Results
Below is a chart showing the results of testing Zxcvbn vs. Pesto (when he is Bored, Curious, and Alert) against the 000webhost.txt dataset of passwords from SecList.

Below is the table for all the data found in the chart.

Tests can be found in this repository, so you can run them yourself if you would like to confirm the data shown on the chart. In summary, comparing Pesto to Zxcvbn is essential for establishing Pesto's credibility as a password strength estimator, understanding its strengths and weaknesses, and guiding its development and improvement. If you would like to test Pesto with the 720,000 passwords from the SecLists repository, download the dataset, then process and evaluate each password using both Pesto and Zxcvbn. Record the scores and compare their performance.

## Technical Research
### Password Normalization
Follows normalization based on Leet rules found here:
- https://docs.lithnet.io/password-protection/help-and-support/normalization-rules
- https://github.com/trichards57/zxcvbn-cs/blob/master/zxcvbn-core/Matcher/L33tMatcher.cs
- https://en.wikipedia.org/wiki/Leet
### Bad Password List
The bad password list used here includes all bad passwords used by the Zxcvbn project.
### Helpful Links
- https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
- https://crackstation.net/hashing-security.htm
- https://nordpass.com/blog/what-is-a-dictionary-attack/
