using System;
using System.Collections.Generic;
using System.Linq;

namespace MPSC.PlenoSQL.AppWin.View.DataSource
{
	public class Ramo
	{
		public const String cConexoes = "Conexões";

		protected Ramo Pai;
		protected readonly Int64 Id;
		protected readonly List<Ramo> _ramos;
		public readonly String Descricao;
		public IEnumerable<Ramo> Ramos { get { return _ramos; } }

		public Ramo(String descricao) : this(descricao, new List<Ramo>()) { }
		private Ramo(String descricao, IEnumerable<Ramo> ramos) : this(Gerador.NewId, null, descricao, ramos) { }
		private Ramo(Int64 id, Ramo pai, String descricao, IEnumerable<Ramo> ramos)
		{
			Id = id;
			Pai = pai;
			Descricao = descricao;
			_ramos = ramos.ToList();
		}

		public virtual TRamo Adicionar<TRamo>(TRamo ramo) where TRamo : Ramo
		{
			ramo.Pai = this;
			_ramos.Add(ramo);
			return ramo;
		}

		public void RemoverTodos()
		{
			_ramos.RemoveAll(r => true);
		}

		public Ramo Filtrar(String filtro)
		{
			Ramo ramo = this;
			if (!String.IsNullOrWhiteSpace(filtro))
			{
				filtro = filtro.ToUpper();
				var ramos = getFolhasDoRamo(ramo).Where(r => r.ToString().Contains(filtro));
				ramo = reconstituir(ramos.ToList()) ?? new Ramo("Nenhuma informação encontrada");
			}
			return ramo;
		}

		public Ramo Clone(Boolean removeChilds)
		{
			return new Ramo(Id, Pai, Descricao, removeChilds ? new List<Ramo>() : _ramos);
		}

		public override String ToString()
		{
			return (Pai != null) ? Pai.ToString() + "/" + Descricao.ToUpper() : Descricao;
		}

		private static Ramo reconstituir(IEnumerable<Ramo> ramos)
		{
			var ramosPai = agrupar(ramos);
			if (ramosPai.Count > 0)
				return reconstituir(ramosPai);
			else
				return ramos.FirstOrDefault();
		}

		private static List<Ramo> agrupar(IEnumerable<Ramo> ramos)
		{
			var ramosPai = new List<Ramo>();
			foreach (var ramoFolha in ramos)
			{
				if (ramoFolha.Pai != null)
				{
					var ramoPai = ramosPai.FirstOrDefault(r => r.Id == ramoFolha.Pai.Id);
					if (ramoPai == null)
					{
						ramoPai = ramoFolha.Pai.Clone(true);
						ramosPai.Add(ramoPai);
					}
					ramoPai.Adicionar(ramoFolha.Clone(false));
				}
			}
			return ramosPai;
		}

		private static IEnumerable<Ramo> getFolhasDoRamo(Ramo ramo)
		{
			return ramo._ramos.Count > 0 ? ramo._ramos.SelectMany(r => getFolhasDoRamo(r)) : ramo.Enumere();
		}
	}

	public static class Gerador
	{
		private static Int64 _id = 0;
		public static Int64 NewId { get { return ++_id; } }

		public static IEnumerable<Ramo> Enumere(this Ramo ramo) { yield return ramo; }
	}

}