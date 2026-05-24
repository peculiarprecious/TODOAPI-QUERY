using TODOAPI_QUERY.DTOs;

namespace TODOAPI_QUERY.Services
{
    public interface ITodoService
    {
        Task<List<TodoResponseDTO>> GetAll(string? status = null);
        Task<TodoResponseDTO?> GetById(int id);
        Task<TodoResponseDTO> Create(CreateTodoDTO dto);
        Task<TodoResponseDTO?> Update(int id, UpdateTodoDTO dto);
        Task<bool> Delete(int id);
    }
}
