using Microsoft.AspNetCore.Mvc;
using TODOAPI_QUERY.DTOs;

namespace TODOAPI_QUERY.Services
{
    public interface ITodoService
    {
        Task<List<TodoResponseDTO>> GetAll(string? status = null);
        Task<object> GetTodos(
             string? status,
             string? priority,
             string? sortBy,
             string? sortOrder,
             int page = 1,
             int pageSize = 10);
        Task<List<TodoResponseDTO>> Search(string q);
        Task<TodoResponseDTO?> GetById(int id);
        Task<TodoResponseDTO> Create(CreateTodoDTO dto);
        Task<TodoResponseDTO?> Update(int id, UpdateTodoDTO dto);
        Task<bool> Delete(int id);
    }
}
