using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        // POST: {apibaseurl}/api/auth/register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            // Create IdentityUser object
            var user = new IdentityUser
            {
                UserName = request.Email?.Trim(),
                Email = request.Email?.Trim(),
            };

            // Create User
            var identityResult = await _userManager.CreateAsync(user, request.Password);

            if (identityResult.Errors.Any())
            {
                AddErrorsToModelState(identityResult);
                return ValidationProblem(ModelState);
            }

            // Add Reader Role to user
            identityResult = await _userManager.AddToRoleAsync(user, "Reader");

            if (identityResult.Succeeded)
            {
                return Ok();
            }

            AddErrorsToModelState(identityResult);
            return ValidationProblem(ModelState);
        }

        // POST: {apibaseurl}/api/auth/login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var identityUser = await _userManager.FindByEmailAsync(request.Email);

            if (identityUser != null)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(identityUser, request.Password);

                if (checkPasswordResult)
                {
                    var roles = await _userManager.GetRolesAsync(identityUser);

                    // Create Token and Response
                    var jwtToken = _tokenRepository.CreateJwtToken(identityUser, roles.ToList());

                    var response = new LoginResponseDto()
                    {
                        Email = request.Email,
                        Roles = roles.ToList(),
                        Token = jwtToken
                    };

                    return Ok(response);
                }
            }
            ModelState.AddModelError("", "Email or Password Incorrect");
            return ValidationProblem(ModelState);
        }

        private void AddErrorsToModelState(IdentityResult identityResult)
        {
            identityResult.Errors.ToList().ForEach(error =>
            {
                ModelState.AddModelError("", error.Description);
            });
        }
    }
}
