using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Logic.Utils;
using Logic.Dtos;
using System.Linq;
using Logic.Students;

namespace Logic.AppServices
{
    //immutable class (- a class whose public props cannot be changed by external code, once instantiated)
    public sealed class GetListQuery : IQuery<List<StudentDto>>
    {
        public string EnrolledIn { get; }
        public int? NumberOfCourses { get; }

        public GetListQuery(string enrolledIn, int? numberOfCourses)
        {
            EnrolledIn = enrolledIn;
            NumberOfCourses = numberOfCourses;
        }

        internal sealed class GetListQueryHandler : IQueryHandler<GetListQuery, List<StudentDto>>
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
    }


}
