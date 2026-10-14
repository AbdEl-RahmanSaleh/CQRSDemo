using API.Requests;
using Application.Todos.Commands.CreateTodo;
using Application.Todos.Commands.DeleteTodo;
using Application.Todos.Commands.UpdateTodo;
using Application.Todos.Queries.GetTodoById;
using Application.Todos.Queries.GetTodos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController(IMediator mediator) : ControllerBase
    {
        private const string todoId = "{todoId:guid}";

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var todos = await mediator.Send(new GetTodosQuery());
            return Ok(todos);
        }


        [HttpGet(todoId, Name = "GetTodo")]
        public async Task<IActionResult> GetTodo(Guid todoId)
        {
            var todo = await mediator.Send(new GetTodoByIdQuery(todoId));

            return todo is null ? NotFound() : Ok(todo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo(CreateTodoRequest request)
        {
            var command = new CreateTodoCommand(request.Title);
            var todoId = await mediator.Send(command);
            return CreatedAtRoute("GetTodo", new { todoId = todoId }, null);
        }
        [HttpPut(todoId)]
        public async Task<IActionResult> UpdateTodo(Guid todoId, UpdateTodoRequest request)
        {
            var command = new UpdateTodoCommand(todoId, request.Title,request.Completed);
            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete(todoId)]
        public async Task<IActionResult> DeleteTodo(Guid todoId)
        {
            var command = new DeleteTodoCommand(todoId);
            await mediator.Send(command);
            return NoContent();
        }

    }
}
