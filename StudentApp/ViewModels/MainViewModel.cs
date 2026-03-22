using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StudentApp.Models;

namespace StudentApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly Student _student;
        private string _selectedSubject = string.Empty;

        public string LastName => _student.LastName;
        public string FirstName => _student.FirstName;
        public DateTime BirthDate => _student.BirthDate;
        public int Course => _student.Course;
        public string Group => _student.Group;

        public ObservableCollection<GradeRowViewModel> GradeRows { get; }
        public ObservableCollection<string> Subjects { get; }
        public ObservableCollection<string> Debts { get; }

        public string SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (_selectedSubject == value)
                {
                    return;
                }

                _selectedSubject = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AverageBySelectedSubjectText));
            }
        }

        public string AverageAllText => _student.CalculateAverageGrade().ToString("F2");

        public string AverageBySelectedSubjectText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SelectedSubject))
                {
                    return "0.00";
                }

                return _student.CalculateAverageGradeBySubject(SelectedSubject).ToString("F2");
            }
        }

        public MainViewModel()
        {
            Dictionary<int, List<string>> subjectsBySemester = new()
            {
                { 1, new List<string> { "Математика", "Информатика", "Физика" } },
                { 2, new List<string> { "Математика", "Программирование", "Английский язык" } }
            };

            _student = new Student(
                lastName: "Иванов",
                firstName: "Иван",
                birthDate: new DateTime(2004, 5, 14),
                course: 2,
                group: "ПИ-21",
                subjectsBySemester: subjectsBySemester);

            // Инициализация значений ВНЕ класса Student
            _student[1, "Математика"] = 5;
            _student[1, "Информатика"] = 4;
            _student[1, "Физика"] = 2;
            _student[2, "Математика"] = 5;
            _student[2, "Программирование"] = 5;
            _student[2, "Английский язык"] = 3;

            GradeRows = new ObservableCollection<GradeRowViewModel>();
            Subjects = new ObservableCollection<string>(_student.GetSubjects());
            Debts = new ObservableCollection<string>(_student.GetDebts());

            foreach (KeyValuePair<int, List<string>> semester in _student.SubjectsBySemester.OrderBy(x => x.Key))
            {
                foreach (string subject in semester.Value)
                {
                    GradeRowViewModel row = new(
                        _student,
                        semester.Key,
                        subject,
                        _student[semester.Key, subject]);

                    row.GradeChanged += UpdateCalculatedData;
                    GradeRows.Add(row);
                }
            }

            if (Subjects.Count > 0)
            {
                SelectedSubject = Subjects[0];
            }
        }

        private void UpdateCalculatedData()
        {
            Debts.Clear();

            foreach (string debt in _student.GetDebts())
            {
                Debts.Add(debt);
            }

            OnPropertyChanged(nameof(AverageAllText));
            OnPropertyChanged(nameof(AverageBySelectedSubjectText));
        }
    }
}