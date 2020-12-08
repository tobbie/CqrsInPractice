using System;
using CSharpFunctionalExtensions;
using Logic.Utils;
using Logic.Dtos;
using Logic.Students;

namespace Logic.AppServices
{
    public sealed class TransferCommand : ICommand
    {
        public long Id { get; }
        public int EnrollmentNumber { get; }
        public string Course { get; }
        public string Grade { get; }

        public TransferCommand(long id, int enrollmentNumber, StudentTransferDto dto)
        {
            Id = id;
            EnrollmentNumber = enrollmentNumber;
            Course = dto.Course;
            Grade = dto.Grade;
        }

        internal sealed class TransferCommandHandler : ICommandHandler<TransferCommand>
        {
            private readonly UnitOfWork _unitOfWork;
            public TransferCommandHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(TransferCommand command)
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

                Enrollment enrollment = student.GetEnrollment(command.EnrollmentNumber);
                if (enrollment == null)
                    return Result.Fail($"No enrollement found with number '{command.EnrollmentNumber}'");

                enrollment.Update(course, grade);

                _unitOfWork.Commit();

                return Result.Ok();
            }
        }
    }


}
