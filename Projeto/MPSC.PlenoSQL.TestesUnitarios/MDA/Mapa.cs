using System;
using System.Linq;

namespace MPSC.PlenoSQL.TestesUnitarios.MDA
{
	public class Mapa<TKey, TValue>
	{
		protected readonly KVP<TKey, TValue>[] lista;
		protected Int32 total = 0;
		private Int32 _ultimo = -1;
		protected Mapa(Int32 capacidade) { lista = new KVP<TKey, TValue>[capacidade]; }

		public TValue this[TKey key] { get { return Find(key, total - 1); } }

		private TValue Find(TKey key, Int32 max)
		{
			_ultimo = (_ultimo + 1) % total;
			var kvp = lista[_ultimo];
			return kvp.Key.Equals(key) ? kvp.Value : ((max > 0) ? Find(key, max - 1) : default(TValue));
		}

		protected class KVP<TKey1, TValue1>
			where TKey1 : TKey
			where TValue1 : TValue
		{
			public readonly TKey1 Key;
			public readonly TValue1 Value;
			public KVP(TKey1 key, TValue1 value)
			{
				Key = key;
				Value = value;
			}
		}
	}

	public static class Mapa
	{
		public static MapaBuilder<TKey, TValue> De<TKey, TValue>(Int32 capacidade)
		{
			return new MapaBuilder<TKey, TValue>(capacidade);
		}

		public sealed class MapaBuilder<TKey, TValue> : Mapa<TKey, TValue>
		{
			internal MapaBuilder(Int32 capacidade) : base(capacidade) { }

			public MapaBuilder<TKey, TValue> Add(TKey key, TValue value)
			{
				return Add(new KVP<TKey, TValue>(key, value));
			}

			private MapaBuilder<TKey, TValue> Add(KVP<TKey, TValue> kvp)
			{
				if (total >= lista.Length)
					throw new InvalidOperationException();
				lista[total++] = kvp;
				return this;
			}

			public Mapa<TKey, TValue> Build()
			{
				var itensUnicos = lista.Where(kvp => kvp != null).Select(kvp => kvp.Key).Distinct().Count();
				if (total > itensUnicos)
					throw new InvalidOperationException();
				return this;
			}
		}
	}
}