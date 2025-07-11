
using AddressBookManagement;
using AddressBookManagement.Datas;
using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Datas.Repositories.Implements;
using AddressBookManagement.Services;
using AddressBookManagement.Services.Implements;
using AddressBookManagement.Services.Shared;
using Blazored.Toast;
using Blazored.Toast.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Add DBContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
);

//Add Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//Add Services
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IMasterService, MasterService>();

//Add Blazor Toast and Service
builder.Services.AddSingleton<ToastNavigationService>();
builder.Services.AddBlazoredToast();

//Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
