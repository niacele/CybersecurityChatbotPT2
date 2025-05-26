using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ByteBuddy___CybersecurityChatbot
{
    class Program
    {
        //list to collect chat history
        static List<string> ChatHistory = new List<string>();

        //speech synthesizer import to read out text 
        static SpeechSynthesizer synth = new SpeechSynthesizer
        {
            Volume = 100,
            Rate = 0
        };

        //random instance to randomize chatbot responses
        static Random random = new Random();

        //dictionary for random follow-up facts
        static Dictionary<string, List<string>> randomFollowUps = new Dictionary<string, List<string>>()
{
    {"password safety", new List<string> {
        "Did you know? Password managers can generate and store strong passwords for you!",
        "Pro Tip: Changing passwords every 3 months is better than reusing old ones!",
        "Interesting fact: The most hacked password in 2023 was '123456'!"
    }},
    {"phishing awareness", new List<string> {
        "Fun fact: 96% of phishing attacks arrive by email!",
        "Remember: Hover over links before clicking to see the real URL!",
        "Did you know? Fake login pages often have subtle typos in the design!"
    }},
    {"safe browsing habits", new List<string> {
        "Security tip: Browser extensions can see everything you do - only install trusted ones!",
        "Did you know? Incognito mode doesn't make you anonymous to websites!",
        "Pro fact: VPNs encrypt your traffic even on public WiFi!"
    }},
    {"two factor authentication", new List<string> {
        "Cool fact: Physical security keys like YubiKey are virtually unhackable!",
        "Remember: Authenticator apps work even without phone service!",
        "Did you know? Apple/Google prompt-based 2FA is more secure than SMS codes!"
    }},
    {"social media safety", new List<string> {
        "Alert: Geo-tagged photos can reveal your home address!",
        "Fun fact: Scammers clone profiles using stolen photos within 2 hours!",
        "Pro tip: Review 'Apps Connected to Account' monthly!"
    }},
    {"device security basics", new List<string> {
        "Critical: iOS/Android updates often include vital security patches!",
        "Did you know? Laptops get stolen every 53 seconds at airports!",
        "Pro advice: Enable 'Find My Device' before you need it!"
    }}
};
        static string[] sentimentKeywords = { 
            //negative sentiment
            "worried", "frustrated", "confused", "angry", "scared", "overwhelmed", "stressed", "annoyed",
            //positive sentiment
             "excited", "happy", "curious", "confident", "relieved", "interested", "optimistic",
            //neutral sentiment
            "unsure", "hesitant", "skeptical", "neutral"
        };
        static string[] sentimentResponses =
        {
            //negative sentiment
            "I understand cybersecurity can feel overwhelming. Let's tackle this step by step...",
            "Digital security can be frustrating! Here's a simpler way to think about this...",
            "This can be confusing at first. Let me break it down...",
            "I hear your frustration. Cybersecurity can be tricky, but we'll work through it...",
            "Feeling scared about online threats is normal. Here's how to stay protected...",
            "Stress about security is common. Let’s focus on one thing at a time...",
            "Being annoyed by security measures is understandable. Here's why they matter...",
            "It's okay to feel overwhelmed. Let's simplify things...",

            //positive sentiment
            "Great enthusiasm! Cybersecurity is exciting when you master these concepts...",
            "Glad to hear you're happy about security! Let's keep that momentum going...",
            "Your curiosity will serve you well in learning security best practices!",
            "Confidence is key! Let’s build on that to make your digital life safer...",
            "Relief is a good sign—let's make sure you stay protected...",
            "Your interest in security is great! Here's more to explore...",
            "Optimism helps in cybersecurity! Let's keep things secure...",

            //neutral sentiment
            "It's okay to be unsure. I can clarify or suggest topics to explore...",
            "Hesitation is natural. Let me guide you through this...",
            "Skepticism is healthy in security. Let's verify things together...",
            "A neutral approach works—let's assess this carefully..."
};
        static ConsoleColor[] sentimentColors =
        {
                //colour to match the negative sentiment
                ConsoleColor.DarkMagenta, ConsoleColor.DarkMagenta, ConsoleColor.DarkMagenta, ConsoleColor.DarkMagenta,
                ConsoleColor.DarkMagenta, ConsoleColor.DarkMagenta, ConsoleColor.DarkMagenta, ConsoleColor.DarkMagenta,
                //colour to match positive sentiment
                ConsoleColor.Yellow, ConsoleColor.Yellow, ConsoleColor.Yellow, ConsoleColor.Yellow,
                ConsoleColor.Yellow, ConsoleColor.Yellow, ConsoleColor.Yellow,
                //colour to match neutral sentiment
                ConsoleColor.Cyan, ConsoleColor.Cyan, ConsoleColor.Cyan, ConsoleColor.Cyan
};

        //dictionary for memory recall
        static Dictionary<string, string> userMemory = new Dictionary<string, string>();

        static void Main(string[] args)

        {


            //instance of method that plays aduio greetung
            PlayAudioGreeting("ByteBuddyGreeting.wav"); //using wav file

            //instance of method that does console window setup
            ConsoleSetup();

            //validates and stores users name
            string username;
            do
            {
                TypingEffect("What's your name?");
                Console.ForegroundColor = ConsoleColor.Blue;
                username = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(username))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    TypingEffect("Invalid username. Please try again.");
                }

            } while (string.IsNullOrEmpty(username));

            //instance of loading effect 
            LoadingEffect();

            TypingEffect($"Hi {username}, I'm ByteBuddy and " +
               "I'm here to help you gain more cybersecurity awareness!");

            //instance of Tip of the Day method
            TipOfTheDay();

            //instance of method that prints list of topics
            Topics();
            TypingEffect("Just type the topic you wanna learn more about! " +
                "(or type 'help' to see topics again, 'exit' to quit)");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(username + " :");
                string userInput = Console.ReadLine()?.ToLower().Trim();

                //loop breaks if user types break
                if (userInput == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    string exitMessage = "ByteBuddy: Keep pushing forward! See you next time!";
                    TypingEffect(exitMessage);
                    ChatHistory.Add($"{username}: exit");
                    ChatHistory.Add(exitMessage);
                    break;
                }

                if (userInput == "help")
                {
                    //reprint topic list if user types 'help'
                    Topics();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    TypingEffect("Just type the topic you wanna learn more about!");
                    continue;
                }

                //checks whether input of user is acceptable
                if (string.IsNullOrEmpty(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    TypingEffect("ByteBuddy: Please enter a valid question.");
                    continue;
                }

                //instance of method for outputted response
                HandleUserQuery(userInput, username);
            }

            //save chat history when exiting loop
            SaveChatHistory(username);


        }





        //method for playing audio
        static void PlayAudioGreeting(string filepath)
        {
            try
            {
                //gets the full file path
                string fullpath = Path.Combine(Directory.GetCurrentDirectory(), filepath);

                if (File.Exists(fullpath))
                {
                    //play sound synchronously
                    SoundPlayer player = new SoundPlayer(fullpath);
                    player.PlaySync();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    TypingEffect("Error: the file " + filepath + " was not found at the specified location");
                }
            }

            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                TypingEffect("Error playing audio: " + ex.Message);
            }
        }

        //method for printing topics
        static void Topics()
        {

            Console.WriteLine("You can ask about the following" +
                "\n1. Password Safety" +
                "\n2. Phishing Awareness" +
                "\n3. Safe Browsing Habits" +
                "\n4. Two-Factor Authentication (2FA)" +
                "\n5. Social Media Safety" +
                "\n6. Device Security Basics" +
                "\nor type 'exit' to quit.\n");
            //TypingEffect("Just type the topic you wanna learn more about!");
        }

        //method for capturing users interect (for memory recall)
        private static void CaptureUserInterests(string input, string topic)
        {
            //user showing interest
            if (input.Contains("interested in") || input.Contains("care about") || input.Contains("focus on"))
            {
                userMemory["primaryInterest"] = topic;
                ChatHistory.Add($"ByteBuddy: [Noted user's interest in {topic}]");
            }

            //user showing concern
            if (input.Contains("worried about") || input.Contains("scared of"))
            {
                userMemory["mainConcern"] = topic;
            }
        }

        //method for detecting user sentiment
        private static void DetectSentiment(string input)
        {
            for (int i = 0; i < sentimentKeywords.Length; i++)
            {
                if (input.Contains(sentimentKeywords[i]))
                {
                    string response = sentimentResponses[i];
                    string fullResponse = $"ByteBuddy: {response}";
                    Console.ForegroundColor = sentimentColors[i];
                    TypingEffect(fullResponse);
                    ChatHistory.Add(fullResponse); // Add this line
                    Console.ResetColor();
                    synth.Speak(response);
                    break;
                }
            }
        }

        //method for interacting & outputting reponses to user
        static void HandleUserQuery(string input, string username)
        {
            //save user input
            ChatHistory.Add($"{username}: {input}");

            Dictionary<string, string> responses = new Dictionary<string, string>
            {
                {"password safety",
                    "Passwords are your first line of defense against unauthorized access. Strong, unique passwords help protect your accounts from breaches." +
                    "\n- **Use long, random passwords**: Aim for 12+ characters with a mix of letters, numbers, and symbols (e.g., `PurpleTiger$42!Bread`)." +
                    "\n- **Try a password manager**: Tools like Bitwarden or 1Password securely store passwords and generate unique ones for every account." +
                    "\n- **Never reuse passwords**: If one account is hacked, reused passwords put *all* your accounts at risk."},
                {"phishing awareness",
                    "Phishing is when attackers trick you into sharing sensitive info via fake emails, texts, or websites." +
                    "\n- **Check sender addresses**: Look for typos or odd domains (e.g., `support@amaz0n.net` instead of `amazon.com`)." +
                    "\n- **Avoid urgent/too-good links**: Hover over links to see the URL before clicking. If unsure, go directly to the official site. " +
                    "\n- **Verify requests for info**: Banks or companies will *never* ask for passwords or payment details via email/text."},
                {"safe browsing habits",
                    "Safe browsing means avoiding malicious websites and downloads that can infect your devices." +
                    "\n- **Stick to HTTPS sites**: Look for the padlock icon in your browser bar—this means data is encrypted. " +
                    "\n- **Avoid shady downloads**: Only download software/apps from official sources (e.g., Apple App Store or Google Play)." +
                    "\n- **Use an ad blocker**: Reduces accidental clicks on malicious ads (e.g., uBlock Origin).  "},
                {"two factor authentication",
                    "2FA adds an extra layer of security by requiring a second step (like a code) to log in.  " +
                    "\n- **Enable 2FA on critical accounts**: Start with email, banking, and social media (use apps like Google Authenticator). " +
                    "\n- **Avoid SMS 2FA if possible**: App-based codes or security keys (e.g., YubiKey) are harder for hackers to intercept.  " +
                    "\n- **Save backup codes offline**: Store them in a secure place (not on your device!) in case you lose access.  "},
                {"social media safety",
                    "Oversharing or lax privacy settings on social media can expose you to scams or identity theft." +
                    "\n- **Lock down privacy settings**: Restrict posts to “Friends Only” and review tagging permissions regularly. " +
                    "\n- **Avoid sharing sensitive info**: Never post IDs, vacation plans, or answers to common security questions (e.g., pet names).  " +
                    "\n- **Be wary of quizzes/links**: “Which Disney character are you?” might harvest personal data for scams.  "},
                {"device security basics",
                    "Protecting your devices (phones, laptops, tablets) prevents physical theft and malware. " +
                    "\n- **Update software ASAP**: Enable auto-updates for your OS and apps to patch security flaws. " +
                    "\n- **Use antivirus software**: Even free tools like Windows Defender add critical protection against malware.   " +
                    "\n- **Lock devices physically**: Always use a PIN/password, and never leave devices unattended in public.  "}

            };

            //key word recognition
            Dictionary<string, List<string>> keywords = new Dictionary<string, List<string>>()
            {
                {"password safety", new List<string> {"password", "strong password", "secure password", "passphrase", "password strength", "login", "credentials", "account securfity" } },
                {"phishing awareness",  new List<string> {"phishing", "phishing email", "email scam", "phishing attack", "fake text", "smishing", "scam" } },
                {"safe browsing habits",  new List<string> {"safe browsing", "secure website", "private browsing", "fake website", "download", "malware", "vpn", "https", "check url" } },
                {"two factor authentication",  new List<string> {"2fa", "mfa", "multi factor authentication", "multifactor authentication", "authenticator app", "sms codes", "recovery options" } },
                {"social media safety",  new List<string> {"social media", "privacy setting", "public profile", "catfish", "oversharing", "disable geotagging", "review followers", "location tags"} },
                {"device security basics",  new List<string> {"device security", "security", "antivirus", "anti virus", "software", "biometrics", "encyption"} }
            };

            bool foundResponse = false;
            string detectedTopic = "";



            //instance of sentiment method that looks for emotion/mood in user input
            DetectSentiment(input);

            //checks for exact topic match
            foreach (var entry in responses)
            {
                if (input.Contains(entry.Key))
                {
                    RespondWithSpeech(entry.Value);
                    detectedTopic = entry.Key;
                    foundResponse = true;
                    CaptureUserInterests(input, detectedTopic);
                    break;
                }
            }

            //checks for synonym matches if exact match not found
            if (!foundResponse)
            {
                foreach (var group in keywords)
                {

                    foreach (var synonym in group.Value)
                    {
                        if (input.Contains(synonym))
                        {
                            RespondWithSpeech(responses[group.Key]);
                            detectedTopic = group.Key;
                            foundResponse = true;
                            CaptureUserInterests(input, detectedTopic);
                            break;
                        }
                    }
                    if (foundResponse) break;
                }
            }


            if (foundResponse && randomFollowUps.ContainsKey(detectedTopic))
            {
                bool validResponse = false;
                int attempts = 0;

                do
                {
                    string morePrompt = $"ByteBuddy: Would you like to know more about {detectedTopic}? (Type 'more' or 'yes')";
                    TypingEffect(morePrompt);
                    ChatHistory.Add(morePrompt); // Log the prompt

                    Console.WriteLine(username + " :");
                    string followUpInput = Console.ReadLine()?.ToLower().Trim();
                    ChatHistory.Add($"{username}: {followUpInput}");

                    if (followUpInput == "more" || followUpInput == "yes")
                    {
                        validResponse = true;
                        var followUps = randomFollowUps[detectedTopic];
                        string randomFollowUp = followUps[random.Next(followUps.Count)];

                        if (userMemory.ContainsKey("primaryInterest") && random.Next(3) == 0)
                        {
                            RespondWithSpeech($"Since you're interested in {userMemory["primaryInterest"]}, " +
                                            $"this might be especially relevant: {randomFollowUp}");
                        }
                        else
                        {
                            RespondWithSpeech(randomFollowUp);
                            RespondWithSpeech("Type 'help' to view the topic list or 'exit' to quit.");
                        }
                    }
                    else if (followUpInput == "no")
                    {
                        validResponse = true;
                        RespondWithSpeech("No problem! Type 'help' to view the topic list or 'exit' to quit.");
                    }
                    else
                    {
                        attempts++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        TypingEffect("Please enter 'more', 'yes', or 'no'");

                        if (attempts >= 2)
                        {
                            validResponse = true; // Give up after 2 attempts
                            RespondWithSpeech("I'll assume that's a no. Let me know if you have other questions!" +
                                "\nType 'help' to view the topic list or 'exit' to quit.");
                        }
                    }
                } while (!validResponse);
            }
            else if (!foundResponse)
            {
                // Output if topic is not recognized
                RespondWithSpeech("Sorry, I didn't understand that. Type 'help' to view the topic list or 'exit' to quit.");
            }

            // Occasionally recall memory in normal responses (10% chance)
            if (foundResponse && userMemory.ContainsKey("mainConcern") && random.Next(10) == 0)
            {
                RespondWithSpeech($"By the way, regarding your concern about {userMemory["mainConcern"]}, " +
                                "this tip might help address that too...");
            }


        }


        //method for printing logo
        static void ConsoleSetup()
        {
            //console window setup
            Console.Title = "ByteBuddy.io";

            Console.ForegroundColor = ConsoleColor.Cyan;

            //top border
            Console.WriteLine(new string('=', Console.WindowWidth));

            //bytebuddy ASCI logo

            Console.WriteLine("                                                           Welcome to");
            Console.WriteLine(@"

                 _______               __                _______                   __        __           
                /       \             /  |              /       \                 /  |      /  |          
                $$$$$$$  | __    __  _$$ |_     ______  $$$$$$$  | __    __   ____$$ |  ____$$ | __    __ 
                $$ |__$$ |/  |  /  |/ $$   |   /      \ $$ |__$$ |/  |  /  | /    $$ | /    $$ |/  |  /  |
                $$    $$< $$ |  $$ |$$$$$$/   /$$$$$$  |$$    $$< $$ |  $$ |/$$$$$$$ |/$$$$$$$ |$$ |  $$ |
                $$$$$$$  |$$ |  $$ |  $$ | __ $$    $$ |$$$$$$$  |$$ |  $$ |$$ |  $$ |$$ |  $$ |$$ |  $$ |
                $$ |__$$ |$$ \__$$ |  $$ |/  |$$$$$$$$/ $$ |__$$ |$$ \__$$ |$$ \__$$ |$$ \__$$ |$$ \__$$ |
                $$    $$/ $$    $$ |  $$  $$/ $$       |$$    $$/ $$    $$/ $$    $$ |$$    $$ |$$    $$ |
                $$$$$$$/   $$$$$$$ |   $$$$/   $$$$$$$/ $$$$$$$/   $$$$$$/   $$$$$$$/  $$$$$$$/  $$$$$$$ |
                          /  \__$$ |                                                            /  \__$$ |
                          $$    $$/                                                             $$    $$/ 
                           $$$$$$/                                                               $$$$$$/  

                         "
);

            Console.WriteLine("                                          Your Personal CyberSecurity Awarness Bot!");

            //bottom border
            Console.WriteLine(new string('=', Console.WindowWidth));


        }

        static void TypingEffect(string line)
        {
            //loop for typing effect - makes it feel more conversational
            for (int i = 0; i < line.Length; i++)
            {
                //writes each character on the same line
                Console.Write(line[i]);
                System.Threading.Thread.Sleep(20);
            }
            Console.WriteLine();
        }

        static void LoadingEffect()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ByteBuddy");
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(400);
                Console.Write(".");
            }
            Console.WriteLine();
        }

        static void SaveChatHistory(string username)
        {
            string path = "chathistory.txt";
            string header = new string('=', 40) +
                $"\nConversation with {username} | {DateTime.Now:yyyy-MM-dd HH:mm}\n" +
                new string('=', 40) + "\n";

            //ensures that there are no duplicate lines saved
            var uniqueHistory = ChatHistory.Distinct().ToList();

            //saves header information
            File.WriteAllText(path, header);
            File.AppendAllLines(path, uniqueHistory);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Chat history saved to {Path.GetFullPath(path)}");
        }

        static void RespondWithSpeech(string response)
        {
            LoadingEffect();
            Console.ForegroundColor = ConsoleColor.Blue;
            string fullResponse = $"ByteBuddy: {response}";
            TypingEffect(fullResponse);
            ChatHistory.Add(fullResponse);

            // Remove the duplicate ChatHistory.Add() that was here
            try
            {
                synth.Speak(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"TTS Error: {ex.Message}");
            }
        }

        static void TipOfTheDay()
        {
            string[] tips = new string[]
            {
             "\nTip of the Day: Never share passwords via email or messaging - legit services will NEVER ask for them!",
             "\nTip of the Day: Log out of accounts on shared devices - avoid remeber me on devices you don't own.",
             "\nTip of the Day: Backup important files - use secure cloud storage or external drives.",
             "\nTip of the Day: Turn off location tags on social media - avoid revealing your home/work/school address in posts!",
             "\nTip of the Day: Review your app permissions - why does your flashlight app need access to your contacts?",
             "\nTip of the Day: Disable Bluetooth in public - hackers can exploit open connections."

            };

            int tipI = random.Next(tips.Length);
            LoadingEffect();
            Console.ForegroundColor = ConsoleColor.Green;
            RespondWithSpeech(tips[tipI]);


        }


    }
}



