using McvCoreCochesCosmosDb.Models;
using McvCoreCochesCosmosDb.Services;
using Microsoft.AspNetCore.Mvc;

namespace McvCoreCochesCosmosDb.Controllers
{
    public class CochesController : Controller
    {
        private ServiceCochesCosmos service;
        public CochesController(ServiceCochesCosmos service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string accion)
        {
            await this.service.CreateDataBaseAsync();
            ViewData["MENSAJE"] = "Datos creados correctamente en Cosmos";
            return View();
        }

        public async Task<IActionResult> ListadoVehiculos()
        {
            List<Vehiculo> coches = await this.service.GetVehiculosAsync();
            return View(coches);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Vehiculo car, string existemotor)
        {
            //Si no recibimos el motor, lo ponemos a null para que no incluya el objeto vacio dentro del JSON de Cosmos Db
            if(existemotor == null)
            {
                car.Motor = null;
            }
            await this.service.InsertVehiculoAsync(car);
            return RedirectToAction("ListadoVehiculos");
        }

        public async Task<IActionResult> Details(string id)
        {
            Vehiculo car = await this.service.FindVehiculoAsync(id);
            return View(car);
        }
        public async Task<IActionResult> Delete(string id)
        {
            await this.service.DeleteVehiculoAsync(id);
            return RedirectToAction("ListadoVehiculos");
        }

        public async Task<IActionResult> Edit(string id)
        {
            Vehiculo car = await this.service.FindVehiculoAsync(id);
            return View(car);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Vehiculo car)
        {

            await this.service.UpdateVehiculoAsync(car);
            return RedirectToAction("ListadoVehiculos");
        }






    }
}
