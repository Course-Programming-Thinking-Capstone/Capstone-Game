using System;

namespace Services.Request
{
    public class StudentFinishQuizGameRequest
    {
        public int UserID { get; set; }
        public string QuizCode { get; set; }
        private DateTime StartTimeQuiz { get; set; }
        private DateTime EndTimeQuiz { get; set; }
        private int StepCount { get; set; }
    }
}