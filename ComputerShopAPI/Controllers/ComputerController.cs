using ComputerShopAPI.Models;
using ComputerShopAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComputerController : ControllerBase
    {
        public CmpShopDbContext context = new CmpShopDbContext();

        [HttpGet("getAll")]
        public object GetAllComputer()
        {
            var computers = context.Computers.Include(c => c.Osys).ToList();
            return new { message = "Sikeres lekérdezés", result = computers };
        }

        [HttpPost]
        public object AddNewComputer(AddNewComputerDto addNewComputerDto)
        {
            var osys = context.Osystems.Find(addNewComputerDto.OsysId);
            if (osys == null)
                return StatusCode(404, new { message = "Osystem nem található" });

            var computer = new Computer
            {
                Id = Guid.NewGuid(),
                Brand = addNewComputerDto.Brand,
                Type = addNewComputerDto.Type,
                Display = addNewComputerDto.Display,
                Memory = addNewComputerDto.Memory,
                CreatedTime = DateTime.Now,
                Osys = osys
            };
            context.Computers.Add(computer);
            context.SaveChanges();
            return StatusCode(201, new { message = "Sikeres hozzáadás", result = computer });
        }

        [HttpPut]
        public object UpdateComputer([FromQuery] Guid id, UpdateComputerDto updateComputerDto)
        {
            var computer = context.Computers.Include(c => c.Osys).FirstOrDefault(c => c.Id == id);
            if (computer != null)
            {
                computer.Brand = updateComputerDto.Brand;
                computer.Type = updateComputerDto.Type;
                computer.Display = updateComputerDto.Display;
                computer.Memory = updateComputerDto.Memory;
                if (updateComputerDto.OsysId != Guid.Empty)
                {
                    var osys = context.Osystems.Find(updateComputerDto.OsysId);
                    if (osys == null)
                        return StatusCode(404, new { message = "Osystem nem található" });
                    computer.Osys = osys;
                }
                context.Computers.Update(computer);
                context.SaveChanges();
                return StatusCode(200, new { message = "Sikeres módosítás", result = computer });
            }
            return StatusCode(404, new { message = "Sikertelen módosítás" });
        }

        [HttpDelete]
        public object DeleteComputer([FromQuery] Guid id)
        {
            var computer = context.Computers.Find(id);
            if (computer != null)
            {
                context.Computers.Remove(computer);
                context.SaveChanges();
                return StatusCode(204, new { message = "Sikeres törlés" });
            }
            return StatusCode(404, new { message = "Sikertelen törlés" });
        }
    }
}
