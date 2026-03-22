using StudentApp.Models;

namespace StudentApp.ViewModels
{
    public class GradeRowViewModel : ObservableObject
    {
        private readonly Student _student;
        private int? _grade;

        public int Semester { get; }
        public string Subject { get; }

        public int? Grade
        {
            get => _grade;
            set
            {
                if (_grade == value)
                {
                    return;
                }

                _grade = value;
                _student[Semester, Subject] = value;
                OnPropertyChanged();
                GradeChanged?.Invoke();
            }
        }

        public event System.Action? GradeChanged;

        public GradeRowViewModel(Student student, int semester, string subject, int? grade)
        {
            _student = student;
            Semester = semester;
            Subject = subject;
            _grade = grade;
        }
    }
}