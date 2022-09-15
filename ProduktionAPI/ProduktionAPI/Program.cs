using ProduktionAPI.BLL;
using ProduktionAPI.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Dependeny Injection
var serviceCollection = new ServiceCollection();
IConfiguration configuration;
configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
    .AddJsonFile("appsettings.json")
    .Build();
serviceCollection.AddSingleton<IConfiguration>(configuration);
serviceCollection.AddSingleton<SAPBOne>();

var app = builder.Build();

//add CORS
app.UseCors(builder =>
{
    builder.AllowAnyOrigin();
});


//Connect To SAP B1 
var serviceProvider = serviceCollection.BuildServiceProvider();
var sapb1 = serviceProvider.GetService<SAPBOne>();

app.MapGet("/Connect", () =>
{
    try
    {
        var oConnect = new ConnectBLL();
        var user = oConnect.Connect();
        return Results.Ok(user);
    }
    catch (global::System.Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
app.MapGet("/Lagerbestands", (string ItemCode) =>
{
    try
    {
        var bestand = new LagerbestandBLL(configuration);
        var bestandLst = bestand.Lagerbestands(ItemCode);
        return Results.Ok(bestandLst);
    }
    catch (global::System.Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }

});
app.MapGet("/BOM", (string ItemCode) =>
{
    try
    {
        var bomBLL = new BillOfMaterialBLL();
        var bom = bomBLL.BillOfMaterials(ItemCode);
        return Results.Ok(bom);
    }
    catch (global::System.Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }

});
app.MapGet("/GetMonteur", (int IDNummer) =>
{
    try
    {
        var monteurBLL = new MonteurBLL();
        var monteur = monteurBLL.GetMonteur(IDNummer);
        return Results.Ok(monteur);
    }
    catch (global::System.Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }

});
app.MapGet("/GetProdAuftrag", (int PANummer) =>
{
    try
    {
        var prodAuftragBLL = new ProduktionAuftragBLL();
        var prodAuftrag = prodAuftragBLL.GetProdAuftrag(PANummer);
        return Results.Ok(prodAuftrag);
    }
    catch (global::System.Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }

});
app.MapGet("/UpdateProdAuftrag", (string DocNum, int PaStatus, int MonteurCode) =>
{
    try
    {
        var prodAuftragBLL = new ProduktionAuftragBLL();
        var prodAuftrag = prodAuftragBLL.UpdateProdAuftrag(DocNum, PaStatus, MonteurCode);
        return Results.Ok(prodAuftrag);
    }
    catch (global::System.Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }

});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();





