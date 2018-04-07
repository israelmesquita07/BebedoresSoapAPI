using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
/// WebService que verifica o perfil do usuário em relação à bebida e dose que o mesmo escolhe.
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

public class WebService : System.Web.Services.WebService
{

    public WebService(){  }

    public static List<Bebidas> listaBebidas = new List<Bebidas>();
    public static List<DosesDiarias> listaDosesDiarias = new List<DosesDiarias>();
    public static List<string> listaPerfil = new List<string>();

    [WebMethod]
    public List<string> Perfil(int id_Doses)
    {
        DosesDiarias dose = new DosesDiarias();
        dose = listaDosesDiarias.Find(i => i.Id == id_Doses);

        if(dose == null) { return listaPerfil; } //Caso não se tenha informado a Bebida anteriormente.
        listaPerfil.Add(dose.Descricao);
        return listaPerfil;
    }

    [WebMethod]
    public List<Bebidas> Bebidas()
    {
        listaBebidas.Clear();
        List<string> linhaValues = new List<string>();
        StreamReader r = LerArquivo("BEBIDAS"); //Lê o arquivo com o nome informado no parâmetro.
        r.ReadLine(); //ignorar cabeçalho
        while (r.Peek() != -1)
        {
            string line = r.ReadLine(); //Se lê linha por linha
            linhaValues = line.Split('|').ToList(); //E se separa por caracter específico
            listaBebidas.Add(new Bebidas() { Id = int.Parse(linhaValues[0]), Descricao = linhaValues[1], Conteudo = linhaValues[2] });
        }
            
        return listaBebidas;
    }

    [WebMethod]
    public List<DosesDiarias> Doses(int id_Bebida)
    {
        listaDosesDiarias.Clear();
        listaPerfil.Clear();
        List<BebidasDosesDiarias> listBebidaDose = ListarBebidasDosesDiarias(id_Bebida);
        List<string> linhaValues = new List<string>();
        StreamReader r = LerArquivo("DOSES_DIARIAS");
        r.ReadLine(); //ignorar cabeçalho
        while (r.Peek() != -1 && listBebidaDose.Count > 0)
        {
            string line = r.ReadLine();
            linhaValues = line.Split('|').ToList();
            if (int.Parse(linhaValues[0]) == listBebidaDose[0].IdDosesDiarias)
            {
                listBebidaDose.RemoveAt(0);//remove primeiro índice
                listaDosesDiarias.Add(new DosesDiarias() { Id = int.Parse(linhaValues[0]), QuantidadeDose = linhaValues[1], Descricao = linhaValues[2] });
            }
        }

        Bebidas conteudo = new Bebidas();
        conteudo = listaBebidas.Find(i => i.Id == id_Bebida);
        listaPerfil.Add(conteudo.Conteudo);

        return listaDosesDiarias;
    }

    private List<BebidasDosesDiarias> ListarBebidasDosesDiarias(int idBebida)
    {

        List<BebidasDosesDiarias> lista = new List<BebidasDosesDiarias>();
        List<string> linhaValues = new List<string>();
        StreamReader r = LerArquivo("BEBIDAS_DOSES_DIARIAS");
        r.ReadLine(); //ignorar cabeçalho
        while (r.Peek() != -1)
        {
            string line = r.ReadLine();
            linhaValues = line.Split('|').ToList();
            if (int.Parse(linhaValues[0]) == idBebida)
            {
                lista.Add(new BebidasDosesDiarias() { IdBebidas = int.Parse(linhaValues[0]), IdDosesDiarias = int.Parse(linhaValues[1]) });
            }
        }
            
        return lista;
    }

    private StreamReader LerArquivo(string nomeArquivo) {

        nomeArquivo = @"../../../"+nomeArquivo+".txt";
        StreamReader r = new StreamReader(nomeArquivo, System.Text.Encoding.Default);
        return r;

    }

}
