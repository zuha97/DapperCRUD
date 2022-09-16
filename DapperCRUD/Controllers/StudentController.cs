using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace DapperCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Student>>> GetAllStudents()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            IEnumerable<Student> students = await SelectAllStudent(connection);
            return Ok(students);
        }

        [HttpGet("{studentId}")]
        public async Task<ActionResult<Student>> GetStudent(int studentId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var student = await connection.QueryFirstAsync<Student>("select * from Student where id= @Id", new {Id= studentId });
            return Ok(student);
        }

        [HttpPost]
        public async Task<ActionResult<List<Student>>> CreateStudent(Student student)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into Student (name, firstname, lastname, place) values (@Name, @FirstName, @LastName, @Place)", student);
            return Ok(await SelectAllStudent(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<Student>>> UpdateStudent(Student student)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update Student set name = @Name, firstname = @FirstName, lastname = @LastName, place = @Place where id = @Id", student);
            return Ok(await SelectAllStudent(connection));
        }

        [HttpDelete("{studentId}")]
        public async Task<ActionResult<List<Student>>> DeleteStudent(int studentId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from Student where id = @Id", new {Id = studentId });
            return Ok(await SelectAllStudent(connection));
        }

        private static async Task<IEnumerable<Student>> SelectAllStudent(SqlConnection connection)
        {
            return await connection.QueryAsync<Student>("select * from Student");
        }
    }
}
