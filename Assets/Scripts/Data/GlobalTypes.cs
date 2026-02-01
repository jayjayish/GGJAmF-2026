
namespace Data
{
    public static class GlobalTypes
    {
        public enum SceneName : byte
        {
            MainMenu,
            Hub,
            UI,
            AmyTest = 252,
            FredTest = 253,
            Test = 254
        }

        public enum Color : byte
        {
            Red,
            Yellow,
            Green,
            Cyan,
            Blue,
            Purple,
        }

        public enum ProjectileTypes : byte
        {
            None,
            Default,
            PlayerMain,
            TestCircle = 254,
        }
    }
}
