namespace BE.Models.Entities;

public class WorkoutSession
{
    public string DayOfWeek { get; set; }
    public string Focus { get; set; }

    public List<ExerciseInSession> Exercises { get; set; }
}
