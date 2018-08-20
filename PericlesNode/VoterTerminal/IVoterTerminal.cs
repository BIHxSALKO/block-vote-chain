using System;
using System.Collections.Generic;
using System.Linq;
using Pericles.Crypto;
using System.Text;
using System.Threading.Tasks;
using ElectionModels;

namespace Pericles.VoterTerminal
{
    interface IVoterTerminal
    {
        bool login(string password, out EncryptedKeyPair crypticKeyPair);
        void ballotPrompt(ElectionType electionType);
        string getResult();
    }
}
