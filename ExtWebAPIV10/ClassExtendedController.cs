using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Primavera.WebAPI.Integration;
using StdBE100;
using RhpBE100;
using IntBE100;

namespace ExtWebAPIV10
{
    [RoutePrefix("ClassExtended")]
    public class ClassExtendedController : ApiController
    {

        [Authorize]
        [Route("PrintSalesDocToPDF/{TipoDoc}/{Serie}/{Numdoc}/{Filial}/{Numvias}/{NomeReport}/{SegundaVia}/{NomePDF}/{EntidadeFacturacao}")]
        [HttpGet]
        public HttpResponseMessage PrintSalesDocToPDF(string TipoDoc, string Serie, int Numdoc, string Filial, int Numvias, string NomeReport, bool SegundaVia, string NomePDF, int EntidadeFacturacao)
        {
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);

                // Garantir que a extensão está definida
                if (string.IsNullOrWhiteSpace(Path.GetExtension(NomePDF)))
                {
                    NomePDF += ".pdf";
                }

                // Garantir um local fisico para guardar o pdf gerado
                if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(NomePDF)))
                {
                    NomePDF = Path.Combine(Path.GetTempPath(), NomePDF);
                }

                // Imprime o documento para pdf via API
                bool imprimedocumento = ProductContext.MotorLE.Vendas.Documentos.ImprimeDocumento(TipoDoc, Serie, Numdoc, Filial, Numvias, NomeReport, SegundaVia, NomePDF, EntidadeFacturacao);

                // Prepara a resposta
                if (imprimedocumento)
                {
                    var dataBytes = File.ReadAllBytes(NomePDF);
                    var dataStream = new MemoryStream(dataBytes);

                    byte[] buffer = new byte[0];
                    buffer = dataStream.ToArray();

                    var contentLength = buffer.Length;

                    var statuscode = HttpStatusCode.OK;
                    response = Request.CreateResponse(statuscode);
                    response.Content = new StreamContent(new MemoryStream(buffer));
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    response.Content.Headers.ContentLength = contentLength;

                    if (ContentDispositionHeaderValue.TryParse("inline; filename=" + NomePDF, out ContentDispositionHeaderValue contentDisposition))
                    {
                        response.Content.Headers.ContentDisposition = contentDisposition;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        [Authorize]
        [Route("MyLstClientes")]
        [HttpGet]
        public StdBELista LstClientes()
        {
            // Motor:
            // var lstclientes = ProductContext.MotorLE.Base.Clientes.LstClientes("");

            var strSQL = string.Empty;
            strSQL = "SELECT * FROM Clientes WITH (NOLOCK)" + strSQL;


            var lstclientes = ProductContext.MotorLE.Consulta(strSQL);
            return lstclientes;
        }
    }

    [RoutePrefix("RHExtended")]
    public class RHExtendedController : ApiController
    {

        [Authorize]
        [Route("LstFuncionarios")]
        [HttpGet]
        public StdBELista LstFuncionarios()
        {
            var strSQL = string.Empty;
            strSQL = "SELECT * FROM V_Funcionarios" + strSQL;

            var lstclientes = ProductContext.MotorLE.Consulta(strSQL);

            return lstclientes;
        }

        [Authorize]
        [Route("LstRemuneracoes")]
        [HttpGet]
        public StdBELista LstRemuneracoes()
        {
            var strSQL = string.Empty;
            strSQL = "SELECT * FROM V_Remuneracoes" + strSQL;

            var lstclientes = ProductContext.MotorLE.Consulta(strSQL);

            return lstclientes;
        }

        [Authorize]
        [Route("LstAusencias")]
        [HttpGet]
        public StdBELista LstAusencias()
        {
            var strSQL = string.Empty;
            strSQL = "SELECT * FROM V_Ausencias" + strSQL;

            var lstclientes = ProductContext.MotorLE.Consulta(strSQL);

            return lstclientes;
        }

        [Authorize]
        [Route("Faltas/Lst")]
        [HttpGet]
        public StdBELista LstFaltas()
        {
            var strSQL = string.Empty;
            strSQL = "SELECT * FROM V_Faltas" + strSQL;

            var lstclientes = ProductContext.MotorLE.Consulta(strSQL);

            return lstclientes;
        }

        [Route("Faltas/Edita/{strFalta}/")]
        [HttpGet]
        public RhpBEFalta EditaFalta(string strFalta)
        {
            try
            {
                var edita = ProductContext.MotorLE.RecursosHumanos.Faltas.Edita(strFalta);

                return edita;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        [Authorize]
        [Route("Faltas/Actualiza/")]
        [HttpPost]
        public void ActualizaFalta([FromBody] RhpBEFalta BEFalta)
        {
            try
            {
                ProductContext.MotorLE.RecursosHumanos.Faltas.Actualiza(BEFalta);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        [Authorize]
        [Route("CadastroFaltas/Lst")]
        [HttpGet]
        public StdBELista LstCadastroFaltas()
        {
            var strSQL = string.Empty;
            strSQL = "SELECT * FROM V_Faltas" + strSQL;

            var lstclientes = ProductContext.MotorLE.Consulta(strSQL);

            return lstclientes;
        }

        [Authorize]
        [Route("CadastroFaltas/Edita/{strFuncionario}/{strFalta}/{dtData}/{intAcerto}/")]
        [HttpGet]
        public RhpBECadastroFalta EditaCadastroFaltas(String strFuncionario, DateTime dtData, string strFalta, int intAcerto = 0)
        {
            try
            {
                var edita = ProductContext.MotorLE.RecursosHumanos.CadastroFaltas.Edita(strFuncionario, dtData, strFalta, intAcerto);

                return edita;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        [Authorize]
        [Route("CadastroFaltas/Actualiza/")]
        [HttpPost]
        public void ActualizaCadastroFaltas([FromBody] RhpBECadastroFalta BECadastroFalta)
        {

            //                    ProductContext.MotorLE.RecursosHumanos.CadastroHorasExtra.Actualiza(item.HExtra);

            try
            {
                ProductContext.MotorLE.RecursosHumanos.CadastroFaltas.Actualiza(BECadastroFalta);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        private RhpBECadastroFalta InsereFaltaAltMens(string CodigoFuncionario, DateTime Data, double Duracao, string Observacoes, out string MensagemErro, RhpBEFalta Falta = null, bool FaltaAssociada = false, bool ExcluirProcessamento = false, bool ExcluirEstatisticas = false)
        {
            MensagemErro = "";
            try
            {
                if (Falta == null)
                {
                    MensagemErro = "A falta não existe";
                    return null;
                }

                CodigoFuncionario = CodigoFuncionario.ToUpper().Trim();

                RhpBECadastroFalta cadastroFalta = new RhpBECadastroFalta
                {
                    EmModoEdicao = ProductContext.MotorLE.RecursosHumanos.CadastroFaltas.Existe(CodigoFuncionario, Data, Falta.Falta),
                    Funcionario = CodigoFuncionario,
                    Falta = Falta.Falta,
                    Data = Data,
                    CalculoFalta = Falta.CalculoFaltaDias,
                    Horas = Falta.Horas,
                    DescontaRem = Falta.DescontaRemuneracoes != 0,
                    Observacoes = Observacoes,
                    // Origem = (byte)RhpStdComuns.OrigemDadosToInteger(OrigemDados.origemAltMens)
                    Origem = (byte)OrigemDados.origemAltMens
                };

                //Verifica se estamos a inserir uma falta automaticamente associada à falta introduzida pelo utilizador (ex: Sub. Alim.)
                double quantidade = Duracao;
                if (FaltaAssociada)
                {
                    if (cadastroFalta.Falta == ProductContext.MotorLE.RecursosHumanos.Params.CodFaltaAlim)
                    {
                        // quantidade = ProductContext.MotorLE.Base..Arredonda(quantidade + 0.1d, 0);
                    }
                    //Falta de subs alimentação sempre em valores inteiros
                    //A falta original foi inserida em dias (só neste caso a falta associada é inserida automaticamente)
                    //mas a falta associada está configurada em horas. É necessário converter os X dias para Y horas
                    if (cadastroFalta.Horas)
                        cadastroFalta.Tempo = (float)(quantidade * ProductContext.MotorLE.RecursosHumanos.Funcionarios.NumeroHorasNumDia(CodigoFuncionario));
                    else
                        cadastroFalta.Tempo = (float)quantidade;
                }
                else
                {
                    cadastroFalta.Tempo = (float)quantidade;
                }

                cadastroFalta.InseridoBloco = true;
                // cadastroFalta.Origem = (byte)RhpStdComuns.OrigemDadosToInteger(OrigemDados.origemAltMens);
                cadastroFalta.Origem = (byte)OrigemDados.origemAltMens;
                cadastroFalta.ExcluiProc = ExcluirProcessamento;
                cadastroFalta.ExcluiEstat = ExcluirEstatisticas;

                //Não é usado o actualiza do motor porque insere automaticamente 1 falta de subsídio de alimentação e/ou turno
                //o que não interessa neste caso. Aqui é dada ao utilizador a decisão de as introduzir ou não e em que quantidade.
                if (ProductContext.MotorLE.RecursosHumanos.CadastroFaltas.Existe(cadastroFalta.Funcionario, cadastroFalta.Data, cadastroFalta.Falta) && !cadastroFalta.EmModoEdicao)
                {
                    // throw new Primavera.Platform.Exceptions.ExpectedException(Primavera.Platform.Helpers.StdErros.StdErroPrevisto, "_InsereFaltaAltMens", ProductContext.MotorLE.DSO.Plat.Strings.Formata(RhpRLL.Resources.RES_str5952, cadastroFalta.Falta, cadastroFalta.Funcionario, cadastroFalta.Data));
                }
                if (ProductContext.MotorLE.RecursosHumanos.CadastroFaltas.ValidaLimitesAplicabilidade(cadastroFalta.Funcionario, cadastroFalta.Falta, cadastroFalta.Data, cadastroFalta.Tempo, ref MensagemErro))
                    ProductContext.MotorLE.RecursosHumanos.CadastroFaltas.Actualiza(cadastroFalta);
                else
                    return null;

                return cadastroFalta;

            }
            catch (Exception Ex)
            {
                MensagemErro = Ex.Message;
                return null;
            }
        }

        [Authorize]
        [Route("CadastroFaltas/Actualiza/{strFuncionario}/{strFalta}/{dtData}/")]
        [HttpPost]
        public void ActualizaCadastroFaltas(String strFuncionario, DateTime dtData, string strFalta, int intAcerto = 0)
        {

            string Observacoes = String.Empty;
            bool ExcluirProcessamento = false;
            bool ExcluirEstatisticas = false;

            RhpBEFalta Falta = ProductContext.MotorLE.RecursosHumanos.Faltas.Edita(strFalta);

            try
            {

                string mensagemErro = "";

                //Insere a falta Inicial
                InsereFaltaAltMens(strFuncionario, dtData, 1, Observacoes, out mensagemErro, Falta, false, ExcluirProcessamento, ExcluirEstatisticas);

                mensagemErro = "";

                try
                {

                    Falta = ProductContext.MotorLE.RecursosHumanos.Faltas.Edita(strFalta);

                    //Valida se a falta têm dependencias de sub alimentação ou sub turno (Apenas lança faltas se estiver configurada em dias)
                    if (!Falta.Horas && Falta.Falta != ProductContext.MotorLE.RecursosHumanos.Params.CodFaltaAlim && Falta.Falta != ProductContext.MotorLE.RecursosHumanos.Params.CodFaltaTurno)
                    {
                        StdBE100.StdBECampos camposFuncionario = ProductContext.MotorLE.RecursosHumanos.Funcionarios.DaValorAtributos(strFuncionario, "Instrumento", "SubsAlim1", "SubsAlim2", "TurnosDia", "TurnosTaxa");

                        int val = 0;

                        int.TryParse(camposFuncionario["SubsAlim1"].Valor.ToString(), out val);

                        if (val == 0)
                            int.TryParse(camposFuncionario["SubsAlim2"].Valor.ToString(), out val);

                        if (Falta.DescontaSubsAlimentacao && val != 0)
                        {
                            RhpBEFalta faltaDependente = ProductContext.MotorLE.RecursosHumanos.Faltas.Edita(ProductContext.MotorLE.RecursosHumanos.Params.CodFaltaAlim);
                            InsereFaltaAltMens(strFuncionario, dtData, 1, Observacoes, out mensagemErro, faltaDependente, true, ExcluirProcessamento, ExcluirEstatisticas);
                        }

                        val = 0;

                        int.TryParse(camposFuncionario["TurnosTaxa"].Valor.ToString(), out val);

                        if (val == 0)
                            int.TryParse(camposFuncionario["TurnosDia"].Valor.ToString(), out val);

                        if (Falta.DescontaSubsTurno && val != 0)
                        {
                            RhpBEFalta faltaDependente = ProductContext.MotorLE.RecursosHumanos.Faltas.Edita(ProductContext.MotorLE.RecursosHumanos.Params.CodFaltaTurno);
                            InsereFaltaAltMens(strFuncionario, dtData, 1, Observacoes, out mensagemErro, faltaDependente, true, ExcluirProcessamento, ExcluirEstatisticas);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    mensagemErro = Ex.Message;
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        [Authorize]
        [Route("CadastroFaltas/Remove/{strFuncionario}/{strFalta}/{dtData}/")]
        [HttpPost]
        public void RemoveCadastroFaltas(String strFuncionario, DateTime dtData, string strFalta, int intAcerto = 0)
        {
            try
            {
                ProductContext.MotorLE.RecursosHumanos.CadastroFaltas.Remove(strFuncionario, dtData, strFalta);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message));
            }
        }
    }

    [RoutePrefix("INTExtended")]
    public class INTExtendedController : ApiController
    {

        [Authorize]
        [Route("Documentos/Edita/{strTipoDoc}/{strSerie}/{intNumDoc}/")]
        [HttpGet]
        public IntBEDocumentoInterno EditaDocumentoInterno(string strTipoDoc, string strSerie, int intNumDoc)
        {
            try
            {
                var edita = ProductContext.MotorLE.Internos.Documentos.Edita(strTipoDoc, intNumDoc, strSerie, "000");

                return edita;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        [Route("Documentos/CreateDocument/")]
        [HttpPost]
        public bool CreateDocument([FromBody] IntBEDocumentoInterno clsDocumentoInterno)
        {
            IntBEDocumentoInterno clsDocumento = new IntBEDocumentoInterno
            {
                Entidade = clsDocumentoInterno.Entidade,
                Tipodoc = clsDocumentoInterno.Tipodoc,
                TipoEntidade = clsDocumentoInterno.TipoEntidade,
                Serie = clsDocumentoInterno.Serie,
                Data = clsDocumentoInterno.Data,
                DataVencimento = clsDocumentoInterno.DataVencimento 
            };

            clsDocumento.Linhas.RemoveTodos();

            try
            {
                ProductContext.MotorLE.Internos.Documentos.PreencheDadosRelacionados(clsDocumento);

                foreach (IntBELinhaDocumentoInterno linha in clsDocumentoInterno.Linhas)
                {
                    double quantidade = linha.Quantidade;

                    ProductContext.MotorLE.Internos.Documentos.AdicionaLinha(clsDocumento, linha.Artigo, Armazem: linha.Armazem, Lote: linha.Lote, Quantidade:quantidade);
                }

                ProductContext.MotorLE.Internos.Documentos.Actualiza(clsDocumento);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
