using Microsoft.EntityFrameworkCore;
using MinimalTodoApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/todoitems", async (TodoDb db) => await db.Todos.ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) => await db.Todos.Where(x => x.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id}", async (string id, TodoDb db) => await db.Todos.FindAsync(id) is Todo todo ? Results.Ok(todo) : Results.NotFound());

app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitem/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (string id, Todo todo, TodoDb db) =>
{
    var curTodo = await db.Todos.FindAsync(id);
    if (curTodo == null)
    {
        return Results.NotFound();
    }

    curTodo.Name = todo.Name;
    curTodo.IsComplete = todo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (string id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();

