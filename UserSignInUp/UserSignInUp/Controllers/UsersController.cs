using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using UserSignInUp.DataAccess;
using UserSignInUp.Models;

namespace UserSignInUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public UsersController(DataContext dataContext) 
        { 
            _dataContext = dataContext;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {

            if(_dataContext.Users.Any(u=>u.Email==request.Email))
            {
                return BadRequest("User already exist.");
            }

            CreatePassworHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

           User user = new User{
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken()

            };

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();

            return Ok("User succesfully created!");
            
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x=> x.Email == request.Email);
            
            if(user == null)
            {
                return BadRequest("User not found");

            }

            if (!VerifyPassworHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password is incorrect");
            }

            if (user.VerifiedAt == null)
            {
                return BadRequest("Not verified'");
            }

          

            return Ok($"Welcome back,{user.Email}");
        }



        [HttpPost("verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.VerificationToken == token);

            if (user == null)
            {
                return BadRequest("Invalid token");

            }

           user.VerifiedAt = DateTime.Now;
           await _dataContext.SaveChangesAsync();

            return Ok("User verified");
        }




        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                return BadRequest("User not found");

            }

            user.PasswordResetToken=CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);

            await _dataContext.SaveChangesAsync();

            return Ok("You may now reset your password");
        }





        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.PasswordResetToken == request.Token);

            if (user == null || user.ResetTokenExpires<DateTime.Now)
            {
                return BadRequest("Invalid Token");

            }
            CreatePassworHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            

            await _dataContext.SaveChangesAsync();

            return Ok("Password successfully reser.");
        }








        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private void CreatePassworHash(string password,out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac=new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassworHash(string password,  byte[] passwordHash,  byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
