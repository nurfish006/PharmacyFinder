namespace PharmacyFinder.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !user.IsActive)
                return Result<AuthResponseDto>.Failure("Invalid credentials");

            if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
                return Result<AuthResponseDto>.Failure("Invalid credentials");

            var token = _jwtTokenService.GenerateToken(user);
            var userDto = _mapper.Map<UserDto>(user);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = userDto
            });
        }

        public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (existingUser != null)
                return Result<UserDto>.Failure("User with this email already exists");

            var user = new User
            {
                Email = createUserDto.Email,
                PasswordHash = _passwordHasher.HashPassword(createUserDto.Password),
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Role = createUserDto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
        }

        public async Task<Result<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return Result<List<UserDto>>.Success(
                _mapper.Map<List<UserDto>>(users));
        }

        // Other methods implementation...
    }
}