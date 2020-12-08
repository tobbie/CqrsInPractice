using System;
using CSharpFunctionalExtensions;
using Logic.Utils;
using Logic.Students;

namespace Logic.AppServices
{
    public sealed class EnrollCommand : ICommand
    {
        public long Id { get; }
        public string Course { get; }
        public string Grade { get; }

        public EnrollCommand(long id, string course, string grade)
        {
            Course = course;
            Grade = grade;
            Id = id;
        }

        internal sealed class EnrollCommandHandler : ICommandHandler<EnrollCommand>
        {
            private readonly UnitOfWork _unitOfWork;
            public EnrollCommandHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(EnrollCommand command)
            {
                var studentRepository = new StudentRepository(_unitOfWork);
                var courseRepository = new CourseRepository(_unitOfWork);

                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                Course course = courseRepository.GetByName(command.Course);
                if (course == null)
                    return Result.Fail($"Course is incorrect: '{command.Course}'");

                bool success = Enum.TryParse(command.Grade, out Grade grade);
                if (!success)
                    return Result.Fail($"Grade is incorrect: '{command.Grade}'");

                student.Enroll(course, grade);

                _unitOfWork.Commit();

                return Result.Ok();
            }
        }

    }


}
