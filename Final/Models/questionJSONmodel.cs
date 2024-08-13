namespace Final.Models
{
    public class questionJSONmodel
    {
        public string? Question { get; set; }
        public Boolean? Important { get; set; }
        public int? NumberOfAnswers { get; set; }
        public List<answerForJSONmodel>? Answers { get; set; }
        public int? Subjectid { get; set; }

    }
    public class answerForJSONmodel
    {
        public string? Answer { get; set; }
        public Boolean? Correct { get; set; }
    }
}
