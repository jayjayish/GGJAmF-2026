
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

        public enum Color
        {
            Red = 0,
            Yellow = 60,
            Green = 120,
            Cyan = 180,
            Blue  = 240,
            Purple = 300,
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
