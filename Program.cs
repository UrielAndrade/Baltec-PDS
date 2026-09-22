
using Baltec.Components;
using Baltec.configs;
using Baltec.DAO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<DatabaseConnection>();

builder.Services.AddScoped<UsuarioDAO>();
builder.Services.AddScoped<CertificadoCalibracaoDAO>();
builder.Services.AddScoped<ClienteDAO>();
builder.Services.AddScoped<EquipamentoDAO>();

// DAO para requisições de peças das Ordens de Serviço
builder.Services.AddScoped<OrdemServicoPecaDAO>();
builder.Services.AddScoped<OrdemServicoDAO>();

// DAOs para componentes e controle de estoque
builder.Services.AddScoped<ComponenteDAO>();
builder.Services.AddScoped<MovimentacaoEstoqueDAO>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
