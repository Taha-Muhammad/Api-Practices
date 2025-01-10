using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.ResourceParameters;

namespace CourseLibrary.API.Services.Repositories.Interfaces;
public interface ICourseRepository
{
	Task<bool> AuthorExistsAsync(Guid authorId);
	Task<IEnumerable<Course>> GetCoursesAsync(Guid authorId);
	Task<PagedList<Course>> GetCoursesAsync(Guid authorId, CoursesResourceParameters resourceParameters);
	Task<Course?> GetCourseAsync(Guid authorId, Guid courseId);
	void AddCourse(Guid authorId, Course course);
	void UpdateCourse(Course course);
	void DeleteCourse(Course course);
	Task<bool> SaveAsync();
}
