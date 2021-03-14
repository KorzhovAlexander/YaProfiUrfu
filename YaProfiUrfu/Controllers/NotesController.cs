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

        /// <summary>
        /// Создает заметку
        /// </summary>
        /// <remarks>
        /// В запросе передается body в формате JSON
        ///  
        ///     POST /notes
        ///     {  
        ///         “title”: “string”,
        ///         “content”: “string”
        ///     }
        /// </remarks>
        /// <param name="note">Создаваемая запись</param>
        /// <returns>Новая созданная заметка Note</returns>
        /// <response code="201">Возвращен если запись была успешно создана</response>
        /// <response code="400">Если возникла ошибка</response>
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


        /// <summary>
        /// Получает все записи
        /// </summary>
        /// <returns>Список всех заметок</returns>
        /// <response code="200">Успешно</response>
        [HttpGet]
        public async Task<IEnumerable<Note>> Get()
        {
            return await _context.Notes.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Изменение заметки
        /// </summary>
        /// <remarks>
        /// В запросе передается body в формате JSON
        ///  
        ///     PUT /notes/{id:int}
        ///     {  
        ///         “title”: “string”,
        ///         “content”: “string”
        ///     }
        /// </remarks>
        /// <param name="id">Номер обновляемой заметки</param>
        /// <param name="note">Новые параметры заметки</param>
        /// <returns>Обновленная заметка Note</returns>
        /// <response code="200">Возвращен если запись была успешно обновлена</response>
        /// <response code="400">Если возникла ошибка</response>
        [HttpPut("{id:int}")]
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

        /// <summary>
        /// Удаление заметки
        /// </summary>
        /// <param name="id">Номер удаляемой заметки</param>
        /// <returns>удаленная заметка Note</returns>
        /// <response code="200">Возвращен если запись была успешно удалена</response>
        /// <response code="400">Если возникла ошибка</response>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Note>> Delete(int id)
        {
            try
            {
                var entity = await _context.Notes.FindAsync(id);

                if (entity == null)
                {
                    return NotFound("такой записи нет");
                }

                _context.Remove(entity);
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