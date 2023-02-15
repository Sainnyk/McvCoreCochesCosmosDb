using McvCoreCochesCosmosDb.Models;
using Microsoft.Azure.Cosmos;

namespace McvCoreCochesCosmosDb.Services
{
    public class ServiceCochesCosmos
    {
        //Se trabaja con items containers. Dentro de items containers podemos recuperar el container para trabajar con el
        //Para acceder, se usa una clase llamada CosmosClient. Dentro de este cliente se accede a los containers.
        private Container containerCosmos;
        private CosmosClient client;
        public ServiceCochesCosmos(CosmosClient client,Container container)
        {
            this.client = client;
            this.containerCosmos = container;
        }

        //*******Para trabajar con containers (creacion de contenedores -> tablas) se usa el Client.*******
        //Metodo para crear la base de datos y el container
        public async Task CreateDataBaseAsync() //Se llamara una vez, despues se añadiran o eliminaran items
        {
            //Aunque trabjaemos con el Id como primary key, esto es opcional, prodiamos indicar que la primary key es otro campo del objeto json

            //Nombre del contenedor y primary key que usara: (Todavia no está creado)
            ContainerProperties properties = new ContainerProperties("containercochesalex","/id");

            //Crear base de datos y un contenedor en su interior
            await this.client.CreateDatabaseAsync("vehiculoscosmosalex"); //Dntro de esta base de datos estara el contenedor con sus items.
            //Dentro de la base de datos creamos el container para los items con las propiedades marcadas anteriormente
            await this.client.GetDatabase("vehiculoscosmosalex").CreateContainerAsync(properties);
        }


        //*******Para trabajar con los items (vehiculos) se usa el Container.*******
        //Metodo para insertar vehiculos
        public async Task InsertVehiculoAsync(Vehiculo car)
        {
            //En el momento de crear un item debemos indicar la clase del objeto y el partition key que es la primary key (id en este caso)
            //Podria ser una partition key diferente, pero habria que enviarla como parametro tambien.
            await this.containerCosmos.CreateItemAsync<Vehiculo>(car, new PartitionKey(car.Id));
        }

        //Metodo para recuperar todos los vehiculos
        public async Task<List<Vehiculo>> GetVehiculosAsync()
        {
            //Los elementos se recorren con un Iterator que es un elemento que no sabe cuantos registros existen, simplemente va hacia delante hasta
            //que no hay mas registros.
            //Se puede hacer con Linq y pasarle en () los elementos a buscar.
            var query = this.containerCosmos.GetItemQueryIterator<Vehiculo>(); //Consulta para coger el iterator de tipo Vehiculo
            List<Vehiculo> coches = new List<Vehiculo>();
            while (query.HasMoreResults)
            {
                //Extraemos cada coche leyendo de uno en uno
                var result = await query.ReadNextAsync();
                coches.AddRange(result);  //Se hace con AddRange y no con Add porque no devuelve un "coche" como tal, con AddRange lo transforma en coche.
            }
            return coches;
        }

        public async Task UpdateVehiculoAsync(Vehiculo car)
        {
            //Tenemos un metodo llamado Upsert que es una mezcla entre Update e Insert.
            //Si lo encuentra lo modifica y si no lo encuentra lo inserta
            await this.containerCosmos.UpsertItemAsync<Vehiculo>(car, new PartitionKey(car.Id));
        }

        //Metodo para eliminar
        public async Task DeleteVehiculoAsync(string id)
        {
            await this.containerCosmos.DeleteItemAsync<Vehiculo>(id,new PartitionKey(id));
        }

        public async Task<Vehiculo> FindVehiculoAsync(string id)
        {
            //Devuelve una respuesta de vehiculo.
            ItemResponse<Vehiculo> response = await this.containerCosmos.ReadItemAsync<Vehiculo>(id, new PartitionKey(id));
            return response.Resource; //Devolvemos el recurso de la respuesta (el vehiculo)
        }

        //Metodo para buscar mediante una consulta SQL en JSON Cosmos DB
        public async Task<List<Vehiculo>> GetVehiculosMarcaAsync(string marca)
        {
            //Las consultas a Json no tienen parametros, deben realizarse concatenando
            string sql = "select * from c where c.Marca ='" + marca + "'";
            //Para filtrar se usan Query Definitions
            QueryDefinition definition = new QueryDefinition(sql);
            //A partir de la definicion, se recuperan los elementos con un Iterator pero enviando la definicion
            var query = this.containerCosmos.GetItemQueryIterator<Vehiculo>(definition);
            List<Vehiculo> coches = new List<Vehiculo>();
            while (query.HasMoreResults)
            {
                var results = await query.ReadNextAsync();
                coches.AddRange(results);
            }

            return coches;
        }







    }
}
