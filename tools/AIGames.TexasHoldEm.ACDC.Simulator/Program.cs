using McCulloch.Networks;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC.Simulator
{
    public class Program
	{
		public static void Main(string[] args)
		{
			var simulator = new Simulator();
			var rnd = new MT19937Generator();

            var network = ResilientBackPropagationTrainer.Train(new ActionOption[0], new NeuralNetworkSettings() { HiddenNodes = 10 });

			long runs = 0;
			while (true)
			{
				runs++;
				var game = simulator.Simulate(network, rnd);
			}
		}
	}
}
