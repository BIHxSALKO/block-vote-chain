using System;
using System.Data.SQLite;
using Pericles.Crypto;

namespace VoterDatabase
{
    public class VoterDatabaseFacade
    {
        private readonly string sqlLiteDatabaseFile;

        public VoterDatabaseFacade(string sqlLiteDatabaseFile)
        {
            this.sqlLiteDatabaseFile = sqlLiteDatabaseFile;
        }

        public void CreateNew()
        {
            SQLiteConnection.CreateFile(this.sqlLiteDatabaseFile);
        }

        public void CreateVoter(string voterPassword)
        {
            var encryptedKeyPair = EncryptedKeyPairGenerator.Generate(voterPassword);
            var voterId = encryptedKeyPair.PublicKey.GetBase64String();
            var encryptedPrivateKey = encryptedKeyPair.EncryptedPrivateKey.GetBase64String();
            var iv = encryptedKeyPair.InitializationVector.GetBase64String();
            var hashedPassword = encryptedKeyPair.HashedPassword.GetBase64String();

            var insertStatement =
                $"insert into voters (voter_id, encrypted_private_key, iv, pw_hash) values (\"{voterId}\", \"{encryptedPrivateKey}\", \"{iv}\", \"{hashedPassword}\")";
            this.ExecuteCommand(insertStatement);
        }

        public bool TryGetVoterEncryptedKeyPair(string password, out EncryptedKeyPair encryptedKeyPair)
        {
            var passwordHash = PasswordHasher.Hash(password);
            var connection = this.OpenConnection();
            var queryString = $"select * from voters where pw_hash = \"{passwordHash.GetBase64String()}\"";
            var query = new SQLiteCommand(queryString, connection);
            var reader = query.ExecuteReader();

            if (!reader.Read())
            {
                encryptedKeyPair = null;
                return false;
            }

            var voterIdString = (string)reader["voter_id"];
            var encryptedPrivateKeyString = (string)reader["encrypted_private_key"];
            var ivString = (string)reader["iv"];
            var hashedPasswordString = (string)reader["pw_hash"];

             encryptedKeyPair = new EncryptedKeyPair(
                 Convert.FromBase64String(voterIdString),
                 null,
                 Convert.FromBase64String(encryptedPrivateKeyString),
                 Convert.FromBase64String(ivString),
                 Convert.FromBase64String(hashedPasswordString));
            return true;
        }

        public void ExecuteCommand(string commandString)
        {
            var connection = this.OpenConnection();
            var command = new SQLiteCommand(commandString, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        private SQLiteConnection OpenConnection()
        {
            var connection = new SQLiteConnection($"DataSource={this.sqlLiteDatabaseFile};Version=3");
            connection.Open();
            return connection;
        }
    }
}