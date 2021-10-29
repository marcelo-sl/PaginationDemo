using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaginationDemo.Data;
using PaginationDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaginationDemo.Controllers
{
    [ApiController]
    [Route("v1/todos")]
    public class TodoController : ControllerBase
    {
        [HttpGet("load")]
        public async Task<IActionResult> LoadAsync(
            [FromServices] AppDbContext context)
        {
            for (int i = 0; i < 1348; i++)
            {
                var todo = new Todo()
                {
                    Id = i + 1,
                    Title = $"Tarefa {i}",
                    Done = false,
                    CreatedAt = DateTime.Now
                };
                await context.Todos.AddAsync(todo);
                await context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("skip/{skip:int}/take/{take:int}")]
        public async Task<IActionResult> GetAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int skip = 0,
            [FromRoute] int take = 25)
        {
            if (take > 1000)
                return BadRequest("The max take values is 1000 registers per request!");

            var total = await context.Todos.CountAsync();

            var todos = await context.Todos
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return Ok(new
            {
                total,
                skip,
                take,
                data = todos
            });
        }
    }
}
