using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace EstacionamentoDatabase.Entidades
{
    public class Veiculo
    {

        public string placa;
        public DateTime data_hora_entrada;
        public DateTime? data_hora_saida;
        public decimal? valor_cobrado;
        public decimal? tempo;




        public static void MarcarEntrada(string placa, DateTime data_hora_entrada)
        {
            using (var conn = new MySqlConnection(Connection.CONNECTION_STRING))
            {
                conn.Open();

                string query = "Insert into veiculos (placa, data_hora_entrada) values (@placa, @data_hora_entrada);";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@placa", placa); 
                    cmd.Parameters.AddWithValue("@data_hora_entrada", data_hora_entrada); 
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static void MarcarSaida(string placa, DateTime data_hora_saida)
        {
            decimal valor_hora_inicial = 0;
            decimal valor_hora_adicional = 0;
            DateTime data_hora_entrada;

            using (var conn = new MySqlConnection(Connection.CONNECTION_STRING))
            {
                conn.Open();

                
                string queryEntrada = "SELECT data_hora_entrada FROM veiculos WHERE placa = @placa AND data_hora_saida IS NULL";
                using (var cmd = new MySqlCommand(queryEntrada, conn))
                {
                    cmd.Parameters.AddWithValue("@placa", placa);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data_hora_entrada = reader.GetDateTime(0);
                        }
                        else
                        {
                            throw new Exception("Veículo não encontrado ou já saiu.");
                        }
                    }
                }

                
                string queryPreco = @"SELECT valor_hora_inicial, valor_hora_adicional 
                              FROM tabela_precos 
                              WHERE @dataEntrada BETWEEN data_inicio AND data_fim";

                using (var cmd = new MySqlCommand(queryPreco, conn))
                {
                    cmd.Parameters.AddWithValue("@dataEntrada", data_hora_entrada);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            valor_hora_inicial = reader.GetDecimal(0);
                            valor_hora_adicional = reader.GetDecimal(1);
                        }
                        else
                        {
                            throw new Exception("Tabela de preços não encontrada para a data de entrada.");
                        }
                    }
                }

                
                TimeSpan tempo = data_hora_saida - data_hora_entrada;
                decimal valorCobrado = 0;

                if (tempo.TotalMinutes <= 30)
                {
                    valorCobrado = valor_hora_inicial / 2;
                }
                else
                {
                    valorCobrado = valor_hora_inicial;
                    double minutosExcedentes = tempo.TotalMinutes - 60;
                    if (minutosExcedentes > 0)
                    {
                        int horasAdicionais = 0;
                        while (minutosExcedentes > 0)
                        {
                            horasAdicionais++;
                            minutosExcedentes -= 70;
                        }
                        valorCobrado += horasAdicionais * valor_hora_adicional;
                    }
                }

                
                decimal tempoDecimal = (decimal)Math.Round(tempo.TotalHours, 2);

                
                string queryUpdate = @"UPDATE veiculos 
                               SET data_hora_saida = @saida, 
                                   valor_cobrado = @valor,
                                   tempo = @tempo
                               WHERE placa = @placa AND data_hora_saida IS NULL";

                using (var cmd = new MySqlCommand(queryUpdate, conn))
                {
                    cmd.Parameters.AddWithValue("@placa", placa);
                    cmd.Parameters.AddWithValue("@saida", data_hora_saida);
                    cmd.Parameters.AddWithValue("@valor", valorCobrado);
                    cmd.Parameters.AddWithValue("@tempo", tempoDecimal);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public static List<Veiculo> GetVeiculosEstacionados()
        {
            List<Veiculo> veiculos = new List<Veiculo>();

            using (var conn = new MySqlConnection(Connection.CONNECTION_STRING))
            {
                conn.Open();

                string query = "Select * from veiculos where data_hora_saida IS NULL";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            Veiculo veiculo = new Veiculo();

                            veiculo.placa = reader.GetString(0);
                            veiculo.data_hora_entrada = reader.GetDateTime(1);

                            veiculos.Add(veiculo);
                            
                        }
                    }
                }
            }
            return veiculos;
        }

        public static List<Veiculo> GetHistoricoVeiculos()
        {
            List<Veiculo> veiculos = new List<Veiculo>();

            using (var conn = new MySqlConnection(Connection.CONNECTION_STRING))
            {
                conn.Open();

                string query = "SELECT placa, data_hora_entrada, data_hora_saida, valor_cobrado, tempo FROM veiculos WHERE data_hora_saida IS NOT NULL";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Veiculo veiculo = new Veiculo();

                            veiculo.placa = reader.GetString(0);
                            veiculo.data_hora_entrada = reader.GetDateTime(1);
                            veiculo.data_hora_saida = reader.GetDateTime(2);
                            veiculo.valor_cobrado = reader.GetDecimal(3);
                            veiculo.tempo = reader.GetDecimal(4);

                            veiculos.Add(veiculo);
                        }
                    }
                }
            }

            return veiculos;
        }

        public static Veiculo BuscarPorPlaca(string placa)
        {
            using (var conn = new MySqlConnection(Connection.CONNECTION_STRING))
            {
                conn.Open();

                string query = @"SELECT placa, data_hora_entrada, data_hora_saida, valor_cobrado, tempo 
                         FROM veiculos 
                         WHERE placa = @placa 
                         LIMIT 1";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@placa", placa);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var veiculo = new Veiculo();

                            veiculo.placa = reader.GetString(0);
                            veiculo.data_hora_entrada = reader.GetDateTime(1);

                            if (!reader.IsDBNull(2))
                                veiculo.data_hora_saida = reader.GetDateTime(2);

                            if (!reader.IsDBNull(3))
                                veiculo.valor_cobrado = reader.GetDecimal(3);

                            if (!reader.IsDBNull(4))
                                veiculo.tempo = reader.GetDecimal(4);

                            return veiculo;
                        }
                    }
                }
            }

            return null;
        }

       
        







    }
}
