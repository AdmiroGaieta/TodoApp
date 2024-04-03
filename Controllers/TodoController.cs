using System.Net;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TodoAPP.Models;
using TodoAPP.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TodoAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    public class TodoController : ControllerBase
    {
         private  readonly ApiDbContext _context;
         public  TodoController(ApiDbContext context)
         {
            _context = context;
         }

         // Verbo para Filtro - GET
         [HttpGet]
         public async Task<IActionResult> GetItems()
         {
            var items= await _context.Items.ToListAsync();
             return Ok(items);
         }

          // Verbo para inserção - POST

         [HttpPost]
         public async Task<IActionResult> CreateItem(ItemData data)
         {
            if(ModelState.IsValid)
            {
              await _context.Items.AddAsync(data);
              await _context.SaveChangesAsync();
               return CreatedAtAction("GetItem", new { id = data.Id }, data);
            }
           return StatusCode(500, "Error Creating Item");

         }

            // Verbo para Filtro - GET

         [HttpGet("{id}")]  
        public async Task<IActionResult> GetItem(int id)
        {
           var item =await _context.Items.FindAsync(id);
          if (item == null)
          {
              return NotFound();
          }
          return Ok(item);
       }   
       [HttpPut("{id}")]

       // Verbo para Update - PUT
       public async Task<IActionResult> UpdateItem(int id, ItemData item)
       {
         if(id != item.Id)
          return BadRequest();
         
         var existItem = await  _context.Items.FirstOrDefaultAsync(i => i.Id==id);
         if(existItem==null)
          return NotFound();

          existItem.Title=item.Title;
          existItem.Description=item.Description;
          existItem.Done=item.Done;
          await  _context.SaveChangesAsync();
          return NoContent();        
      }
      [HttpDelete("{id}")]
     // Metodo de Eliminacion Logica - DELETE   
     public async Task<IActionResult> DeleteItem(int id)
     {
         var existItem = await _context.Items.FirstOrDefaultAsync(i=>i.Id==id);
         if (existItem == null)
             return NotFound();
             _context.Items.Remove(existItem);
             await _context.SaveChangesAsync();
             return Ok(existItem);
     }  

   }                           
}