using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using MvcDonativosInsight.Models;
using System.Diagnostics;

namespace MvcDonativosInsight.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private TelemetryClient telclient;

        public HomeController(ILogger<HomeController> logger, TelemetryClient telclient)
        {
            _logger = logger;
            this.telclient = telclient;
        }

        public IActionResult Index()
        {
            this._logger.LogInformation("Entrando en indexx.cshtml GET " + DateTime.Now);
            return View();
        }
        [HttpPost]
        public IActionResult Index(string usuario, int cantidad)
        {
            this._logger.LogInformation("Entrando en indexx.cshtml POST " + DateTime.Now);
            ViewData["MENSAJE"] = "Su donativo de " + cantidad + " ha sido aceptado. Muchas gracias Sr/Sra " + usuario;
            //NECESITAMOS CREAR UN CONTROL DE TELEMETRIA NUEVO PARA PODER INCLUIR AHÍ ESTOS DATOS
            this.telclient.TrackEvent("DonativosRequest");
            //PODEMOS CREAR UN RESUMEN NUMERO SOBRE LOS DONATIVOS POR EJEMPLO VISUALIZAMOS LA SUMA DE LOS DONATIVOS
            MetricTelemetry metrica = new MetricTelemetry();
            metrica.Name = "Donativos";
            metrica.Sum = cantidad;
            //AGREGAMOS LA NUEVA METRICA A AZURE
            this.telclient.TrackMetric(metrica);
            //TAMBIEN PODEMOS CREAR TRAZAS CON DIFERENTES NIVELES DE SEVERIDAD
            string mensaje = usuario + " " + cantidad + "€.";
            SeverityLevel level;
            if (cantidad == 0)
            {
                level = SeverityLevel.Error;
            }
            else if (cantidad < 5)
            {
                level= SeverityLevel.Critical;
            }
            else if (cantidad < 20)
            {
                level= SeverityLevel.Warning;
            }
            else
            {
                level= SeverityLevel.Information;
            }
            TraceTelemetry trace = new TraceTelemetry(mensaje, level);
            this.telclient.TrackTrace(trace);
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}