using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel server to listen on all IPs (0.0.0.0) and port 8080
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7000); // change port here if needed
});

// Add services
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlite("Data Source=todos.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure DB created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles(); // serve index.html by default
app.UseStaticFiles();

// API endpoints
app.MapGet("/api/todos", async (TodoContext db) =>
    await db.Todos.OrderBy(t => t.Id).ToListAsync());

app.MapGet("/api/todos/{id}", async (int id, TodoContext db) =>
    await db.Todos.FindAsync(id) is TodoItem todo ? Results.Ok(todo) : Results.NotFound());

app.MapPost("/api/todos", async (TodoItem todo, TodoContext db) =>
{
    todo.CreatedAt = DateTime.UtcNow;
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

app.MapPut("/api/todos/{id}", async (int id, TodoItem input, TodoContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo == null) return Results.NotFound();
    todo.Title = input.Title;
    todo.IsDone = input.IsDone;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/todos/{id}", async (int id, TodoContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo == null) return Results.NotFound();
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

// ------------------- Models + DbContext -------------------
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }
    public DbSet<TodoItem> Todos { get; set; }
}
