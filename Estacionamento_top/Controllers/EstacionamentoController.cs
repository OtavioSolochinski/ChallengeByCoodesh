using Estacionamento_top.Models;
using EstacionamentoDatabase;
using EstacionamentoDatabase.Entidades;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Estacionamento_top.Controllers
{
    public class EstacionamentoController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var veiculo = Veiculo.GetVeiculosEstacionados();
            return View(veiculo);
        }

        [HttpGet]
        public IActionResult MarcarEntrada()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EntradaMarcada(string placa)
        {
            Veiculo.MarcarEntrada(placa, DateTime.Now);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult MarcarSaida()
        {
            var veiculo = Veiculo.GetVeiculosEstacionados();
            return View(veiculo);
        }

        [HttpPost]
        public IActionResult ConfirmacaoSaida(string placa)
        {
            var veiculo = Veiculo.BuscarPorPlaca(placa);

            if (veiculo == null)
            {
                ViewBag.Mensagem = "Veículo não encontrado.";
                return View();
            }

            DateTime data_hora_entrada = veiculo.data_hora_entrada;
            DateTime data_hora_saida = DateTime.Now;

            decimal valor_hora_inicial = 0;
            decimal valor_hora_adicional = 0;

            using (var conn = new MySqlConnection(Connection.CONNECTION_STRING))
            {
                conn.Open();

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
                            ViewBag.Mensagem = "Tabela de preços não encontrada.";
                            return View();
                        }
                    }
                }
            }

            TimeSpan tempo = data_hora_saida - data_hora_entrada;
            decimal tempoHoras = Math.Round((decimal)tempo.TotalHours, 2);

            decimal valorCobrado = 0;
            double minutos = tempo.TotalMinutes;

            if (minutos <= 30)
            {
                valorCobrado = valor_hora_inicial / 2m;
            }
            else if (minutos <= 60)
            {
                valorCobrado = valor_hora_inicial;
            }
            else
            {
                double minutosExcedentes = minutos - 60;
                int horasAdicionais = (int)Math.Ceiling(minutosExcedentes / 60.0);
                valorCobrado = valor_hora_inicial + (horasAdicionais * valor_hora_adicional);
            }

            ViewBag.Placa = veiculo.placa;
            ViewBag.Tempo = tempoHoras;
            ViewBag.Valor = valorCobrado;

            return View();
        }






        [HttpPost]
        public IActionResult SaidaMarcada(string placa)
        {
            Veiculo.MarcarSaida(placa, DateTime.Now);
            return RedirectToAction("Index");
        }

        
        [HttpGet]

        public IActionResult HistoricoVeiculos()
        {
            return View(Veiculo.GetHistoricoVeiculos());
        }



        [HttpPost]
        public IActionResult BuscarVeiculoEstacionado(string placa)
        {
            var veiculo = Veiculo.BuscarPorPlaca(placa);

            if (veiculo == null)
            {
                ViewBag.Mensagem = "Veículo não encontrado.";
                return View("ResultadoBusca2"); 
            }
            if (veiculo.data_hora_saida != null)
            {
                ViewBag.Mensagem = "Veículo não encontrado.";
                return View("ResultadoBusca2");
            }
            ViewBag.Placa = veiculo.placa;
            ViewBag.data_hora_entrada = veiculo.data_hora_entrada;


            return View("ResultadoBusca2");
        }


        [HttpPost]
        public IActionResult BuscarVeiculo(string placa)
        {
            var veiculo = Veiculo.BuscarPorPlaca(placa);

            if (veiculo == null)
            {
                ViewBag.Mensagem = "Veículo não encontrado.";
                return View("ResultadoBusca");
            }
            if (veiculo.data_hora_saida == null)
            {
                ViewBag.Mensagem = "Veículo não encontrado.";
                return View("ResultadoBusca");
            }


            ViewBag.Placa = veiculo.placa;
            ViewBag.Entrada = veiculo.data_hora_entrada;
            ViewBag.Saida = veiculo.data_hora_saida;
            ViewBag.Valor = veiculo.valor_cobrado;
            ViewBag.Tempo = veiculo.tempo;

            return View("ResultadoBusca");
        }

        [HttpGet]

        public IActionResult TabelasPrecos()
        {
            
            return View(TabelaPreco.GetTabelas());
        }

        public IActionResult CriacaoTabela() {
            return View();
        }

        [HttpPost]
        public IActionResult CriarTabela(DateOnly datainicio, DateOnly datafim, decimal horainicial, decimal horaadicional)
        {
            TabelaPreco.CriarTabelaPrecos(datainicio,datafim,horainicial,horaadicional);
            return RedirectToAction("TabelasPrecos");

        }

        public IActionResult DestruindoTabela()
        {
            return View();
        }

        public IActionResult DeletarTabela(int id)
        {
            TabelaPreco.DeletarTabela(id);
            return RedirectToAction("TabelasPrecos");
        }




    }

}
