using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIsDataDriven.Models;
using APIsDataDriven.Data;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using APIsDataDriven.Services;

namespace APIsDataDriven.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post([FromServices] DataContext context, [FromBody] User model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try 
            {
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password = string.Empty;

                return model;
            }
            catch
            {
                return BadRequest(new {message = "Não foi possível salvar o Usuario"}); 
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromServices] DataContext context, [FromBody] User model)
        {
            var user = await context
                .Users
                .AsNoTracking()
                .Where(u => u.UserName == model.UserName && u.Password == model.Password)
                .FirstOrDefaultAsync();
            
            if(user == null)
                return NotFound(new {message = "usuário ou senha invalidos"});

            var token = TokenService.GenerateToken(user);

            user.Password = string.Empty;

            return new 
            {
                user = user,
                token = token 
            };
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> GetAction([FromServices] DataContext context)
        {
            var users = await context.Users.AsNoTracking().ToListAsync();

            return Ok(users);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put([FromServices] DataContext context, int id, [FromBody] User model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
        

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return model;
            }
            catch
            {
                return BadRequest(new {message = "Não foi possivel atualizaro o usuario"});
            }
        }
    }
}