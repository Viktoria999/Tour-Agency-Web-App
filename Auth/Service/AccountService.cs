using DAL;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Auth.Service
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly JwtService _jwtService;
        private readonly IFileService _fileService;
        public AccountService(JwtService jwtService, IAccountRepository accountRepository, IFileService fileService)
        {
            _jwtService = jwtService;
            _accountRepository = accountRepository;
            _fileService = fileService;
        }

        public void Register(UserModel model, string password)
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                Email = model.Email,
                ProfilePictureUrl = model.ProfilePictureUrl
            };
            var passwordHash = new PasswordHasher<Account>().HashPassword(account, password);
            account.PasswordHash = passwordHash;

            _accountRepository.Insert(account);
        }
        public void EditProfile(UserModel model)
        {
            var account = new Account
            {
                Id = model.Id,
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                Email = model.Email,
                ProfilePictureUrl = model.ProfilePictureUrl
            };
            account.PasswordHash = _accountRepository.GetByUserName(model.UserName)?.PasswordHash;

            _accountRepository.Edit(account);
        }
        public string Login(string userName, string password)
        {
            var account = _accountRepository.GetByUserName(userName);
            if (account == null)
            {
                throw new Exception($"Пользователь с именем {userName} не существует.");
            }

            var verificationResult = new PasswordHasher<Account>().VerifyHashedPassword(account, account.PasswordHash, password);
            if (verificationResult == PasswordVerificationResult.Success)
            {
                return _jwtService.GenerateToken(account);
            }
            else
            {
                throw new Exception($"Введён неверный пароль.");
            }
        }

        public Account? GetAccount(string authorizationToken)
        {
            var principal = _jwtService.ValidateToken(authorizationToken);
            if (principal == null)
                return null;

            var userName = principal.FindFirst("userName")?.Value;
            if (string.IsNullOrEmpty(userName))
                return null;

            return _accountRepository.GetByUserName(userName);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            return _jwtService.ValidateToken(token);
        }

        public string? GetTokenForPasswordRestoration(string userName, string email)
        {
            var account = _accountRepository.GetByUserName(userName);
            if (account == null || account.Email != email)
            {
                return null;
            }

            return _jwtService.GenerateToken(account);
        }

        public void RestorePassword(string newPassword, string userName)
        {
            var account = _accountRepository.GetByUserName(userName);
            if (account == null)
                throw new Exception("Аккаунт с таким именем пользователя не найден");

            var passwordHash = new PasswordHasher<Account>().HashPassword(account, newPassword);
            account.PasswordHash = passwordHash;

            _accountRepository.Edit(account);
        }

        public async Task<string?> SaveUserProfilePictureAsync(IFormFile file, string userName)
        {
            var account = _accountRepository.GetByUserName(userName);
            if (account == null)
                throw new Exception("Аккаунт с таким именем пользователя не найден");

            var fileName = await _fileService.SaveProfilePictureAsync(file, userName);
            if (fileName == null)
                return null;

            account.ProfilePictureUrl = $"/assets/profile_pictures/{fileName}";
            _accountRepository.Edit(account);

            return fileName;
        }
    }
}
