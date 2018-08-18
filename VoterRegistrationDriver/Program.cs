using System;
using System.Security.Cryptography;
using Pericles.Crypto;
using VoterDatabase;

namespace VoterRegistrationDriver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var password = args[0];
            var db = new VoterDatabaseFacade("../../../VoterDatabase/pericles_voter_db.sqlite");

            //db.CreateVoter(password);

            EncryptedKeyPair keyPair;
            db.TryGetVoterEncryptedKeyPair(password, out keyPair);

            RSACryptoServiceProvider privateKey;
            PrivateKeyProvider.TryGetPrivateKey(keyPair.EncryptedPrivateKey, password, keyPair.InitializationVector, out privateKey);

            // ENCRYPTION DEMO
            //var myPassword = "monkey123";
            //var myBallot = "I vote for... NO ONE";

            //var myEncryptedKeyPair = EncryptedKeyPairGenerator.Generate(myPassword);

            //// These 3 strings go in the database:
            //var publicKeyString = myEncryptedKeyPair.PublicKey.GetBase64String();
            //var encryptedPrivateKeyString = myEncryptedKeyPair.EncryptedPrivateKey.GetBase64String();
            //var ivString = myEncryptedKeyPair.InitializationVector.GetBase64String();

            //RSACryptoServiceProvider privateKey;
            //var success = PrivateKeyProvider.TryGetPrivateKey(
            //    Convert.FromBase64String(encryptedPrivateKeyString),
            //    myPassword,
            //    Convert.FromBase64String(ivString),
            //    out privateKey);
            //if (!success)
            //{
            //    throw new Exception("couldn't decrypt private key");
            //}

            //var signature = privateKey.SignData(myBallot.GetBytes(), new SHA256CryptoServiceProvider());

            //var publicKey = PublicKeyProvider.GetPublicKey(Convert.FromBase64String(publicKeyString));
            //var isSignatureValid = publicKey.VerifyData(myBallot.GetBytes(), new SHA256CryptoServiceProvider(), signature);

            //if (!isSignatureValid)
            //{
            //    throw new Exception("Signature is invalid!");
            //}

            //Console.WriteLine("Signature validated, everything is working amazingly");


            // DB CREATION
            //var db = new VoterDatabaseFacade("../../../VoterDatabase/sqlite/pericles_voter_db.sqlite");
            //db.CreateNew();

            //db.ExecuteCommand("create table voters (voter_id nvarchar, encrypted_private_key nvarchar, iv nvarchar, pw_hash nvarchar)");

            Console.ReadKey();
        }
    }
}
