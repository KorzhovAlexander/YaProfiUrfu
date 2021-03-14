using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YaProfiUrfu.Dto;
using YaProfiUrfu.Entity;

namespace YaProfiUrfu.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public NotesController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
        /// <response code="400">Если возникла ошибка (к примеру "контент должен быть указан")</response>
        [HttpPost]
        public async Task<ActionResult<Note>> Create(NoteCreateDto note)
        {
            try
            {
                if (note == null || string.IsNullOrEmpty(note.Content))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Контент должен быть указан");
                }

                var entity = new Note
                {
                    Content = note.Content,
                    Title = string.IsNullOrEmpty(note.Title) ? null : note.Title,
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
        /// Получает записи
        /// </summary>
        /// <remarks>
        /// Получение списка всех заметок, удовлетворяющих поисковому запросу
        /// (т.е. запрос содержится в наименовании (title) или в тексте заметки (content))
        ///
        /// Если заголовок не указан - то вместо него возвращать первые N символов текста заметки,
        /// где N - число, задаваемое в конфигурационном файле
        /// </remarks>
        /// <param name="query">[НЕ обязательный] параметр для фильтра заметок</param>
        /// <returns>Список всех заметок</returns>
        /// <response code="200">Успешно</response>
        [HttpGet]
        public async Task<IEnumerable<Note>> Get(string query)
        {
            var takeDefaultN = int.Parse(_configuration["TakeDefaultN"]);

            if (string.IsNullOrEmpty(query))
            {
                return await _context.Notes.Select(note => new Note
                {
                    Id = note.Id,
                    Content = note.Content,
                    Title = note.Title ?? note.Content.Substring(0, Math.Min(note.Content.Length, takeDefaultN))
                }).ToListAsync();
            }

            return await _context.Notes
                .Where(note => note.Content.Contains(query) || note.Title.Contains(query))
                .Select(note => new Note
                {
                    Id = note.Id,
                    Content = note.Content,
                    Title = note.Title ?? note.Content.Substring(0, Math.Min(note.Content.Length, takeDefaultN))
                })
                .ToListAsync();
        }

        /// <summary>
        /// Получает запись по ключу
        /// </summary>
        /// <returns>Найденную запись по ключу</returns>
        /// <response code="200">Успешно найдена</response>
        /// <response code="204">Запись не найдена</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Note>> GetByKey(int id)
        {
            var takeDefaultN = int.Parse(_configuration["TakeDefaultN"]);

            return await _context.Notes.Select(note => new Note
            {
                Id = note.Id,
                Content = note.Content,
                Title = note.Title ?? note.Content.Substring(0, Math.Min(note.Content.Length, takeDefaultN))
            }).FirstOrDefaultAsync(note => note.Id == id);
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

                if (note == null || string.IsNullOrEmpty(note.Content))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Контент должен быть указан");
                }

                entity.Content = note.Content;
                entity.Title = string.IsNullOrEmpty(note.Title) ? null : note.Title;
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