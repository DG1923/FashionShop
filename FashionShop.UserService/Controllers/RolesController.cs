using FashionShop.UserService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FashionShop.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly UserDbContext _context;

        public RolesController(UserDbContext context)
        {
            _context = context;
        }
        // GET: api/<RolesController>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IEnumerable<IdentityRole<Guid>> GetAllRoles()
        {
            return _context.Roles.ToList();
        }

        // GET api/<RolesController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]

        public IdentityRole<Guid> Get(string id)
        {
            Guid guid = Guid.TryParse(id, out var parsedGuid) ? parsedGuid : Guid.Empty;
            if (guid == Guid.Empty)
            {
                throw new ArgumentException("Invalid GUID format.", nameof(id));
            }
            var role = _context.Roles.Find(guid);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {guid} not found.");
            }
            return role;
        }

        // POST api/<RolesController>
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public void Post([FromBody] string value)
        {
            // Create a new role
            var role = new IdentityRole<Guid>
            {
                Name = value,
                NormalizedName = value.ToUpper()
            };
            _context.Roles.Add(role);
            _context.SaveChanges();
        }

        // PUT api/<RolesController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public void Put(string id, [FromBody] string value)
        {
            Guid guid = Guid.TryParse(id, out var parsedGuid) ? parsedGuid : Guid.Empty;
            // Update an existing role
            var role = _context.Roles.Find(guid);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }
            role.Name = value;
            role.NormalizedName = value.ToUpper();
            _context.Roles.Update(role);
            _context.SaveChanges();
        }

        // DELETE api/<RolesController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public void Delete(string id)
        {
            Guid guid = Guid.TryParse(id, out var parsedGuid) ? parsedGuid : Guid.Empty;
            // Delete a role
            var role = _context.Roles.Find(guid);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }
            _context.Roles.Remove(role);
            _context.SaveChanges();
        }
    }
}
