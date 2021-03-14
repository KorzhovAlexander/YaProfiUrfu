using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YaProfiUrfu.Dto;
using YaProfiUrfu.Entity;

namespace YaProfiUrfu.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Note>> Create(NoteCreateDto note)
        {
            try
            {
                var entity = new Note
                {
                    Content = note.Content,
                    Title = note.Title,
                };
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }


        [HttpGet]
        public async Task<IEnumerable<Note>> Get()
        {
            return await _context.Notes.AsNoTracking().ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Note>> Put(int id, NoteCreateDto note)
        {
            try
            {
                var entity = await _context.Notes.FindAsync(id);

                if (entity == null)
                {
                    return NotFound("такой записи нет");
                }

                entity.Content = note.Content;
                entity.Title = note.Title;
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }
    }
}