
using System.Collections.Generic;
using System.Threading.Tasks;
using APIsDataDriven.Data;
using APIsDataDriven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIsDataDriven.Controllers
{
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();

            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> Get(int id, [FromServices]DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return Ok(category);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> POST([FromBody] Category model, [FromServices] DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {    
                context.Categories.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new {message = "Não foi possível criar categoria"});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody] Category model, [FromServices] DataContext context)
        {
            if(model.Id != id)
                return NotFound(new { message = "Categoria não encontrada!"});

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch(DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Não foi possível atualizar a categoria"});
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Delete(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if(category == null)
                return NotFound(new {message = "Categoria não Encontrada"});

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new {message = "Categoria removida com sucesso!"});
            } 
            catch
            {
                return BadRequest(new {message = "Não foi possível remover categoria"});
            }
        }
    }
}