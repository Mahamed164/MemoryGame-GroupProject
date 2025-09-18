namespace SUP.ViewModels;

public class PlayerInformation 
{
    
    public required string  Name { get; set; }

    public int CorrectGuesses { get; set; }

    public int Guesses { get; set; }

    public int Accuracy => (int)((CorrectGuesses / (double)Guesses) * 100);
   
}