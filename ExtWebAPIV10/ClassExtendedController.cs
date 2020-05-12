using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Primavera.WebAPI.Integration;
using StdBE100;
using RhpBE100;

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

        [Authorize]
        [Route("CadastroFaltas/Actualiza/{strFuncionario}/{strFalta}/{dtData}/")]
        [HttpPost]
        public void ActualizaCadastroFaltas(String strFuncionario, DateTime dtData, string strFalta, int intAcerto = 0)
        {

            RhpBECadastroFalta BECadastroFalta = new RhpBECadastroFalta();

            BECadastroFalta.EmModoEdicao = ProductContext.MotorLE.RecursosHumanos.CadastroFaltas.Existe(strFuncionario, dtData, strFalta);

            BECadastroFalta.Funcionario = strFuncionario;
            BECadastroFalta.Falta = strFalta;
            BECadastroFalta.Data = dtData;
            BECadastroFalta.Horas = false;
            BECadastroFalta.Tempo = 1;

            try
            {
                ProductContext.MotorLE.RecursosHumanos.CadastroFaltas.Actualiza(BECadastroFalta);
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
}
