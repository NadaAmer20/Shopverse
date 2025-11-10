using MediatR;
using Microsoft.AspNetCore.Http;
using Shopverse.Application.DTOs.User;
using Shopverse.Application.Features.Identity.Commands.RegisterUser;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Interfaces.Communication;
using Shopverse.Domain.Interfaces.Security;

    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, ApiResponse<Guid>>
    {
        #region Fields
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly IRepository<Seller> _sellerRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService; 
        private readonly IEmailService _emailService;  
        #endregion

        #region Constructor
        public RegisterUserHandler(
            IRepository<User> userRepo,
            IRepository<Role> roleRepo,
            IRepository<Seller> sellerRepo,
            IHttpContextAccessor httpContextAccessor,
            IJwtService jwtService,
            IEmailService emailService) 
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _sellerRepo = sellerRepo;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
            _emailService = emailService; 
        }
        #endregion

        #region Handle Method
        public async Task<ApiResponse<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var exists = await IsUserExistAsync(request.Request, cancellationToken);
            if (exists)
            {
                return ApiResponse<Guid>.Fail("User already exists.");
            }

            var user = await CreateUserAsync(request.Request);

            var role = await AssignRoleToUserAsync(request.Request.RoleId, user, cancellationToken);
            if (role == null)
            {
                return ApiResponse<Guid>.Fail("Invalid role.");
            }

            await HandleSpecificRoleAsync(role.Name, user.Id, request.Request.PhoneNumber, cancellationToken);

            var authTokenResult = await _jwtService.GenerateToken(user.Id, user.Username, user.Email);

            user.Token = authTokenResult.Token;
            user.RefreshToken = authTokenResult.newRefreshToken;
            user.IsActive = false;  

            await _userRepo.UpdateAsync(user, CancellationToken.None);

            var activationLink = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/activate-account?token={authTokenResult.Token}";

            var subject = "Activate Your Account";
            var body = $"Hello {user.Username},<br/>Please click the link below to activate your account:<br/><a href=\"{activationLink}\">Activate Account</a>";

            await _emailService.SendEmailAsync(user.Email, subject, body, cancellationToken);

            return ApiResponse<Guid>.Success(user.Id, "User registered successfully. Please check your email to activate your account.");
        }
        #endregion

        #region Helper Methods
        private async Task<bool> IsUserExistAsync(RegisterUserRequest req, CancellationToken cancellationToken)
        {
            return await _userRepo.IsExistAsync(u => u.Email == req.Email || u.Username == req.Username, cancellationToken);
        }

        private async Task<User> CreateUserAsync(RegisterUserRequest req)
        {
            var user = new User
            {
                Username = req.Username,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = false
            };

            await _userRepo.AddAsync(user);
            return user;
        }

        private async Task<Role?> AssignRoleToUserAsync(Guid roleId, User user, CancellationToken cancellationToken)
        {
            var role = await _roleRepo.GetByIdAsync(roleId);
            if (role != null)
            {
                user.RoleId = role.Id;
                user.Role = role;
            }
            return role;
        }

        private async Task HandleSpecificRoleAsync(string roleName, Guid userId, string phoneNumber, CancellationToken cancellationToken)
        {
            if (roleName == "Seller")
            {
                var seller = new Seller
                {
                    UserId = userId
                };
                await _sellerRepo.AddAsync(seller, cancellationToken);
            }
        }
        #endregion
    }

