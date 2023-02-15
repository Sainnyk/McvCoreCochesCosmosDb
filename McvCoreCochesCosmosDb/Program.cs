using McvCoreCochesCosmosDb.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connectionString = builder.Configuration.GetConnectionString("CosmosDb");
string database = builder.Configuration.GetValue<string>("CochesCosmosDb:Database");
string container = builder.Configuration.GetValue<string>("CochesCosmosDb:Container");

//Recuperamos nuestro CosmosClient con la cadena de conexión
CosmosClient cosmosClient = new CosmosClient(connectionString); 
//Recuperamos el contenedor a partir de la base de datos y el nombre del container
Container containerCosmos = cosmosClient.GetContainer(database, container);
//Ponemos dentro de nuestra aplicacion el cliente de cosmos. Solo lo vamos a usar una vez para crear la BBDD al iniciar la aplicacion (SINGLETON)
builder.Services.AddSingleton<CosmosClient>(x => cosmosClient);
//El contenedor lo usaremos multiples veces, por lo que lo incluimos como Transient
builder.Services.AddTransient<Container>(x=> containerCosmos);
//Inyecamos tambien nuestro servicio de Cosmos para que lo puedan usar los controladores
builder.Services.AddTransient<ServiceCochesCosmos>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
