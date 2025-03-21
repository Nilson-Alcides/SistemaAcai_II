namespace SistemaAcai_II.Models
{
    public class VendasViewModel
    {
        public IEnumerable<ProdutoSimples> Produtos { get; set; } = new List<ProdutoSimples>();
        public IEnumerable<ProdutoSimples> ItensCarrinho { get; set; } = new List<ProdutoSimples>();
        public Comanda Comanda { get; set; } = new Comanda();
        public ItemComanda ItemComanda { get; set; } = new ItemComanda();
    }
}
