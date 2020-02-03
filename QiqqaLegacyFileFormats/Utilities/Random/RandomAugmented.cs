using System;

namespace QiqqaLegacyFileFormats          // namespace Utilities.Random
{
    /// <summary>
    /// Summary description for RandomAugmented.
    /// </summary>
    public class RandomAugmented
    {
        private System.Random rand;

        public RandomAugmented()
        {
            rand = new System.Random();
        }

        public RandomAugmented(int ticks)
        {
            rand = new System.Random(Math.Max(1, ticks));
        }

        // ---------------------------------------------------------------

        public static RandomAugmented Instance = new RandomAugmented((int)DateTime.Now.Ticks);

    }
}
