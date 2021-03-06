﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pericles.Crypto;
using VoterDatabase;
using ElectionModels;
using ElectionModels.FirstPastThePost;
using ElectionModels.InstantRunoff;
using Pericles.Votes;

namespace Pericles.VoterTerminal
{
    class VoterTerminal: IVoterTerminal
    {
        VoterDatabaseFacade voterDb;
        string[] candidateArr;
        EncryptedKeyPair keyPair;
        IVoteSerializer voteSerializer;
        string password;

        public VoterTerminal(VoterDatabaseFacade voterDb, IVoteSerializer voteSerializer) {
            this.voterDb = voterDb;
            this.candidateArr = new string[] { "Donald Trump", "Arnold Schawarzeneggar", "Oprah Winfrey", "Patrick Yukman"};
            this.voteSerializer = voteSerializer;
        }

        public bool login(string password, out EncryptedKeyPair crypticKeyPair)
        {
            Console.WriteLine("Please enter your Voter Regestration Identification Number:\n");
            string userSuppliedPassword = Console.ReadLine();
            crypticKeyPair = null;
            var success = this.voterDb.TryGetVoterEncryptedKeyPair(userSuppliedPassword, out var encryptedKeyPair);
            if (!success)
            {
                return false;
            }
            this.keyPair = encryptedKeyPair;
            this.password = userSuppliedPassword;
            return true;
        }

        void ballotPrompt(ElectionType electionType) {
            
            if (electionType == ElectionType.FirstPastThePost)
            {
                var i = 1;
                Console.WriteLine("Welcome to the election! These are the following candidates to select from:\n" +
                    string.Join("\n", this.candidateArr.Select(x => $"{i++}: {x}"))
                    );
                int counter = 1;
                int choice = 0;
                while (counter < 4) {
                    Console.WriteLine("Please select your choice of candidate e.g. 1.");
                    choice = Convert.ToInt32(Console.ReadLine());
                    if (choice < 1 || choice > candidateArr.Length)
                    {
                        Console.WriteLine("Please enter a valid choice.");
                        counter += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (counter == 4)
                {
                    Console.WriteLine("You've exhausted your tries. Bye Bye");
                    return;
                }
                FirstPastThePostVote irVote = new FirstPastThePostVote(this.candidateArr[choice - 1]);
                var jsonVote = this.voteSerializer.Serialize(irVote);
                var signature = SignatureProvider.Sign(this.password, this.keyPair, jsonVote.GetBytes());
                Vote vote = new Vote(this.keyPair.PublicKey.GetBase64String(), jsonVote, signature.GetBase64String());
            }
            else if (electionType == ElectionType.InstantRunoff)
            {
                var i = 1;
                Console.WriteLine("Welcome to the election! These are the following candidates to select from:\n" +
                    string.Join("\n", this.candidateArr.Select(x => $"{i++}: {x}"))
                    );
                // We need to consider when a candidate is not wishing NONE
                int counter = 1;
                var prefs = "";
                char[] charSeparators = new char[] { ',' };
                while (counter < 4)
                {
                    Console.WriteLine("Please type a list in order of candidate preference e.g. 2, 1, 3.");
                    prefs= Console.ReadLine();
                    string[] tokens = prefs.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if () //TODO validation check of voter input check
                    {
                        Console.WriteLine("Please enter a valid choice.");
                        counter += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (counter == 4)
                {
                    Console.WriteLine("You've exhausted your tries. Bye Bye");
                    return;
                }
                InstantRunoffVote iroVote = new InstantRunoffVote(); // make the IR vote
                var jsonVote = this.voteSerializer.Serialize(iroVote);
                var signature = SignatureProvider.Sign(this.password, this.keyPair, jsonVote.GetBytes());
                Vote vote = new Vote(this.keyPair.PublicKey.GetBase64String(), jsonVote, signature.GetBase64String());
            }
        }

        string getResult() { }

    }
}
