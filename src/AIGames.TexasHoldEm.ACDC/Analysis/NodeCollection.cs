using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	public class NodeCollection: IEnumerable<Node>
	{
		private Dictionary<SubRoundType, List<Node>> CallNodes = new Dictionary<SubRoundType, List<Node>>();
		private Dictionary<SubRoundType, List<Node>> CallRaiseNodes = new Dictionary<SubRoundType, List<Node>>();
		private Dictionary<SubRoundType, List<Node>> NoCallRaiseNodes = new Dictionary<SubRoundType, List<Node>>();

		public NodeCollection()
		{
			foreach (var sub in SubRoundTypes.All)
			{
				CallNodes[sub] = new List<Node>();
				CallRaiseNodes[sub] = new List<Node>();
				NoCallRaiseNodes[sub] = new List<Node>();
			}
		}

		public NodeCollection(IEnumerable<Node> nodes) : this() { AddRange(nodes); }

		public int Count
		{
			get
			{
				var count = 0;
				foreach (var sub in SubRoundTypes.All)
				{
					count += CallNodes[sub].Count;
					count += CallRaiseNodes[sub].Count;
					count += NoCallRaiseNodes[sub].Count;
				}
				return count;
			}
		}

		public void AddRange(IEnumerable<Node> nodes)
		{
			foreach (var node in nodes)
			{
				Add(node);
			}
		}

		public void Add(Node item)
		{
			if (!SubRoundTypes.All.Contains(item.SubRound)) { return; }
			if (item.Action == GameAction.Call)
			{
				CallNodes[item.SubRound].Add(item);
			}
			else if (item.HasAmountToCall)
			{
				CallRaiseNodes[item.SubRound].Add(item);
			}
			else
			{
				NoCallRaiseNodes[item.SubRound].Add(item);
			}
		}


		public IEnumerable<Node> GetCallNodes(SubRoundType sub)
		{
			return CallNodes[sub];
		}

		public IEnumerable<Node> GetRaiseNodes(SubRoundType sub, bool hasAmountToCall)
		{
			return hasAmountToCall ? CallRaiseNodes[sub] : NoCallRaiseNodes[sub];
		}

		public void Clear()
		{
			foreach (var sub in SubRoundTypes.All)
			{
				CallNodes[sub].Clear();
				CallRaiseNodes[sub].Clear();
				NoCallRaiseNodes[sub].Clear();
			}
		}

		public IEnumerator<Node> GetEnumerator()
		{
			foreach (var sub in SubRoundTypes.All)
			{
				foreach(var node in CallNodes[sub])
				{
					yield return node;
				}
				foreach (var node in CallRaiseNodes[sub])
				{
					yield return node;
				}
				foreach (var node in NoCallRaiseNodes[sub])
				{
					yield return node;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}
}
