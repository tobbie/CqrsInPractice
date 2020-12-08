using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Dtos;
using CSharpFunctionalExtensions;
using Logic.AppServices;
using Logic.Students;
using Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/students")]
    public sealed class StudentController : BaseController
    { 
        private readonly Messages _mesages;

        public StudentController(Messages messages)
        {       
            _mesages = messages;
        }

        [HttpGet]
        public IActionResult GetList(string enrolled, int? number)
        {
            var query = new GetListQuery(enrolled, number);
            var resultList = _mesages.Dispatch(query);
            return Ok(resultList);
        }

       
        [HttpPost]
        public IActionResult Register([FromBody]NewStudentDto dto)
        {
            var command = new RegisterCommand(dto);
            Result result = _mesages.Dispatch(command);
            return FromResult(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Unregister(long id)
        {
            Result result = _mesages.Dispatch(new UnregisterCommand(id));
            return FromResult(result);
        }


        [HttpPost("{id}/enrollments")]
        public IActionResult Enroll(long id, [FromBody] StudentEnrollmentDto dto) {

            Result result = _mesages.Dispatch(new EnrollCommand(id, dto.Course, dto.Grade));
         return FromResult(result);
        }

        [HttpPut("{id}/enrollments/{enrollmentNumber}")]
        public IActionResult Transfer(long id,int enrollmentNumber,  [FromBody] StudentTransferDto dto)
        {
            Result result = _mesages.Dispatch(new TransferCommand(id, enrollmentNumber, dto));
            return FromResult(result);

        }

        [HttpPost("{id}/enrollments/{enrollmentNumber}/deletion")]
        public IActionResult Disenroll(long id, int enrollmentNumber, [FromBody]StudentDisenrollmentDto dto) {

            Result result = _mesages.Dispatch(new DisenrollCommand(id, enrollmentNumber, dto.Comment));
            return FromResult(result);

        }

        [HttpPut("{id}")]
        public IActionResult EditPersonalInfo(long id, [FromBody] StudentPersonalInfoDto dto) {

            var command = new EditPersonalInfoCommand(id, dto.Name, dto.Email);
            Result result = _mesages.Dispatch(command);

            return FromResult(result);
        }     
    }
}
