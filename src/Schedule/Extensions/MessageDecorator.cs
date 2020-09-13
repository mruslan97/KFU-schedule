using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CodeJam.Strings;
using Schedule.Entities;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace Schedule.Extensions
{
    public static class MessageDecorator
    {
        public static string ToMessage(this IEnumerable<Subject> subjects, DayOfWeek day, bool showGroup = false)
        {
            if (!subjects.Any()) return "Нет данных/Выходной день";
            var stringBuilder = new StringBuilder();
            var subjectNumber = 1;
            var culture = new CultureInfo("ru-Ru");
            var dayOfWeek = culture.DateTimeFormat.GetDayName(day);
            stringBuilder.AppendLine($"{dayOfWeek.ToUpper()}");
            stringBuilder.AppendLine();
            foreach (var subject in subjects.OrderBy(x => x.StartTime))
            {
                stringBuilder.AppendLine($"{subjectNumber}. {subject.Name}");
                stringBuilder.AppendLine($"🕑 {subject.TotalTime}");
                stringBuilder.AppendLine(
                    $"👤 {subject.TeacherLastname} {subject.TeacherFirstname} {subject.TeacherMiddlename}");
                stringBuilder.AppendLine($"🏢 Аудитория №{subject.CabinetNumber} {subject.BuildingName}");
                if (subject.StartDay.HasValue && subject.StartDay.Value.Year > 1
                                              && subject.EndDay.HasValue && subject.EndDay.Value.Year > 1)
                    stringBuilder.AppendLine(
                        $"📅 Период с {subject.StartDay?.ToShortDateString()} по {subject.EndDay?.ToShortDateString()}");

                if (!subject.Note.IsNullOrWhiteSpace()) stringBuilder.AppendLine($"💬 {subject.Note}");
                if (!subject.SubjectKindName.IsNullOrWhiteSpace())
                {
                    stringBuilder.AppendLine($"✅ {subject.SubjectKindName}");
                }

                if (showGroup)
                {
                    stringBuilder.AppendLine($"🎓 Группа: {subject.GroupName}");
                }

                stringBuilder.AppendLine();
                subjectNumber++;
            }

            stringBuilder.AppendLine();
            
            return stringBuilder.ToString();
        }

        public static MessageKeyboard BuildKeyboard(this IEnumerable<Teacher> teachers)
        {
            var keyboardBuilder = new MessageKeyboardBuilder();
            // TODO решить проблему с ограничением числа сотрудников в одной клавиатуре
            var teachersList = teachers.Take(29).ToList();
            if (teachersList.Count > 1)
            {
                var index = 0;
                foreach (var teacher in teachersList)
                {
                    keyboardBuilder.AddButton($"{teacher.GetFullName()}", $"{teacher.KpfuId}",
                        KeyboardButtonColor.Default, "teacherId");
                    index++;
                    if (index % 3 == 0)
                    {
                        keyboardBuilder.Line();
                    }
                }
            }
            else
            {
                keyboardBuilder.AddButton($"{teachersList[0].GetFullName()}", $"{teachersList[0].KpfuId}",
                    KeyboardButtonColor.Default, "teacherId");
            }
            keyboardBuilder.AddButton("Назад 🔙", "", KeyboardButtonColor.Positive, "");
            keyboardBuilder.SetOneTime();

            return keyboardBuilder.Get();
        }

        public static MessageKeyboard BuildMainMenu()
        {
            var keyboardBuilder = new MessageKeyboardBuilder();
            keyboardBuilder.AddButton("На сегодня", "", KeyboardButtonColor.Primary, "");
            keyboardBuilder.AddButton("На завтра", "", KeyboardButtonColor.Primary, "");
            keyboardBuilder.Line();
            keyboardBuilder.AddButton("На неделю", "", KeyboardButtonColor.Primary, "");
            keyboardBuilder.Line();
            keyboardBuilder.AddButton("Поиск преподавателя 🔎", "", KeyboardButtonColor.Positive, "");
            keyboardBuilder.Line();
            keyboardBuilder.AddButton("Настройки 🛠", "", KeyboardButtonColor.Positive, "");

            return keyboardBuilder.Get();
        }
        
        public static MessageKeyboard BuildSettingsMenu()
        {
            var keyboardBuilder = new MessageKeyboardBuilder();
            keyboardBuilder.AddButton("Текст 🔡", "", KeyboardButtonColor.Primary, "set_button_text");
            keyboardBuilder.AddButton("Картинка 🏞", "", KeyboardButtonColor.Primary, "set_button_picture");
            keyboardBuilder.Line();
            keyboardBuilder.AddButton("Назад 🔙", "", KeyboardButtonColor.Default, "");

            return keyboardBuilder.Get();
        }

        public static MessageKeyboard ReturnMenu()
        {
            var keyboardBuilder = new MessageKeyboardBuilder();
            keyboardBuilder.AddButton("Назад 🔙", "", KeyboardButtonColor.Default, "");

            return keyboardBuilder.Get();
        }

        public static string GetTeacher(this Subject subject)
        {
            if(!subject.TeacherFirstname.IsNullOrWhiteSpace() && !subject.TeacherMiddlename.IsNullOrWhiteSpace())
                return $"{subject.TeacherLastname} {subject.TeacherFirstname[0]}. {subject.TeacherMiddlename[0]}.";
            return subject.TeacherLastname;
        }

        public static string GetFullName(this Teacher teacher)
        {
             return $"{teacher.Lastname} {teacher.Firstname[0]}. {teacher.Middlename[0]}.";
        }
    }
}