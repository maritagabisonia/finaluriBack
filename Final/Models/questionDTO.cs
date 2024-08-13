namespace Final.Models
{
    public class questionDTO
    {
        public int? ID { get; set; }
        public string? Question { get; set; }
        public Boolean? Important { get; set; }
        public int? NumberOfAnswers { get; set; }
        public List<answerDTO>? Answers { get; set; }


    }

    public class answerDTO
    {
        public int? ID { get; set; }
        public string? Answer { get; set; }
        public Boolean? Correct { get; set; }

    }
    

}
