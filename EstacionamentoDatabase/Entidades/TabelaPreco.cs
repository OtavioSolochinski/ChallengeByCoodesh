using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EstacionamentoDatabase.Entidades
{
    public class TabelaPreco
    {
        public int id;
        public DateOnly data_inicio;
        public DateOnly data_fim;
        public decimal valor_hora_inicial;
        public decimal valor_hora_adicional;


        public static void CriarTabelaPrecos(DateOnly dataInicio, DateOnly dataFim, decimal valorHoraInicial, decimal valorHoraAdicional)
        {
            using (var conn = new MySqlConnection(Connection.CONNECTION_STRING))
            {
                conn.Open();

                string query = @"INSERT INTO tabela_precos 
                         (data_inicio, data_fim, valor_hora_inicial, valor_hora_adicional) 
                         VALUES (@inicio, @fim, @valorInicial, @valorAdicional);";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@inicio", dataInicio.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@fim", dataFim.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@valorInicial", valorHoraInicial);
                    cmd.Parameters.AddWithValue("@valorAdicional", valorHoraAdicional);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<TabelaPreco> GetTabelas()
        {
            List<TabelaPreco> tabelas = new List<TabelaPreco>();
            using (var conn = new MySqlConnection(Connection.CONNECTION_STRING))
            {
                conn.Open ();

                string query = "Select * from tabela_precos;";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TabelaPreco tabela = new TabelaPreco();

                            tabela.id = reader.GetInt32(0);
                            tabela.data_inicio = DateOnly.FromDateTime(reader.GetDateTime(1));
                            tabela.data_fim = DateOnly.FromDateTime(reader.GetDateTime(2));
                            tabela.valor_hora_inicial = reader.GetDecimal(3);
                            tabela.valor_hora_adicional = reader.GetDecimal(4);

                            tabelas.Add(tabela);

                        }
                    }
                    return tabelas;
                }
            }
        }
        public static void DeletarTabela(int id)
        {
            using (var conn = new MySqlConnection(Connection.CONNECTION_STRING))
            {
                conn.Open();

                string query = @"DELETE FROM tabela_precos WHERE id = @id;";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
