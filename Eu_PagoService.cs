using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using ClickViajaBot.EuPagoService;
using System.Threading.Tasks;

namespace ClickViajaBot.Services
{
    public class Eu_PagoService
    {
        private static string chaveEuPago       = ConfigurationManager.AppSettings["EuPago_Key"].ToString();
        private static API_LibraryClient client = new API_LibraryClient();

        /// <summary>
        /// Método que invoca o servico da EuPago para gerar refs com datas de validade
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<gerarReferenciaMBDLResponse> GeraReferenciaMBDLAsync(gerarReferenciaMBDLRequest obj)
        {
            obj.chave = chaveEuPago;

            return await client.gerarReferenciaMBDLAsync(obj);
        }

        /// <summary>
        /// Método que invoca o servico da EuPago para gerar pedido MBWay
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<pedidoMBWResponse> GeraPedidoMBWayAsync(pedidoMBWRequest obj)
        {
            obj.chave = chaveEuPago;

            return await client.pedidoMBWAsync(obj);
        }
    }
}