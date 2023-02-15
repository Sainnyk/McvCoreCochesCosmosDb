using Newtonsoft.Json;

namespace McvCoreCochesCosmosDb.Models
{
    public class Vehiculo
    {
        //Aunque se llame ID, debemos mapearla con JSON para que lo almacene en minusculas para que lo coja.
        //Asi es como queremos que se llame dentro del Json
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } //Cosmos DB trabaja con Strings aunque sean numeros en el caso del ID SOLO
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Imagen { get; set; }
        public int VelocidadMaxima { get; set; }
        //EL MOTOR SERA OPCIONAL
        public Motor Motor { get; set; }

    }
}
