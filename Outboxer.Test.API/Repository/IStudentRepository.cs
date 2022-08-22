using Outboxer.Test.API.Entities;

namespace Outboxer.Test.API.Repository;

public interface IStudentRepository
{
    Task Add(Student student);
}