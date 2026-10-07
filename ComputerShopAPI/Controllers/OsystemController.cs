using ComputerShopAPI.Models;
using ComputerShopAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace ComputerShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OsystemController : ControllerBase
    {
        public CmpShopDbContext context = new CmpShopDbContext();
        [HttpGet("getAll")]
        public object GetAllOsystem()
        { 
            var users = context.Osystems.ToList();
            return new {message = "Sikeres lekérdezés", result = users };
        }
        [HttpPost]
        public object AddNewOsystem(AddNewOsystemDto addNewOsystemDto)
        {
            var osystem = new Osystem
            {
                Id = Guid.NewGuid(),
                Name = addNewOsystemDto.Name,
                Version = addNewOsystemDto.Version,
                RegisterTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            context.Osystems.Add(osystem);
            context.SaveChanges();
            return StatusCode(201, new { message = "Sikeres hozzáadás", result = osystem });
        }

        [HttpPut]

        public object UpdateOsystem([FromQuery] Guid id, UpdateOsystemDto updateOsystemDto)
        {
            var osystem = context.Osystems.FirstOrDefault(osystem => osystem.Id == id);
            if (osystem != null)
            {
                osystem.Name = updateOsystemDto.Name;
                osystem.Version = updateOsystemDto.Version;
                osystem.UpdateTime = DateTime.Now;
                context.Osystems.Update(osystem);
                context.SaveChanges();
                return StatusCode(200, new { message = "Sikeres módosítás", result = osystem });
            }
            return StatusCode(404, new { message = "Sikertelen módosítás" });
        }

        [HttpDelete]
        public object DeleteOsystem([FromQuery] Guid id)
        {
            var osystem = context.Osystems.Find(id);
            if (osystem != null)
            {
                context.Osystems.Remove(osystem);
                context.SaveChanges();
                return StatusCode(204, new { message = "Sikeres törlés" });
            }
            return StatusCode(404, new { message = "Sikertelen törlés" });
        }
    }
}
