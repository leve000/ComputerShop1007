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

        [HttpGet("~/api/query/os")]
        public object GetOsystemsWithComputers()
        {
            var osystems = context.Osystems
                .Select(os => new
                {
                    os.Id,
                    os.Name,
                    os.Version,
                    os.RegisterTime,
                    os.UpdateTime,
                    Computers = context.Computers
                        .Where(computer => computer.Osys.Id == os.Id)
                        .Select(computer => new
                        {
                            computer.Id,
                            computer.Brand,
                            computer.Type,
                            computer.Display,
                            computer.Memory,
                            computer.CreatedTime
                        }).ToList()
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = osystems };
        }

        [HttpGet("~/api/query/computers")]
        public object GetComputersWithOsystems()
        {
            var computers = context.Computers
                .Include(computer => computer.Osys)
                .Select(computer => new
                {
                    computer.Id,
                    computer.Brand,
                    computer.Type,
                    computer.Display,
                    computer.Memory,
                    computer.CreatedTime,
                    Osystem = new
                    {
                        computer.Osys.Id,
                        computer.Osys.Name,
                        computer.Osys.Version
                    }
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = computers };
        }

        [HttpGet("~/api/query/os/{id:guid}")]
        public ActionResult<object> GetOsystemWithComputers(Guid id)
        {
            var osystem = context.Osystems
                .Where(os => os.Id == id)
                .Select(os => new
                {
                    os.Id,
                    os.Name,
                    os.Version,
                    os.RegisterTime,
                    os.UpdateTime,
                    Computers = context.Computers
                        .Where(computer => computer.Osys.Id == os.Id)
                        .Select(computer => new
                        {
                            computer.Id,
                            computer.Brand,
                            computer.Type,
                            computer.Display,
                            computer.Memory,
                            computer.CreatedTime
                        }).ToList()
                })
                .FirstOrDefault();

            if (osystem == null)
                return NotFound(new { message = "Osystem nem található" });

            return Ok(new { message = "Sikeres lekérdezés", result = osystem });
        }

        [HttpGet("~/api/query/computer/{id:guid}")]
        public ActionResult<object> GetComputerWithOsystem(Guid id)
        {
            var computer = context.Computers
                .Where(computer => computer.Id == id)
                .Select(computer => new
                {
                    computer.Id,
                    computer.Brand,
                    computer.Type,
                    computer.Display,
                    computer.Memory,
                    computer.CreatedTime,
                    Osystem = new
                    {
                        computer.Osys.Id,
                        computer.Osys.Name,
                        computer.Osys.Version
                    }
                })
                .FirstOrDefault();

            if (computer == null)
                return NotFound(new { message = "Computer nem található" });

            return Ok(new { message = "Sikeres lekérdezés", result = computer });
        }

        [HttpGet("~/api/query/os-computer-count")]
        public object GetOsystemComputerCounts()
        {
            var osystems = context.Osystems
                .Select(os => new
                {
                    os.Id,
                    os.Name,
                    os.Version,
                    ComputerCount = context.Computers.Count(computer => computer.Osys.Id == os.Id)
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = osystems };
        }

        [HttpGet("~/api/query/brand/{brand}")]
        public object GetComputersByBrand(string brand)
        {
            var computers = context.Computers
                .Where(computer => computer.Brand == brand)
                .Select(computer => new
                {
                    computer.Id,
                    computer.Brand,
                    computer.Type,
                    computer.Display,
                    computer.Memory,
                    computer.CreatedTime,
                    Osystem = new
                    {
                        computer.Osys.Id,
                        computer.Osys.Name,
                        computer.Osys.Version
                    }
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = computers };
        }

        [HttpGet("~/api/query/computers-sorted")]
        public object GetComputersSorted()
        {
            var computers = context.Computers
                .OrderBy(computer => computer.Brand)
                .ThenBy(computer => computer.Type)
                .Select(computer => new
                {
                    computer.Id,
                    computer.Brand,
                    computer.Type,
                    computer.Display,
                    computer.Memory,
                    computer.CreatedTime,
                    Osystem = new
                    {
                        computer.Osys.Id,
                        computer.Osys.Name,
                        computer.Osys.Version
                    }
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = computers };
        }

        [HttpGet("~/api/query/min-display/{size}")]
        public object GetComputersWithMinimumDisplay(double size)
        {
            var computers = context.Computers
                .Where(computer => computer.Display > size)
                .Select(computer => new
                {
                    computer.Id,
                    computer.Brand,
                    computer.Type,
                    computer.Display,
                    computer.Memory,
                    computer.CreatedTime,
                    Osystem = new
                    {
                        computer.Osys.Id,
                        computer.Osys.Name,
                        computer.Osys.Version
                    }
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = computers };
        }

        [HttpGet("~/api/query/os-stats")]
        public object GetOsystemStatistics()
        {
            var statistics = context.Osystems
                .Select(os => new
                {
                    os.Id,
                    os.Name,
                    os.Version,
                    ComputerCount = context.Computers.Count(computer => computer.Osys.Id == os.Id),
                    AverageMemory = context.Computers
                        .Where(computer => computer.Osys.Id == os.Id)
                        .Select(computer => (double?)computer.Memory)
                        .Average(),
                    AverageDisplay = context.Computers
                        .Where(computer => computer.Osys.Id == os.Id)
                        .Select(computer => (double?)computer.Display)
                        .Average()
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = statistics };
        }

        [HttpGet("~/api/query/by-os")]
        public object GetComputersGroupedByOsystem()
        {
            var computersByOsystem = context.Osystems
                .Select(os => new
                {
                    Osystem = os.Name,
                    Computers = context.Computers
                        .Where(computer => computer.Osys.Id == os.Id)
                        .Select(computer => new
                        {
                            computer.Id,
                            computer.Brand,
                            computer.Type,
                            computer.Display,
                            computer.Memory,
                            computer.CreatedTime
                        })
                        .ToList()
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = computersByOsystem };
        }

        [HttpGet("~/api/query/os-without-computers")]
        public object GetOsystemsWithoutComputers()
        {
            var osystems = context.Osystems
                .Where(os => !context.Computers.Any(computer => computer.Osys.Id == os.Id))
                .Select(os => new
                {
                    os.Id,
                    os.Name,
                    os.Version,
                    os.RegisterTime,
                    os.UpdateTime
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = osystems };
        }

        [HttpGet("~/api/query/max-memory-by-os")]
        public object GetHighestMemoryComputerByOsystem()
        {
            var computers = context.Osystems
                .Where(os => context.Computers.Any(computer => computer.Osys.Id == os.Id))
                .Select(os => context.Computers
                    .Where(computer => computer.Osys.Id == os.Id)
                    .OrderByDescending(computer => computer.Memory)
                    .Select(computer => new
                    {
                        Osystem = os.Name,
                        computer.Id,
                        computer.Brand,
                        computer.Type,
                        computer.Memory,
                        computer.Display
                    })
                    .First())
                .ToList();

            return new { message = "Sikeres lekérdezés", result = computers };
        }

        [HttpGet("~/api/query/filters")]
        public object GetComputersByMemoryAndOsVersion(
            [FromQuery] int minMemory,
            [FromQuery] int minOsVersion)
        {
            var computers = context.Computers
                .Where(computer => computer.Memory >= minMemory && computer.Osys.Version >= minOsVersion)
                .Select(computer => new
                {
                    computer.Id,
                    computer.Brand,
                    computer.Type,
                    computer.Display,
                    computer.Memory,
                    computer.CreatedTime,
                    Osystem = new
                    {
                        computer.Osys.Id,
                        computer.Osys.Name,
                        computer.Osys.Version
                    }
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = computers };
        }

        [HttpGet("~/api/query/latest")]
        public object GetLatestComputers()
        {
            var computers = context.Computers
                .OrderByDescending(computer => computer.CreatedTime)
                .Select(computer => new
                {
                    computer.Id,
                    computer.Brand,
                    computer.Type,
                    computer.Display,
                    computer.Memory,
                    computer.CreatedTime,
                    Osystem = new
                    {
                        computer.Osys.Id,
                        computer.Osys.Name,
                        computer.Osys.Version
                    }
                })
                .ToList();

            return new { message = "Sikeres lekérdezés", result = computers };
        }

        [HttpGet("~/api/query/brand-summary")]
        public object GetBrandSummary()
        {
            var summary = context.Computers
                .GroupBy(computer => computer.Brand)
                .Select(group => new
                {
                    Brand = group.Key,
                    ComputerCount = group.Count()
                })
                .OrderBy(summary => summary.Brand)
                .ToList();

            return new { message = "Sikeres lekérdezés", result = summary };
        }
    }
}
