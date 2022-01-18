using System;
using System.Linq;
using System.Security.Cryptography;


namespace ConsoleApp
{
    public static class Program
    {
        private const int MaxUtf32 = 0x10FFFF;

        public enum OperationType
        {
            Encrypt,
            Decrypt
        }
        static void Main(string[] args)
        {
            var choice = "";
            do
            {
                Console.WriteLine("C) Cesar");
                Console.WriteLine("V) Vigenere");
                Console.WriteLine("R) RSA");
                Console.WriteLine("D) Diffie Hellman");
                Console.WriteLine("T) Terminate");
                Console.WriteLine("--------------");
                Console.Write("Your choice: ");

                choice = Console.ReadLine()?.Trim().ToUpper();

                switch (choice)
                {
                    case "V":
                        Vigenere();
                        break;
                    case "C":
                        Cesar();
                        break;
                    case "R":
                        Rsa();
                        break;
                    case "D":
                        DiffieHellman();
                        break;
                }
            } while (choice != "T");
            
        }

        private static void Vigenere()
        {
            var choice = "";
            do
            {
                Console.WriteLine("***** Vigenere *****");
                Console.WriteLine("E) Encrypt");
                Console.WriteLine("D) Decrypt");
                Console.WriteLine("T) Terminate");
                Console.WriteLine("--------------");
                Console.Write("Your choice: ");

                choice = Console.ReadLine()?.Trim().ToUpper();

                if (choice is "E" or "D")
                {
                    Console.Write("Your key: ");
                    var key = Console.ReadLine()?.Trim();

                    Console.Write("Your text: ");
                    var text = Console.ReadLine()?.Trim();

                    Console.WriteLine(Cypher(key, text, choice == "E" ? OperationType.Encrypt : OperationType.Decrypt));
                    break;
                }
                
            } while (choice != "T");
        }

        private static void Cesar()
        {
            var choice = "";

            do
            {
                Console.WriteLine("***** Cesar ******");
                Console.WriteLine("E) Encrypt");
                Console.WriteLine("D) Decrypt");
                Console.WriteLine("T) Terminate");
                Console.WriteLine("--------------");
                Console.Write("Your choice: ");

                choice = Console.ReadLine()?.Trim().ToUpper();

                if (choice is "E" or "D")
                {
                    var inputIsInt = false;
                    var key = "";
                    do
                    {
                        Console.Write("Your rotation number: ");
                        key = Console.ReadLine()?.Trim();
                        if (!int.TryParse(key, out var keyInt))
                        {
                            Console.WriteLine("Not an valid integer!");
                            continue;
                        }

                        while (keyInt < 0)
                        {
                            keyInt += MaxUtf32;
                        }

                        keyInt %= MaxUtf32;

                        key = char.ConvertFromUtf32(keyInt);
                        inputIsInt = true;

                    } while (!inputIsInt);
                    
                    

                    Console.Write("Your text: ");
                    var text = Console.ReadLine()?.Trim();

                    Console.WriteLine(Cypher(key, text, choice == "E" ? OperationType.Encrypt : OperationType.Decrypt));
                    break;
                }
            } while (choice != "T");
        
            
            Console.Write("Shift by: ");
        }

        public static string CesarCypher(int key, string text, OperationType type)
        {
            while (key < 0)
            {
                key += MaxUtf32;
            }
            
            key %= MaxUtf32;
            
            return Cypher(char.ConvertFromUtf32(key), text, type) ;
        }
        public static string Cypher(string key, string text, OperationType type)
        {
            // Normalizing the key string
            if (key.GetUtf32Length() != text.GetUtf32Length())
            {
                //Make sure that key string is at least the size of cipher string, or more
                while (key.GetUtf32Length() < text.GetUtf32Length())
                {
                    key += key;
                }
                //Make sure they're the same length by truncating the possibly longer key string
                key = key.Utf32Substring(0, text.GetUtf32Length());
            }
            var cipherText = "";
            // rotate the value of ith character in plaintext by the UTF32 value of the ith character in 
            // key string
            foreach (var codePoints in text.GetUnicodeCodePoints().Zip(key.GetUnicodeCodePoints()))
            {
                var unicodeValue = RotateCharacterValue(codePoints.First, codePoints.Second, type);
                cipherText += char.ConvertFromUtf32(unicodeValue);
            }

            return cipherText;
        }
        
        private static int RotateCharacterValue(int charValue, int keyValue, OperationType type)
        {
            // Rotate character value. If encrypting, add key value, otherwise subtract
            charValue = type == OperationType.Encrypt ? charValue + keyValue : charValue - keyValue;
            charValue = NormalizeCharValue(charValue, type);
            while (char.IsControl(char.ConvertFromUtf32(charValue), 0))
            {
                // Rotate character value out of the undesirable (control character) number range.
                // If encrypting, add key value, otherwise subtract
                charValue = type == OperationType.Encrypt ? charValue + 1 : charValue - 1;
                charValue = NormalizeCharValue(charValue, type);
            }

            return charValue;
        }
        
        private static int NormalizeCharValue(int charValue, OperationType type)
        {
            // Normalize the actual number we're going to shift with
            // In c#, number field springs up from number 0 and flows outwards
            // Hence, modulo operator works weirdly.
            // Assume |A| > |B| and A > 0 and B < 0
            // Modulo operator, in B % A should return ||B| - |A||
            // But what it actually returns, is -||B|%|A||
            // Next line remedies that
            while (charValue < 0)
            {
                charValue = MaxUtf32 + charValue;
            }
            // loop back on the number field up from 0
            charValue %= MaxUtf32;
            // If character value lands in the surrogate number range, get out of there
            // If encrypting, make it 0xe000, otherwise 0xd799 (right before and after the range)
            if (charValue is >= 0xd800 and <= 0xdfff)
            {
                charValue = type == OperationType.Decrypt ? 0xd800 - 1 : 0xdfff + 1 ;
            }

            return charValue;
        }

        private static void Rsa()
        {
            
            var choice = "";
            var rsa = new int[3] ;

            do
            {
                Console.WriteLine("***** RSA *****");
                Console.WriteLine("E) Encrypt");
                Console.WriteLine("D) Decrypt");
                Console.WriteLine("T) Terminate");
                Console.WriteLine("--------------");
                Console.Write("Your choice: ");

                choice = Console.ReadLine()?.Trim()?.ToUpper();
                
                switch (choice)
                {
                    case "E":
                        Console.WriteLine("Input your string");
                        rsa =PrimeCrypto.GenerateRSA();
                        Console.WriteLine(PrimeCrypto.RsaEnc(Console.ReadLine()?.Trim(),rsa[1],rsa[0]));
                        Console.WriteLine("n = " + rsa[0]);
                        Console.WriteLine("e = " + rsa[1]);
                        Console.WriteLine("d = " + rsa[2]);
                        break;
                    case "D":
                        Console.WriteLine("Input your string");
                        int n, d;
                        if (rsa[1] == 0)
                        {
                            Console.WriteLine("Input your n (shared part of the key)");
                            while (true)
                            {
                                if (!int.TryParse(Console.ReadLine()?.Trim(), out n))
                                {
                                    Console.WriteLine("Not an valid integer!");
                                    continue;
                                }
                                break;
                            }
                            Console.WriteLine("Input your d (private part of the key)");
                            while (true)
                            {
                                if (!int.TryParse(Console.ReadLine()?.Trim(), out d))
                                {
                                    Console.WriteLine("Not an valid integer!");
                                    continue;
                                }
                                break;
                            }
                            Console.WriteLine(PrimeCrypto.RsaDec(Console.ReadLine()?.Trim(), d,n));
                            break;
                        }
                        Console.WriteLine(PrimeCrypto.RsaDec(Console.ReadLine()?.Trim(), rsa[2],rsa[0]));
                        break;
                }
            } while (choice != "T");
        }

        private static void DiffieHellman() 
        {
            var choice = "";
            do
            {
                Console.WriteLine("***** RSA *****");
                Console.WriteLine("T) Terminate");
                Console.WriteLine("--------------");
                Console.Write("Your choice: ");
                Console.WriteLine("Pick secret integer to encrypt");
                choice = Console.ReadLine()?.Trim().ToUpper();
                if (!int.TryParse(choice, out var secret))
                {
                    Console.WriteLine("Not an valid integer!");
                    continue;
                }

                var (prime, primitive, serverSecret, ourPartial, theirPartial, sharedOur) =
                    PrimeCrypto.GenerateDH(0, 0, secret);
                
                Console.WriteLine("Prime number P is: " + prime);
                Console.WriteLine("Primitive (Base) number is: " + primitive);
                Console.WriteLine("Server side secret:" + serverSecret);
                Console.WriteLine("Server Partial secret: " + ourPartial);
                Console.WriteLine("User Partial secret: " + theirPartial);
                Console.WriteLine("Shared secret: " + sharedOur);

            } while (choice != "T");
        }
    }
}