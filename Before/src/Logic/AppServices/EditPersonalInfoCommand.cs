using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Logic.Utils;
using Logic.Dtos;
using System.Linq;
using Logic.Decorators;
using Logic.Students;

namespace Logic.AppServices
{
    public interface ICommand {
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand {

        Result Handle(TCommand command);
    }

    public interface IQuery<TResult> {

    }

    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>{

       TResult Handle(TQuery query);
    }

    //immutable class (- a class whose public props cannot be changed by external code, once instantiated)
    public sealed class EditPersonalInfoCommand : ICommand
    {
        public long Id { get;}
        public string Name { get;}
        public string Email { get;}

        public EditPersonalInfoCommand(long id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }


        [AuditLog]
        [DatabaseRetry]
        internal sealed class EditPersonalInfoCommandHandler : ICommandHandler<EditPersonalInfoCommand>
        {

            private readonly SessionFactory _sessionFactory;

            public EditPersonalInfoCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory;
            }

            public Result Handle(EditPersonalInfoCommand command)
            {
                var unitOfWork = new UnitOfWork(_sessionFactory);

                var respository = new StudentRepository(unitOfWork);
                Student student = respository.GetById(command.Id);

                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                student.Name = command.Name;
                student.Email = command.Email;

                unitOfWork.Commit();

                return Result.Ok();

            }
        }

    }

    

    //immutable class (- a class whose public props cannot be changed by external code, once instantiated)

    
   
}
