﻿using System;
using CSharpFunctionalExtensions;
using Logic.Utils;

namespace Logic.Students
{
    public interface ICommand {
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand {

        Result Handle(TCommand command);
    }

    public sealed class EditPersonalInfoCommand : ICommand
    {
        public EditPersonalInfoCommand()
        {
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public sealed class EditPersonalInfoCommandHandler :ICommandHandler<EditPersonalInfoCommand> {

        private readonly UnitOfWork _unitOfWork;

        public EditPersonalInfoCommandHandler(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Result Handle(EditPersonalInfoCommand command) {

            var respository = new StudentRepository(_unitOfWork);
            Student student = respository.GetById(command.Id);

            if (student == null)
                return Result.Fail($"No student found for Id {command.Id}");

            student.Name = command.Name;
            student.Email = command.Email;

            _unitOfWork.Commit();

            return Result.Ok();
            
        }
    }
}
