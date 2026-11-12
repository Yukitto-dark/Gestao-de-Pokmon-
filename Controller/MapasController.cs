using System.Windows.Controls;

namespace GestaoDePokemon.Controller
{
    public class MapasController
    {
        public void AbrirRegiao(Page page, string regiao)
        {
            NavegacaoController.NavegarParaRegiao(page, regiao);
        }
    }
}
