SELECT s.StudentID, s.Name, s.Email, c.Name as CourseName, c.Credits as CourseCredits, e.Grade
 from Student s, Course c INNER JOIN
Enrollment e  on e.CourseID = c.CourseID
ORDER by s.Name