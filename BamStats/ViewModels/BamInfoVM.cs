using System.ComponentModel.DataAnnotations;

namespace BamStats.ViewModels
{
	public partial class BamInfoVM
	{
		[Required]
		public int Defender { get; set; }

		[Required]
		public int Attacker { get; set; }
	}
}