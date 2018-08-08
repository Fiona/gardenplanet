using System.Collections.Generic;

namespace GardenPlanet
{
    public class GlobalConfig
    {
        public class Crop
        {
            public List<string> seasons;
            public float[] growthPerDay;
            public int numGrowthStages;
        }

        public class AppearenceConfig
        {
            public class AppearenceObject
            {
                public string name;
                public bool unlockedAtStart;
            }

            public Dictionary<string, AppearenceObject> hair;
            public Dictionary<string, AppearenceObject> tops;
            public Dictionary<string, AppearenceObject> eyes;
            public Dictionary<string, AppearenceObject> eyebrows;
            public Dictionary<string, AppearenceObject> noses;
            public Dictionary<string, AppearenceObject> mouths;
            public Dictionary<string, AppearenceObject> faceDetail;
            public List<List<int>> skinColours;
            public List<List<int>> hairColours;
            public List<List<int>> eyeColours;
        }

        public Dictionary<string, float> energyUsage;

        public Dictionary<string, Crop> crops;

        public AppearenceConfig appearence;
    }
}