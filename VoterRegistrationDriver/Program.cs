using System;
using System.Security.Cryptography;
using Pericles.Crypto;

namespace VoterRegistrationDriver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var myPassword = "monkey123";
            var myBallot = "I vote for... NO ONE";

            var myEncryptedKeyPair = EncryptedKeyPairGenerator.Generate(myPassword);

            // These 3 strings go in the database:
            var publicKeyString = myEncryptedKeyPair.PublicKey.GetBase64String();
            var encryptedPrivateKeyString = myEncryptedKeyPair.EncryptedPrivateKey.GetBase64String();
            var ivString = myEncryptedKeyPair.InitializationVector.GetBase64String();

            RSACryptoServiceProvider privateKey;
            var success = PrivateKeyProvider.TryGetPrivateKey(
                Convert.FromBase64String(encryptedPrivateKeyString),
                myPassword,
                Convert.FromBase64String(ivString),
                out privateKey);
            if (!success)
            {
                throw new Exception("couldn't decrypt private key");
            }

            var signature = privateKey.SignData(myBallot.GetBytes(), new SHA256CryptoServiceProvider());

            var publicKey = PublicKeyProvider.GetPublicKey(Convert.FromBase64String(publicKeyString));
            var isSignatureValid = publicKey.VerifyData(myBallot.GetBytes(), new SHA256CryptoServiceProvider(), signature);

            if (!isSignatureValid)
            {
                throw new Exception("Signature is invalid!");
            }

            Console.WriteLine("Signature validated, everything is working amazingly");
            Console.ReadKey();
        }
    }
}
