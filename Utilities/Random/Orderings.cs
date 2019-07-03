namespace Utilities.Random
{
	public class Orderings
	{
		public static int[] generateRandomOrder(int num_tuples)
		{
			int[] random_order = new int[num_tuples];
			for (int i = 0; i < num_tuples; ++i)
			{
				random_order[i] = i;
			}

			RandomAugmented random = RandomAugmented.getSeededRandomAugmented();
			for (int i = 0; i < num_tuples; ++i)
			{
				int r = random.NextInt(num_tuples-1);
				int t = random_order[i];
				random_order[i] = random_order[r];
				random_order[r] = t;
			}

			return random_order;
		}
	}
}
