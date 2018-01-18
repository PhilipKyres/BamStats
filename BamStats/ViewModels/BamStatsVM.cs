using BamStats.Models;

namespace BamStats.ViewModels
{
	public partial class BamStatsVM
	{
		public BamName Defender { get; set; }
		public BamName Attacker { get; set; }
		public int DWinsCurrent { get; set; }
		public int AWinsCurrent { get; set; }
		public int DWinRateCurrent { get; set; }
		public int AWinRateCurrent { get; set; }
		public int DWinsInStanceCurrent { get; set; }
		public int AWinsInStanceCurrent { get; set; }
		public int DWinRateStanceCurrent { get; set; }
		public int AWinRateStanceCurrent { get; set; }
		public int DWinsOverall { get; set; }
		public int AWinsOverall { get; set; }
		public int DLossesOverall { get; set; }
		public int ALossesOverall { get; set; }
		public int DWinRateOverall { get; set; }
		public int AWinRateOverall { get; set; }
		public int DWinsInStanceOverall { get; set; }
		public int AWinsInStanceOverall { get; set; }
		public int DLossesInStanceOverall { get; set; }
		public int ALossesInStanceOverall { get; set; }
		public int DWinRateInStanceOverall { get; set; }
		public int AWinRateInStanceOverall { get; set; }
		public int DGamesRecordedOverall { get; set; }
		public int AGamesRecordedOverall { get; set; }
		public int Winner { get; set; }
	}
}