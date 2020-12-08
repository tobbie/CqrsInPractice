using CSharpFunctionalExtensions;
using Logic.Students;
using Logic.Utils;

namespace Logic.AppServices
{
    public sealed class DisenrollCommand : ICommand
    {
        public long Id { get; }
        public int EnrollmentNumber { get; }
        public string Comment { get; }

        public DisenrollCommand(long id, int enrollmentNumber, string comment)
        {
            Id = id;
            EnrollmentNumber = enrollmentNumber;
            Comment = comment;
        }

        internal sealed class DisenrollCommandHandler : ICommandHandler<DisenrollCommand>
        {
            private readonly UnitOfWork _unitOfWork;
            public DisenrollCommandHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(DisenrollCommand command)
            {
                var studentRepository = new StudentRepository(_unitOfWork);
                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                if (string.IsNullOrWhiteSpace(command.Comment))
                    return Result.Fail("Disenrollment comment is required");

                Enrollment enrollment = student.GetEnrollment(command.EnrollmentNumber);
                if (enrollment == null)
                    return Result.Fail($"No enrollement found with number '{command.EnrollmentNumber}'");

                student.RemoveEnrollment(enrollment, command.Comment);

                _unitOfWork.Commit();

                return Result.Ok();
            }
        }
    }


}
