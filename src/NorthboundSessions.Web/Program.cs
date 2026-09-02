using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Web.Services;
using NorthboundSessions.Web.Components;
using NorthboundSessions.Web.Components.Account;
using NorthboundSessions.Web.Data;
using Microsoft.AspNetCore.HttpOverrides;


var builder = WebApplication.CreateBuilder(args);

// Azure Container Apps terminates HTTPS at its own ingress and forwards
// plain HTTP to the container — without this, ASP.NET Core doesn't know
// the original request was actually HTTPS, which breaks Identity's
// cookie/antiforgery handling specifically (not simple GET requests).
var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeaderOptions.KnownNetworks.Clear();
forwardedHeaderOptions.KnownProxies.Clear();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<SlideService>();
builder.Services.AddScoped<LessonGeneratorService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
Console.WriteLine("=== DATABASE CONFIG ===");

Console.WriteLine($"Connection string found: {!string.IsNullOrEmpty(connectionString)}");

Console.WriteLine($"Connection string length: {connectionString.Length}");

Console.WriteLine($"Connection string starts with: {connectionString.Substring(0, Math.Min(40, connectionString.Length))}");

Console.WriteLine("======================");

/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())); */

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<QuizService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ReportingService>();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; //true; set this to true when we implement the email verification
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())

{

    var dbContextFactory = scope.ServiceProvider

        .GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

    using var dbContext = dbContextFactory.CreateDbContext();

    dbContext.Database.Migrate();

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseForwardedHeaders(forwardedHeaderOptions);

app.UseAuthentication();

app.UseAuthorization();

app.UseAntiforgery();

app.UseDefaultFiles();

app.UseStaticFiles();

app.MapStaticAssets();

app.MapGet("/", async context =>

{

    context.Response.ContentType = "text/html";

    await context.Response.SendFileAsync(

        Path.Combine(app.Environment.WebRootPath, "index.html"));

});
app.MapGet("/api/lessons/{id}/image", 
async (
    int id, IDbContextFactory<ApplicationDbContext> dbFactory
    ) => { 
        await using var context = await dbFactory.CreateDbContextAsync(); 
        var lesson = await context.Lessons.FindAsync(id); 
        if (lesson?.ImageBytes is null) 
        { 
            return Results.Redirect("/images/lessons/continue_learning_default.jpg"); 
        } 
        return Results.File(lesson.ImageBytes, "image/jpeg"); });

app.MapRazorComponents<App>()

    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

//Seeding the Instructor role and assign it to my email
using (var scope = app.Services.CreateScope()) 
    { var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); 
      var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(); 
        if (!await roleManager.RoleExistsAsync("Instructor")) 
            { 
                await roleManager.CreateAsync(new IdentityRole("Instructor")); 
            } 
      var instructorUser = await userManager.FindByEmailAsync("fehintolusamuel@gmail.com"); 
        if (instructorUser is not null && !await userManager.IsInRoleAsync(instructorUser, "Instructor"))
            { 
                await userManager.AddToRoleAsync(instructorUser, "Instructor"); 
            } 
    } 

app.Run();