namespace PharmacyFinder.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetUserByIdAsync(int id);
        Task<Result<List<UserDto>>> GetAllUsersAsync();
        Task<Result<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Result<UserDto>> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<Result<bool>> DeleteUserAsync(int id);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto);
    }
}