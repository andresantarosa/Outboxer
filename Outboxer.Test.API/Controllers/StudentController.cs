using Microsoft.AspNetCore.Mvc;
using Outboxer.Models;
using Outboxer.Outbox;
using Outboxer.Test.API.Entities;
using Outboxer.Test.API.Models;
using Outboxer.Test.API.Repository;
using Outboxer.Test.API.UnitOfWork;

namespace Outboxer.Test.API.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentRepository _studentRepository;
    private readonly IPublisher _publisher;

    public StudentController(IUnitOfWork unitOfWork, IStudentRepository studentRepository, IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _studentRepository = studentRepository;
        _publisher = publisher;
    }

    [HttpPost]
    public async Task AddStudent([FromBody] AddStudentRequest addStudentRequest)
    {
        Student student = new Student()
        {
            Id = Guid.NewGuid(),
            Name = addStudentRequest.Name
        };

       await _studentRepository.Add(student);
       await _publisher.Publish(new Entry("studentsQueue", student));
       await _unitOfWork.CommitAsync();
    }
    
    [HttpPost]
    [Route("sync")]
    public async Task AddStudentSync([FromBody] AddStudentRequest addStudentRequest)
    {
        Student student = new Student()
        {
            Id = Guid.NewGuid(),
            Name = addStudentRequest.Name
        };

        await _studentRepository.Add(student);
        await _publisher.Publish(new Entry("studentsQueue", student));
       _unitOfWork.Commit();
    }
}