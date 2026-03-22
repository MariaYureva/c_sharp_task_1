using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentApp.Models
{
    public class Student
    {
        private readonly Dictionary<int, Dictionary<string, int?>> _grades;

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Course { get; set; }
        public string Group { get; set; }

        /// <summary>
        /// Список предметов по семестрам.
        /// Ключ - номер семестра, значение - список предметов.
        /// Передаётся в конструктор извне.
        /// </summary>
        public Dictionary<int, List<string>> SubjectsBySemester { get; }

        public Student(
            string lastName,
            string firstName,
            DateTime birthDate,
            int course,
            string group,
            Dictionary<int, List<string>> subjectsBySemester)
        {
            if (subjectsBySemester == null)
            {
                throw new ArgumentNullException(nameof(subjectsBySemester));
            }

            LastName = lastName;
            FirstName = firstName;
            BirthDate = birthDate;
            Course = course;
            Group = group;

            SubjectsBySemester = new Dictionary<int, List<string>>();
            _grades = new Dictionary<int, Dictionary<string, int?>>();

            foreach (KeyValuePair<int, List<string>> pair in subjectsBySemester)
            {
                SubjectsBySemester[pair.Key] = new List<string>(pair.Value);
                _grades[pair.Key] = new Dictionary<string, int?>();

                foreach (string subject in pair.Value)
                {
                    _grades[pair.Key][subject] = null;
                }
            }
        }

        /// <summary>
        /// Индексатор: первый индекс - номер семестра,
        /// второй - название предмета.
        /// </summary>
        public int? this[int semester, string subject]
        {
            get
            {
                ValidateSemesterAndSubject(semester, subject);
                return _grades[semester][subject];
            }
            set
            {
                ValidateSemesterAndSubject(semester, subject);

                if (value is < 2 or > 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Оценка должна быть от 2 до 5.");
                }

                _grades[semester][subject] = value;
            }
        }

        public double CalculateAverageGrade()
        {
            List<int> existingGrades = _grades
                .SelectMany(semester => semester.Value.Values)
                .Where(grade => grade.HasValue)
                .Select(grade => grade!.Value)
                .ToList();

            if (existingGrades.Count == 0)
            {
                return 0;
            }

            return existingGrades.Average();
        }

        public double CalculateAverageGradeBySubject(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("Название предмета не должно быть пустым.", nameof(subject));
            }

            List<int> subjectGrades = _grades
                .Where(semester => semester.Value.ContainsKey(subject))
                .Select(semester => semester.Value[subject])
                .Where(grade => grade.HasValue)
                .Select(grade => grade!.Value)
                .ToList();

            if (subjectGrades.Count == 0)
            {
                return 0;
            }

            return subjectGrades.Average();
        }

        public List<string> GetDebts()
        {
            List<string> debts = new();

            foreach (KeyValuePair<int, Dictionary<string, int?>> semester in _grades)
            {
                foreach (KeyValuePair<string, int?> subject in semester.Value)
                {
                    // Считаем задолженностью отсутствие оценки или оценку 2
                    if (!subject.Value.HasValue || subject.Value.Value < 3)
                    {
                        debts.Add($"Семестр {semester.Key}: {subject.Key}");
                    }
                }
            }

            return debts;
        }

        public List<string> GetSubjects()
        {
            return SubjectsBySemester
                .OrderBy(pair => pair.Key)
                .SelectMany(pair => pair.Value)
                .Distinct()
                .ToList();
        }

        private void ValidateSemesterAndSubject(int semester, string subject)
        {
            if (!_grades.ContainsKey(semester))
            {
                throw new ArgumentException($"Семестр {semester} не найден.");
            }

            if (!_grades[semester].ContainsKey(subject))
            {
                throw new ArgumentException($"Предмет \"{subject}\" не найден в семестре {semester}.");
            }
        }
    }
}