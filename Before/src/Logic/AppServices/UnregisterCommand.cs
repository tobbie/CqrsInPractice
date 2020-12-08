using CSharpFunctionalExtensions;
using Logic.Students;
using Logic.Utils;

namespace Logic.AppServices
{ 
    public sealed class UnregisterCommand : ICommand
    {
        public long Id { get; }

        public UnregisterCommand(long id)
        {
            Id = id;
        }

        internal sealed class UnregisterCommandHandler : ICommandHandler<UnregisterCommand>
        {
            private readonly UnitOfWork _unitOfWork;
            public UnregisterCommandHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public Result Handle(UnregisterCommand command)
            {
                var studentRepository = new StudentRepository(_unitOfWork);
                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                studentRepository.Delete(student);
                _unitOfWork.Commit();

                return Result.Ok();
            }
        }
    }


}
