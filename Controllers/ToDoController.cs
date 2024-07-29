using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using ToDoApp.Models;
using ToDoApp.services;
using System.Linq;

namespace ToDoApp.Controllers
{
    public class ToDoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ToDoController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Functions to display all the tasks
        //public async Task<IActionResult> Index()
        //{
        //    var todoItems = await _context.ToDos.ToListAsync();
        //    return View(todoItems);
        // }

        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
        {
            Console.WriteLine($"Received request - Search: {searchString}, Page: {page}, PageSize: {pageSize}");

            ViewData["CurrentFilter"] = searchString;

            var todos = from t in _context.ToDos
                        select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                todos = todos.Where(t => t.Title.Contains(searchString));
            }

            var totalItems = await todos.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var todoItems = await todos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new ToDoViewModel
            {
                ToDos = todoItems,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            Console.WriteLine($"Returning {todoItems.Count} items");

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TodoTable", viewModel);
            }

            return View(viewModel);
        }

        //this function display the form
        public IActionResult Create()
        {
            return View();
        }

        //this function post the form data to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,IsCompleted,DueDate")] ToDo todo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }


        //this function is used to load form with the data of the specified id populated in it
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.ToDos.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }
            return View(todoItem);
        }

        //this function submits the form data to the database and returns exception in case of !.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IsCompleted,DueDate")] ToDo todo)
        {
            if (id != todo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        // To obtain the targetted item you want to delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.ToDos.FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }


        //To delete the item
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoItem = await _context.ToDos.FindAsync(id);
            _context.ToDos.Remove(todoItem);
            await _context.SaveChangesAsync();  
            return RedirectToAction(nameof(Index));
        }


        //Private function that check whether the specified id exists or not
        private bool TodoItemExists(int id)
        {
            return _context.ToDos.Any(x => x.Id == id);
        }
    }
}
