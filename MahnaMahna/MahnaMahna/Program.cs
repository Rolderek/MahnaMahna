using MahnaMahna.Client.Pages;
using MahnaMahna.Client.Services;
using MahnaMahna.Components;
using MahnaMahna.Data;
using MahnaMahna.Hubs;
using MahnaMahna.Services;
using MahnaMahna.Shared.Models;
using MahnaMahna.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// szervices hozzáadása
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddPooledDbContextFactory<MahnaMahnaDbContext>(options =>
 options.UseSqlite(connectionstring));

// migráció futtatásához kellenek
builder.Services.AddDbContext<MahnaMahnaDbContext>(options => options.UseSqlite(connectionstring));

//szervices-ek hozzáadása
builder.Services.AddScoped<ITodoItemService, TodoItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITodoApiService, TodoApiService>();
builder.Services.AddHttpClient();

//SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationService, NotificationService>();

//JsonSerializer máködjön EntityFrameworkkel
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.SerializerOptions.WriteIndented = true;
});
var app = builder.Build();

// HTTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
//SignalR HUB a Notification-nak
app.MapHub<NotificationHub>("/NotificationHub");

app.UseStaticFiles();
app.UseAntiforgery();

//Nem ajánlott migráció kezelésm, de máködik
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MahnaMahnaDbContext>();
    dbContext.Database.Migrate();
}
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MahnaMahna.Client._Imports).Assembly,typeof(MahnaMahna.Shared.Components.TodoList).Assembly);

//Minimal API a TodoItem-nke és a Categories-nak
app.MapGet("/api/todos", async (ITodoItemService todoItemService) =>
{
    return Results.Ok(await todoItemService.GetAllAsync());
});

app.MapGet("/api/todos/{id}", async (int id, ITodoItemService todoItemService) =>
{
    var todoItem = await todoItemService.GetByIdAsync(id);
    return todoItem == null ? Results.NotFound() : Results.Ok(todoItem);
});

app.MapPost("/api/todos", async (TodoItem todoItem, ITodoItemService todoItemService) =>
{
    var createdTodoItem = await todoItemService.CreateAsync(todoItem);
    return Results.Created($"/api/todos/{createdTodoItem.Id}", createdTodoItem);
});

app.MapPut("/api/todos/{id}", async (int id, TodoItem todoItem, ITodoItemService todoItemService) =>
{
    if (id != todoItem.Id)
    {
        return Results.BadRequest();
    }

    await todoItemService.UpdateAsync(todoItem);
    return Results.NoContent();
});

app.MapDelete("/api/todos/{id}", async (int id, ITodoItemService todoItemService) =>
{
    await todoItemService.DeleteAsync(id);
    return Results.NoContent();
});

app.MapGet("/api/categories", async (ICategoryService categoryService) =>
{
    return Results.Ok(await categoryService.GetAllAsync());
});

app.MapGet("/api/categories/{id}", async (int id, ICategoryService categoryService) =>
{
    var category = await categoryService.GetByIdAsync(id);
    return category == null ? Results.NotFound() : Results.Ok(category);
});

app.MapPost("/api/categories", async (Category category, ICategoryService categoryService) =>
{
    var createdCategory = await categoryService.CreateAsync(category);
    return Results.Created($"/api/categories/{createdCategory.Id}", createdCategory);
});

app.MapPut("/api/categories/{id}", async (int id, Category category, ICategoryService categoryService) =>
{
    if (id != category.Id)
    {
        return Results.BadRequest();
    }

    await categoryService.UpdateAsync(category);
    return Results.NoContent();
});

app.MapDelete("/api/categories/{id}", async (int id, ICategoryService categoryService) =>
{
    await categoryService.DeleteAsync(id);
    return Results.NoContent();
});




app.Run();


/* elérések:
 * "/List"
 * "/Items/{Id:int}"
 * "/categories"
 * "/board"
 */