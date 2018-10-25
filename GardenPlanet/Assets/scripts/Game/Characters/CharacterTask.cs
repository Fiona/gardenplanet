namespace GardenPlanet
{
    public enum TaskPriority
    {
        MEH = 1,
        NOT_IMPORTANT = 2,
        NORMAL = 3,
        IMPORTANT = 4,
        VERY_IMPORTANT = 5,
        CRITICAL = 5
    }

    public class CharacterTask
    {
        protected string name = "Unamed";
        protected TaskPriority priority;
        protected Character character;
        protected bool done;
        protected bool started;
        protected CharacterTask dependency;

        public CharacterTask(TaskPriority priority, CharacterTask dependency = null)
        {
            this.priority = priority;
            this.dependency = dependency;
        }

        public void SetCharacter(Character character)
        {
            this.character = character;
        }

        public override string ToString()
        {
            var charString = character == null ? "None" : $"{character.id} - {character.name}";
            return $"AI Task {name}({priority}): {charString}";
        }
    }

    /*
     * Task Type: PathToTile
     * Walks the character to a particular tile in a particular map
     */
    public class CharacterTaskPathToTile: CharacterTask
    {
        private string name = "PathToTile";
        private MapTilePosition pathToPos;

        public CharacterTaskPathToTile(TaskPriority priority, MapTilePosition pathTo, CharacterTask dependency = null):
            base(priority, dependency)
        {
            pathToPos = pathTo;
        }
    }
}