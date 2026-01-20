using MySql.Data.MySqlClient;

namespace EstacionamentoDatabase
{
    public class Connection
    {
        public const string CONNECTION_STRING = "Server=localhost;Port=3306;Database=EstacionamentoTop;Uid=root;Pwd=root;";

        public MySqlConnection GetConnection()
        { 
            return new MySqlConnection(CONNECTION_STRING);
        }
    }
}
