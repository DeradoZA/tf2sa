using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF2SA.Data.Models;

namespace TF2SA.Web.Models
{
	public class PlayerStatPageModel
	{
		public PlayerStatPageModel(
			List<PlayerHighlights> highlights,
			List<AverageMainStats> scoutAverages,
			List<ExplosiveClassStats> soldierAverages,
			List<ExplosiveClassStats> demomanAverages,
			List<MedicPerformanceStats> medicAverages
		)
		{
			Highlights = highlights;
			ScoutAverages = scoutAverages;
			SoldierAverages = soldierAverages;
			DemomanAverages = demomanAverages;
			MedicAverages = medicAverages;
		}

		public List<PlayerHighlights> Highlights { get; set; }
		public List<AverageMainStats> ScoutAverages { get; set; }
		public List<ExplosiveClassStats> SoldierAverages { get; set; }
		public List<ExplosiveClassStats> DemomanAverages { get; set; }
		public List<MedicPerformanceStats> MedicAverages { get; set; }
	}
}
