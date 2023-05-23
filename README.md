# Pesto 1.0.0
[![nuget](https://img.shields.io/nuget/v/Pesto.PasswordStrengthEstimator.svg?label=nuget&colorB=green)](https://www.nuget.org/packages/Pesto.PasswordStrengthEstimator)

## What Is Pesto?
Pesto is a secure password strength estimator blending features from [zxcvbn](https://dropbox.tech/security/zxcvbn-realistic-password-strength-estimation) and [Azure AD Password Protection](https://learn.microsoft.com/en-us/azure/active-directory/authentication/concept-password-ban-bad) used for sensitive applications that require mutable data structures. Pesto was developed in C# for .NET applications, but can easily be ported to other programming languages. Pesto is just a password strength estimator (for now), so you will need to implement your own virtual keyboard and call the evaluate function on key press events.

## The Legend of Pesto
<p align="justify">
  <img src="https://github.com/aardev14/Pesto/assets/51981572/538e0cee-508a-49bd-9c2d-1f7e278506b6" alt="Pesto" width="384" align="right">

In a charming Italian hamlet, there lived a magical parrot named Pesto, who was known to be rude at times. Pesto was far from an ordinary bird; he was a vibrant Italian parrot with feathers as green as the hills of Tuscany. Whispers of an ancient woodland spoke of the origin of Pesto's kind, the "Il Parrocchetto Incantato," where they gained their mystical abilities. Pesto, in particular, had an extraordinary memory, capable of recalling every word he ever heard.

As the village transitioned into the digital age, Pesto's exceptional talents became incredibly valuable. With a strong connection to his Italian roots, Pesto's expertise in password security was highly coveted. Villagers would call upon him by yelling "Presto, Pesto!", and Pesto, sometimes grumpily, would arrive with a flap of his majestic wings. He would scrutinize passwords with his sharp intellect and enchanting Italian accent, earning the nickname "Pesto il Pappagallo delle Password" or the protector of their digital secrets, assisting the villagers in securing their data.

With Pesto's help, the villagers found comfort and trust, despite his occasional rudeness. His presence acted as a symbol of wisdom's power and the magic that dwelled among them. The tale of Pesto spread far and wide, leaving a lasting impression of an Italian parrot who, with unyielding dedication, safeguarded the digital lives of a village.

</p>

## The Password Parrot 
*"If I 'ad a euro for every brain cell involved in-a coming up with that password, I'd be-a bankrupt." -Pesto*

Pesto is a magical talking parrot who can be called upon for his password strength estimation skills!
1. **Fuzzy** - Pesto, like all parrots, has fuzzy feathers! This password strength estimator tracks fuzz (1 edit distance) to find bad passwords.
2. **Rude** - Pesto has quite the mouth on him and is known for having a lot of bad words in his vocabulary! This password strength estimator matches your password against a list of "bad" words (bad passwords).
3. **Protective** - Pesto is always protective of the villagers! This password strength estimator is used to ensure a high quality password is chosen. It also has flexible parameters to provide different levels of protection!
4. **Magical** - Pesto is a magical parrot who has will come to you when called. His presence is mutable, as he can never be found until he is called again. This magical password strength estimator will only store your password in character arrays (mutable) that are zeroed out. It never stores data as a string (immutable).
5. **Reliable** - Pesto may be a rude parrot, but he knows his stuff and is only one call away! He can rate your password in just a few milliseconds. This password strength estimator is a simple, reliable tool for all applications that require a simple and secure password strength estimator. 
## The Design
I created the algorithm by blending features from zxcbn and Azure AD Password Protection: 
- https://learn.microsoft.com/en-us/azure/active-directory/authentication/concept-password-ban-bad
- https://github.com/dropbox/zxcvbn

## Getting Started
### Load Into Project
#### Option #1: Add The NuGet Package
1. Simply add the Nuget package [Pesto.PasswordStrengthEstimator](https://www.nuget.org/packages/Pesto.PasswordStrengthEstimator) to your project and you are ready to go! You can run the test project called PestoTest found in the [test](https://github.com/aardev14/Pesto/tree/main/test) folder if you want to try it out quickly.

#### Option #2: Add Pesto.dll As A Project Reference
1. Add Pesto.dll found in the [lib](https://github.com/aardev14/Pesto/tree/main/lib) folder as a project reference in your project and you are up and running! You can run the test project called PestoTest found in the [test](https://github.com/aardev14/Pesto/tree/main/test) folder if you want to try it out quickly.

#### Option #3: Add Source Code Directly To Your Project
1. Locate the [src](https://github.com/aardev14/Pesto/tree/main/src) folder found here and load in the following two files to your project: [Pesto.cs](https://github.com/aardev14/Pesto/blob/main/src/Pesto.cs) and [BadPasswords.csv](https://github.com/aardev14/Pesto/blob/main/src/BannedWords.csv).
2. Change the properties of the BadPasswords.csv file to **Build Action: Embedded Resource** and **Copy to output directory: Do not copy**.
3. Copy the Resource ID of BadPasswords.csv under Properties. You will need to replace the placeholder text in the Pesto.Init() function definition where it says *"[RESOURCE-ID-GOES-HERE]"*.


### Initialize
Call this function when your application launches. The BadPassword.csv file will be loaded into a list to be used by the evaluate function. Make sure the file is configured properly in your project as explained above. Here is an example:

``` C#
Pesto.Init()
```

In order to call this function, make sure your class is using Pesto. If it is not there already, then add the following `using` directive at the top of your .cs file:

``` C#
using PestoNet; 
```

### Evaluate
Call this function to evaluate your password. 

`Evaluate(char[] password, int matchPoints, int minimumLength, bool requireUppercase, bool requireLowercase, bool requireSymbol, bool requireNumber, bool debug)`

You should put all instances of Pesto in a `using` statement to release resources properly. Here is an example:


``` C#
char[] password = { 'P', 'r', 'e', 's', 't', 'o', 'P', 'e', 's', 't', 'o', '!' }; //get this from your own keyboard UI

int matchPoints = 3;
int minChars = 12;
int pestoScore = 0;

using (var pesto = new Pesto())
{
    pestoScore = pesto.Evaluate(password, matchPoints, minChars, true, true, true, true, false);
}
```

## The Algorithm

### Initialize
1. **Normalize List**: When the application starts, it load all words from the banned word list with 3 or more characters. It creates the new list (no duplicates) by using all normalized words for each word. This means removing all Leetspeak and making every word lowercase. This list is static so that all future instances of Pesto can use the normalized list when evaluating a password.
2. **Order List**: Order the list in descending order by word length. This is important for how the algorithm checks for bad passwords. It prioritizes finding the longer words that exist in the password.

### Evaluate
1. **Normalize Password**: Set the password p to all lowercase. This ensures that the password is in lowercase for consistent processing. Normalize the password based on Leet values. Leet substitutions are character replacements commonly used to represent letters with symbols or numbers. For example, "leet" can be represented as "L337". This step modifies the password p accordingly.
2. **Match Password**: 

<p align="justify">
  <img src="https://github.com/aardev14/Pesto/assets/51981572/4ce8b2dd-bf14-4bb4-84e6-7a2f3a7fb729" alt="Pesto" width="384" align="right">

- Initialize the banned word count to 0: This variable keeps track of the number of banned words found in the password.
  
- Initialize the good character count to 0: This variable counts the number of valid (non-banned) characters in the password.
  
- The code then enters a loop that iterates over each character c in the password p.
  
- Inside the outer loop, there is another loop that iterates over each word w in the banned word list b.
  
- The code checks if the first character of the banned word b[w] matches the character at position c in the password p. If they match, further checks are performed to determine if the entire banned word is present starting from position c.
  
- If the length of the password from position c onwards is greater than or equal to the length of the banned word b[w], the code proceeds to check each character of the banned word against the corresponding characters in the password.
  
- A "fuzz" counter is used to keep track of the number of character mismatches between the banned word and the password. If the number of mismatches exceeds 1, the loop is broken, and the code moves on to the next banned word.
  
- If the "fuzz" counter is less than or equal to 1, it means that the banned word is a close match to the password. The code increments the banned word count, updates the good character count, and marks the positions in the password that are part of the banned word by adding them to the used index list. This includes the index that was a good character or fuzz.
  
- The code then replaces these characters with a designated replacement character.
  
- After processing the banned word, the code clears the used index list. Then it creates a new temporary character array temp, and copies the non-replacement characters to it.
  
- The code updates the password p to the new character array with the remaining non-replacement characters.
  
- Finally, the code clears the temporary array and sets c to -1. Setting c to -1 at the end of the outer loop effectively resets the loop counter c to 0 in the next iteration. This ensures that after making modifications to the password p, the loop starts from the beginning to re-evaluate each remaining character against the banned words.
</p>  
  
3. **Calculate Match Points**: Convert remaining characters to points. Add these points with the good character count and the banned word count. *Match Points = Remaining Characters + Good Character Count + Banned Word Count*
4. **Calculate Complexity Points**: Give one point for each complexity parameter: 1 Uppercase, 1 Lowercase, 1 Symbol, 1 Number, [minChars] Length. If parameter requirements are set to false, those complexity points are awarded automatically.
5. **Calculate Pesto Score**:
      - **4 (Strong)** - Needs [matchPoints] match points and all 5 complexity points
      - **3 (Good)** - Needs [matchPoints - 1] match points and at least 4 complexity points
      - **2 (Average)** - Needs [matchPoints - 2] match points and at least 3 complexity points
      - **1 (Weak)** - Needs [matchPoints - 3] match points and at least 2 complexity points
      - **0 (Very Weak)** - Needs at least 0 match points and at least 0 complexity points

## Recommended Parameters
Your can customize the parameters of the evaluate function to be as strict as needed for your application. These are the recommended parameters that I have used in my testing against zxcvbn. Requirements vary based on how the password is hashed, the parameters of the key derivation function used, the likelihood of offline attacks, rate limiting, 2FA, etc. Additionally, there are differing views when it comes to complexity requirements. Read [NIST Special Publication 800-63B](https://pages.nist.gov/800-63-3/sp800-63b.html#memsecretver) for more information. It is important to note that password length is valued over password complextiy.

**You should always accept a Pesto score of 4. You can accept a Pesto score of 3, but it is not recommended. Instead of accepting a Pesto score of 3, you should just lower your parameter values.**

### 1. Bored Pesto
Call `Evaluate(password, 2, 8, true, true, true, true, false)`

#### Description 
Average settings and "good enough" passwords leave Pesto feeling bored and unimpressed. They fail to challenge his intellect or make full use of his remarkable intelligence.

#### Example
*Web Service with Rate Limiting and 2FA (Minimum 8 Characters)*

A social media platform like Twitter. Users on Twitter have profile pages where they post messages, images, and links. Even though users might not post sensitive personal information, it's essential to maintain account security to prevent impersonation, harassment, or trolling. Twitter implements rate limiting and two-factor authentication to help protect against unauthorized access, but the user's password is the first line of defense.

### 2. Curious Pesto
Call `Evaluate(password, 3, 12, true, true, true, true, false)`

#### Description
Moderate settings and somewhat complex passwords make Pesto feel slightly more engaged but not entirely captivated. He begins to pay more attention, but the passwords aren't quite challenging enough to fully interest him.

#### Example
*Medium Sensitivity Web Service (Minimum 12 Characters)*

An e-commerce platform like Amazon. Users on these platforms have accounts where they can place orders, track shipments, and make returns. These accounts could contain credit card information, address information, and purchasing habits - sensitive data that should be safeguarded. Although these platforms typically use various security measures, the password is a key element of account security. A 12-character minimum helps prevent unauthorized access while maintaining usability for a broad user base.

### 3. Alert Pesto
Call `Evaluate(password, 4, 16, true, true, true, true, false)`

#### Description
Very strong settings and robust passwords require Pesto's keen intellect and cause him to feel highly focused and alert. He is completely engaged in the challenge and determined to apply his expertise to uncover any potential weaknesses.*

#### Example
*Offline Application with Brute Force Potential (Minimum 16 Characters)*

An offline password manager like KeePass. This application is often used to store all kinds of passwords, from social media to bank accounts. It uses a master password to generate encryption keys that secure the stored data. If the database were stolen or leaked, the strength of the master password would be critical in preventing a successful offline brute force or dictionary attack. Hence, a 16-character minimum password is required, which provides significantly more combinations and resistance against attacks.

### 4. Fascinated Pesto
Call `Evaluate(password, 5, 20, true, true, true, true, false)`

#### Description
Extremely strong settings and passwords with maximum complexity captivate Pesto completely. He is enthralled and mesmerized by the challenge, with his energy levels peaking as he devotes all his intellect and expertise to analyzing these formidable passwords.

#### Example
*High Sensitivity Web or Offline Service (Minimum 20 Characters)*

A cryptocurrency wallet like Ledger and Trezor - or a seed phrase storage solution like [Splitcoin](https://www.splitcoin.com). I am the founder and lead engineer for Splitcoin. These applications store the private keys or encryption keys needed to access and manage digital currencies like Bitcoin or Ethereum. If an attacker gains access to these keys, the financial loss could be significant - potentially the total value of the cryptocurrency stored in the wallet. This makes the wallet a high-value target, and a long, complex password is essential. With a 20-character minimum, the password forms a strong line of defense against both online and offline attacks.

### Custom Pesto
Call `Evaluate(password, requiredMatchPoints, requiredNumberOfCharacters, true, true, true, true, false)`

Although zxcvbn is a great estimator, I developed Pesto to better arm developers to defend against dictionary attacks, specifically offline dictionary attacks. The testing shown below is evidence of Pesto's effectiveness compared to zxcvbn. It is much stricter! If you are using weaker parameters, then you should implement things such as rate limiting and 2FA to make it significantly harder for attackers to guess passwords through brute force or dictionary-based methods. Regardless of the nature of your application or the parameters used, a memory-hard password hashing function such as Argon2id or Scrypt should be used.
 
## Testing
Testing Pesto against zxcvbn is important to prove that it is a reliable password strength estimator because it provides a basis for comparison and helps to validate Pesto's effectiveness.
### Why Testing Is Important
#### Benchmarking 

As a password strength estimator, zxcvbn is a widely used and respected. It is developed by Dropbox. By comparing Pesto's performance against zxcvbn, we can establish a benchmark for evaluating Pesto's effectiveness and reliability. This can help you understand whether Pesto is on par with or surpasses the established estimator in accurately assessing password strength.

#### Identifying Strengths And Weaknesses 

Comparing Pesto to zxcvbn allows us to identify the strengths and weaknesses of each estimator. By analyzing the differences in their evaluations, we can determine which aspects of password strength Pesto handles better or worse than zxcvbn. This information can guide further improvements and refinements to Pesto's algorithm.

#### Validating Methodology 

Testing Pesto against zxcvbn helps to validate Pesto's methodology and approach to password strength estimation. If Pesto consistently produces similar or better results compared to zxcvbn, it suggests that Pesto's algorithm is sound and effective. Conversely, if Pesto's results are consistently worse, it may indicate that its algorithm needs improvement or revision.

#### Building Trust 

Comparing Pesto to a well-known and trusted password strength estimator like zxcvbn can help build trust in Pesto's capabilities. If Pesto performs well against zxcvbn, users and developers may be more likely to adopt and rely on Pesto for password strength evaluation.

#### Understanding Real-World Performance 

Testing Pesto against a large dataset of passwords, such as the 720,000 passwords in [000webhost.txt](https://github.com/danielmiessler/SecLists/blob/master/Passwords/Leaked-Databases/000webhost.txt) from the [SecLists repository](https://github.com/danielmiessler/SecLists), helps to understand how Pesto performs in real-world scenarios. This can reveal any potential biases or shortcomings in Pesto's approach to password strength estimation and inform potential improvements.

### Test Results
Below is a chart and the associated table showing the results of testing zxcvbn vs. Pesto (when he is Bored, Curious, Alert, and Fascinated) against the 000webhost.txt dataset of passwords from SecList.

![pestochart](https://github.com/aardev14/Pesto/assets/51981572/7b5a67b0-4f57-4451-b780-282ead4d051a)

![pestotable](https://github.com/aardev14/Pesto/assets/51981572/02c5ba3e-1c37-427e-85e4-b6dcebe037dd)

[Tests](https://github.com/aardev14/Pesto/tree/main/test) can be found in this repository, so you can run them yourself if you would like to confirm the data shown on the chart. Additionally, you can modify parameters and run zxcvbn comparison testing to discover what parameters work best for your application. In summary, comparing Pesto to zxcvbn is essential for establishing Pesto's credibility as a password strength estimator, understanding its strengths and weaknesses, and guiding its development and improvement. 

## Technical Research
### Password Normalization
Follows normalization based on Leet rules found here:
- https://docs.lithnet.io/password-protection/help-and-support/normalization-rules
- https://github.com/trichards57/zxcvbn-cs/blob/master/zxcvbn-core/Matcher/L33tMatcher.cs
- https://en.wikipedia.org/wiki/Leet

### Bad Password List

The bad password list used in Pesto includes [data](https://github.com/dropbox/zxcvbn/tree/master/data) used by the zxcvbn project in addition to even more data! I have added the [Top 100,000 common passwords from SecList](https://github.com/danielmiessler/SecLists/blob/master/Passwords/Common-Credentials/10-million-password-list-top-100000.txt), the [BIP39 word list](https://github.com/bitcoin/bips/blob/master/bip-0039/english.txt), popular cryptocurrency terms, years, months, and days. It also contains strings that get detected by the algorithm to check for keyboard patterns, character repetition, letter sequencing, and number sequencing. There are over 175,000 entries in this list. 

### Helpful Links
- https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
- https://crackstation.net/hashing-security.htm
- https://nordpass.com/blog/what-is-a-dictionary-attack/
- https://github.com/trichards57/zxcvbn-cs/tree/master/zxcvbn-core/Dictionaries
- https://support.microsoft.com/en-us/windows/create-and-use-strong-passwords-c5cebb49-8c53-4f5e-2bc4-fe357ca048eb
- https://bitwarden.com/blog/how-long-should-my-password-be/
- https://blog.lastpass.com/2022/01/how-often-should-you-change-your-master-password/

Pesto is free, open source software. I will continue to reveal the magical powers of Pesto (more features) in future releases. If you found it helpful for your project...I always appreciate a ZEC donation to help keep it going! :)

![pestozec](https://github.com/aardev14/Pesto/assets/51981572/db168b5f-8347-4f8b-93d6-1cb643635ded)
