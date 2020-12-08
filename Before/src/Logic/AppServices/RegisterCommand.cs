using System;
using CSharpFunctionalExtensions;
using Logic.Utils;
using Logic.Dtos;
using Logic.Decorators;
using Logic.Students;

namespace Logic.AppServices
{
    //immutable class (- a class whose public props cannot be changed by external code, once instantiated)

    public sealed class RegisterCommand : ICommand
    {
        public string Name { get; }
        public string Email { get; }
        public string Course1 { get; }
        public string Course1Grade { get; }
        public string Course2 { get; }
        public string Course2Grade { get; }

        public RegisterCommand(NewStudentDto dto)
        {
            Name = dto.Name;
            Email = dto.Email;
            Course1 = dto.Course1;
            Course1Grade = dto.Course1Grade;
            Course2 = dto.Course2;
            Course2Grade = dto.Course2Grade;
        }

        [AuditLog]
        internal sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand>
        {
            private readonly UnitOfWork _unitOfWork;
            public RegisterCommandHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(RegisterCommand command)
            {
                var student = new Student(command.Name, command.Email);
                var studentRepository = new StudentRepository(_unitOfWork);
                var courseRepository = new CourseRepository(_unitOfWork);

                if (command.Course1 != null && command.Course1Grade != null)
                {
                    Course course = courseRepository.GetByName(command.Course1);
                    student.Enroll(course, Enum.Parse<Grade>(command.Course1Grade));
                }

                if (command.Course2 != null && command.Course2Grade != null)
                {
                    Course course = courseRepository.GetByName(command.Course2);
                    student.Enroll(course, Enum.Parse<Grade>(command.Course2Grade));
                }

                studentRepository.Save(student);
                _unitOfWork.Commit();
                return Result.Ok();
            }
        }

    }


}
