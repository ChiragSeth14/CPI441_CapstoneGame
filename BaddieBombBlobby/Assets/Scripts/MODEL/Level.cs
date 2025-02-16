public class Level
{
    public int LevelId { get; set; }
    public int LevelNumber { get; set; }

   
    public string Description { get; set; }

    // Constructor
    public Level(int levelId, int levelNumber, string description = "")
    {
        LevelId = levelId;
        LevelNumber = levelNumber;
        Description = description;
    }
}
