
using AddressBookManagement;
using AddressBookManagement.Commons.Constants;
using AddressBookManagement.Datas;
using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Datas.Repositories.Implements;
using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.Services.Implements;
using AddressBookManagement.Services.Shared;
using Blazored.Toast;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
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
builder.Services.AddScoped<ITodoTaskService, TodoTaskService>();
builder.Services.AddScoped<IWebsiteService, WebsiteService>();
builder.Services.AddScoped<IPhoneService, PhoneService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IAppUserService, AppUserService>();

//Http Accessor
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//Password Hasher
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();

//Add authentication state
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddAuthentication(AppConstants.AuthScheme)
    .AddCookie(AppConstants.AuthScheme, cookieOptions =>
    {
        cookieOptions.Cookie.Name = AppConstants.AuthScheme;
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
    {
        googleOptions.ClientId = AppConstants.ClientId;
        googleOptions.ClientSecret = AppConstants.ClientSecret;
        googleOptions.AccessDeniedPath = AppConstants.AccessDeniedPath;
    });

//Reminder watcher
builder.Services.AddScoped<ReminderWatcher>();


//Add Blazor Toast and Service
builder.Services.AddSingleton<FilterService>();
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

//Use authen and author
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
