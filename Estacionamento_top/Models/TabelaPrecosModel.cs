namespace Estacionamento_top.Models
{
    public class TabelaPrecosModel
    {
        public int id;
        public DateOnly data_inicio;
        public DateOnly data_fim;
        public decimal valor_hora_inicial;
        public decimal valor_hora_adicional;
    }
}
