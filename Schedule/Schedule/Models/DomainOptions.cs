namespace Schedule.Models
{
    public class DomainOptions
    {
        /// <summary> имя хоста </summary>
        public string HostName { get; set; }

        /// <summary> Адрес веба </summary>
        public string KpfuHost { get; set; }

        /// <summary> Учебный год </summary>
        public string Year { get; set; }

        /// <summary> Семестр </summary>
        public string Semester { get; set; }
    }
}