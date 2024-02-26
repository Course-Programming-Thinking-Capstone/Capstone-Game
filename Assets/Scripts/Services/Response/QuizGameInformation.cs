using System;

namespace Services.Response
{
    public class QuizGameInformation
    {
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string TeacherName { get; set; }
    }
}