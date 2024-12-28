using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace asp.net.core.jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/<AuthController>
        [HttpGet]
        public IActionResult Get()
        {
            bool flag=true;
            if (flag)
            {
                var claims = new List<Claim>() {
                       new Claim(ClaimTypes.Role, "systemAdmin")
                };

             var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Key")));
             var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

             var tokeOptions = new JwtSecurityToken(//יצירת אובייקט הטוקן
             issuer: _configuration.GetValue<string>("JWT:Issuer"),//מזהה את השרת שהנפיק את הטוקן
             audience: _configuration.GetValue<string>("JWT:Audience"),//מזהה את היישום שמורשה להשתמש בטוקן
             claims: claims,//רשימת הטענות שצורפו לטוקן
             expires: DateTime.Now.AddMinutes(30),//מועד פקיעת הטוקן (עוד 30 דקות)
             signingCredentials: signinCredentials//קרדנציאלים לחתימה על הטוקן
            );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);//ה-JwtSecurityTokenHandler ממיר את האובייקט של הטוקן למחרוזת המוכרת כ-JWT

                // יצירת ה-Cookie
                Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
                {
                    HttpOnly = true, // לא נגיש דרך JavaScript
                    Secure = true, // רק ב-HTTPS
                    SameSite = SameSiteMode.Strict, // למנוע שליחה חוצה דומיינים
                    //כשאין זה תוקף אז אוטומטי בעת סגירת הדפדפן ימחק מהקוקיז
                    //Expires = DateTime.Now.AddMinutes(6) // תוקף הטוקן
                });

                return Ok(new { Token = tokenString });
            }
            return Unauthorized();
        }

        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AuthController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AuthController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
