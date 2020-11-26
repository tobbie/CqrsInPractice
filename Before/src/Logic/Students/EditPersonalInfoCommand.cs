﻿using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Logic.Utils;
using Logic.Dtos;
using System.Linq;

namespace Logic.Students
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
    public sealed class GetListQuery : IQuery<List<StudentDto>> {
        public string EnrolledIn { get; }
        public int? NumberOfCourses { get; }

        public GetListQuery(string enrolledIn, int? numberOfCourses) {
            EnrolledIn = enrolledIn;
            NumberOfCourses = numberOfCourses;
        }
    }

    public sealed class GetListQueryHandler : IQueryHandler<GetListQuery, List<StudentDto>>
    {
        private readonly UnitOfWork _unitOfWork;

        public GetListQueryHandler(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<StudentDto> Handle(GetListQuery query)
        {
            return new StudentRepository(_unitOfWork)
                                 .GetList(query.EnrolledIn, query.NumberOfCourses)
                                 .Select(x => ConvertToDto(x)).ToList();  
        }

        private StudentDto ConvertToDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Course1 = student.FirstEnrollment?.Course?.Name,
                Course1Grade = student.FirstEnrollment?.Grade.ToString(),
                Course1Credits = student.FirstEnrollment?.Course?.Credits,
                Course2 = student.SecondEnrollment?.Course?.Name,
                Course2Grade = student.SecondEnrollment?.Grade.ToString(),
                Course2Credits = student.SecondEnrollment?.Course?.Credits,
            };
        }
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
        
    }

    public sealed class EditPersonalInfoCommandHandler :ICommandHandler<EditPersonalInfoCommand> {

        private readonly SessionFactory _sessionFactory;

        public EditPersonalInfoCommandHandler(SessionFactory sessionFactory)
        {  
            _sessionFactory = sessionFactory;
        }

        public Result Handle(EditPersonalInfoCommand command) {
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

    //immutable class (- a class whose public props cannot be changed by external code, once instantiated)
    public sealed class RegisterCommand : ICommand
    {
        public string Name {get;}
        public string Email {get;}
        public string Course1 { get;}
        public string Course1Grade { get;}
        public string Course2 {get;}
        public string Course2Grade { get;}

        public RegisterCommand(NewStudentDto dto)
        {
            Name = dto.Name;
            Email = dto.Email;
            Course1 = dto.Course1;
            Course1Grade = dto.Course1Grade;
            Course2 = dto.Course2;
            Course2Grade = dto.Course2Grade;
        }

    }

    public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand>
    {
        private readonly UnitOfWork _unitOfWork;
        public RegisterCommandHandler(UnitOfWork unitOfWork) {
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

    public sealed class UnregisterCommand : ICommand {
        public long Id { get; }

        public UnregisterCommand(long id) {
            Id = id;
        }
    }

    public sealed class UnregisterCommandHandler : ICommandHandler<UnregisterCommand>
    {
        private readonly UnitOfWork _unitOfWork;
        public UnregisterCommandHandler(UnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public Result Handle(UnregisterCommand command)
        {
            var studentRepository =  new StudentRepository(_unitOfWork);
            Student student = studentRepository.GetById(command.Id);
            if (student == null)
                return Result.Fail($"No student found for Id {command.Id}");

            studentRepository.Delete(student);
            _unitOfWork.Commit();

            return Result.Ok();
        }
    }

    public sealed class EnrollCommand : ICommand {
        public long Id { get; }
        public string Course { get;}
        public string Grade { get; }

        public EnrollCommand(long id, string course, string grade) {
            Course = course;
            Grade = grade;
            Id = id;
        }

    }

    public sealed class EnrollCommandHandler : ICommandHandler<EnrollCommand>
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

    public sealed class TransferCommand : ICommand {
        public long Id {get; }
        public int EnrollmentNumber { get; }
        public string Course { get;}
        public string Grade { get;}

        public TransferCommand(long id, int enrollmentNumber, StudentTransferDto dto) {
            Id = id;
            EnrollmentNumber = enrollmentNumber;
            Course = dto.Course;
            Grade = dto.Grade;
        }
    }

    public sealed class TransferCommandHandler : ICommandHandler<TransferCommand>
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

    public sealed class DisenrollCommand : ICommand {
        public long Id { get; }
        public int EnrollmentNumber { get; }
        public string Comment { get; }

        public DisenrollCommand(long id, int enrollmentNumber, string comment) {
            Id = id;
            EnrollmentNumber = enrollmentNumber;
            Comment = comment;
        }
    }

    public sealed class DisenrollCommandHandler : ICommandHandler<DisenrollCommand>
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
