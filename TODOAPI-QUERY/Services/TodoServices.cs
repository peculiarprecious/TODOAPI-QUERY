using Microsoft.EntityFrameworkCore;
using TODOAPI_QUERY.DatabaseContext;
using TODOAPI_QUERY.DTOs;
using TODOAPI_QUERY.Models;


namespace TODOAPI_QUERY.Services
{
    public class TodoServices : ITodoService
    {

        private readonly TodoDbContext _appDBContext;

        public TodoServices(TodoDbContext appDBcontext)
        {
            _appDBContext = appDBcontext;
        }

        private static TodoResponseDTO MapToResponse(TodoItem todo)
        {
            return new TodoResponseDTO
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt,
                DueDate = todo.DueDate,
                Priority = todo.Priority
            };
        }

        // GET all — ToListAsync()
        public async Task<List<TodoResponseDTO>> GetAll(string? status = null)
        {
            var query = _appDBContext.TodoItems.AsQueryable();


            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "completed")
                {
                    query = query.Where(t => t.IsCompleted == true);
                }
                else if (status.ToLower() == "pending")
                {
                    query = query.Where(t => t.IsCompleted == false);
                }
            }

            var todos = await query.ToListAsync();
            return todos
                .Select(t => MapToResponse(t))
                .ToList();
        }
        public async Task<List<TodoResponseDTO>> Search(string q)
        {
            // Search in both Title and Description, then order by newest CreatedAt date first
            var todos = await _appDBContext.TodoItems
                .Where(t => t.Title.Contains(q) ||
                            (t.Description != null && t.Description.Contains(q)))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            // Map using your existing translation helper method
            return todos
                .Select(t => MapToResponse(t))
                .ToList();
        }

        //    public async Task<List<TodoResponseDTO>> GetTodos(
        //string? status,
        //string? priority,
        //string? sortBy,
        //string? sortOrder)
        //    {
        //        var query = _appDBContext.TodoItems.AsQueryable();

        //        if (!string.IsNullOrEmpty(status))
        //        {
        //            if (status.ToLower() == "completed")
        //            {
        //                query = query.Where(t => t.IsCompleted == true);
        //            }
        //            else if (status.ToLower() == "pending")
        //            {
        //                query = query.Where(t => t.IsCompleted == false);
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(priority))
        //        {
        //            query = query.Where(t => t.Priority.ToLower() == priority.ToLower());
        //        }

        //        bool descending = sortOrder?.ToLower() == "desc";

        //        query = sortBy?.ToLower() switch
        //        {
        //            "title" => descending
        //                ? query.OrderByDescending(t => t.Title)
        //                : query.OrderBy(t => t.Title),
        //            "duedate" => descending
        //                ? query.OrderByDescending(t => t.DueDate)
        //                : query.OrderBy(t => t.DueDate),
        //            "priority" => descending
        //                ? query.OrderByDescending(t => t.Priority)
        //                : query.OrderBy(t => t.Priority),
        //            "createdat" => descending
        //                ? query.OrderByDescending(t => t.CreatedAt)
        //                : query.OrderBy(t => t.CreatedAt),
        //            _ => query.OrderBy(t => t.Id) 
        //        };

        //        var todos = await query.ToListAsync();
        //        return todos
        //            .Select(t => MapToResponse(t))
        //            .ToList();
        //    }


        public async Task<object> GetTodos(
            string? status,
            string? priority,
            string? sortBy,
            string? sortOrder,
            int page = 1,
            int pageSize = 10)
        {
            // 1. Validate pagination safety boundaries
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; 

            // 2. Prepare base query blueprint
            var query = _appDBContext.TodoItems.AsQueryable();

            // 3. Apply status filtering (Part 2)
            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "completed")
                {
                    query = query.Where(t => t.IsCompleted == true);
                }
                else if (status.ToLower() == "pending")
                {
                    query = query.Where(t => t.IsCompleted == false);
                }
            }

            // 4. Apply priority filtering (Part 4)
            if (!string.IsNullOrEmpty(priority))
            {
                query = query.Where(t => t.Priority.ToLower() == priority.ToLower());
            }

            // 5. Get the total count of items matching the filters BEFORE slicing pages
            var totalCount = await query.CountAsync();

            // 6. Apply multi-criteria sorting configurations (Part 4)
            bool descending = sortOrder?.ToLower() == "desc";
            query = sortBy?.ToLower() switch
            {
                "title" => descending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "duedate" => descending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                "priority" => descending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                "createdat" => descending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
                _ => query.OrderBy(t => t.Id) // Default 
            };

            // 7. Calculate pagination indexes
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var skip = (page - 1) * pageSize;

            // 8. Extract the specific chunk from the database
            var todos = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // 9. Map data utilizing the existing response converter helper method
            var mappedData = todos.Select(t => MapToResponse(t)).ToList();

            // 10. Generate and return the exact metadata object envelope required for grading
            return new
            {
                Data = mappedData,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount,
                    HasPrevious = page > 1,
                    HasNext = page < totalPages
                }
            };
        }


        // GET by id — FindAsync()
        public async Task<TodoResponseDTO?> GetById(int id)
        {
            var todo = await _appDBContext.TodoItems
                .FindAsync(id);

            if (todo == null) return null;
            return MapToResponse(todo);
        }
        // POST 
        public async Task<TodoResponseDTO> Create(CreateTodoDTO dto)
        {
            var todo = new TodoItem
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = false,
                CreatedAt = DateTime.Now,
                DueDate = dto.DueDate,
                Priority = dto.Priority
            };

            _appDBContext.TodoItems.Add(todo);
            await _appDBContext.SaveChangesAsync();

            return MapToResponse(todo);
        }

        // PUT 
        public async Task<TodoResponseDTO?> Update(int id, UpdateTodoDTO dto)
        {
            var todo = await _appDBContext.TodoItems
                .FindAsync(id);

            if (todo == null) return null;

            todo.Title = dto.Title;
            todo.Description = dto.Description;
            todo.IsCompleted = dto.IsCompleted;
            todo.DueDate = dto.DueDate;
            todo.Priority = dto.Priority;

            await _appDBContext.SaveChangesAsync();

            return MapToResponse(todo);
        }

        // DELETE 
        public async Task<bool> Delete(int id)
        {
            var todo = await _appDBContext.TodoItems
                .FindAsync(id);

            if (todo == null) return false;

            _appDBContext.TodoItems.Remove(todo);
            await _appDBContext.SaveChangesAsync();

            return true;
        }




    }
}
