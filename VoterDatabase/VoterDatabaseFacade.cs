using System.Data.SQLite;

namespace VoterDatabase
{
    public class VoterDatabaseFacade
    {
        private readonly string sqlLiteDatabaseFile;
        private readonly SQLiteConnection sqlLiteConnection;

        public VoterDatabaseFacade(string sqlLiteDatabaseFile)
        {
            this.sqlLiteDatabaseFile = sqlLiteDatabaseFile;
        }

        public void CreateNew()
        {
            SQLiteConnection.CreateFile(this.sqlLiteDatabaseFile);
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