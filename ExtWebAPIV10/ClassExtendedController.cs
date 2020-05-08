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
}
