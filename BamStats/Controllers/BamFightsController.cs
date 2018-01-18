using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BamStats.Models;
using BamStats.ViewModels;
using RestaurantReview.Validators;

namespace BamStats.Controllers
{

/* TODO
 * Re-design Stats page: 
 *	colors blue and red
 *	more descriptive / tags
 *	split down middle
 *	Importance top to bottom
 *	Total Win/Loss    4/8 (33%)
 *	Make overflows apear as NA
 *	
 * Remove Old fights link in hotbar (after old fights added to DB)
 * 
 */
    public class BamFightsController : Controller
    {
        private Entities db = new Entities();

        // GET: BamFights
		[Authorize]
        public ActionResult Index()
        {
            List<BamFight> bamFights = db.BamFights.Include(b => b.BamName).Include(b => b.BamName1).OrderByDescending(b => b.FightDate).ThenByDescending(b => b.Id).ToList();
			ViewBag.Total = db.BamFights.Count();
			int d = db.BamFights.Count(b => b.Stance == false);
			int a = db.BamFights.Count(b => b.Stance == true);
			int tot = d + a;
			ViewBag.D = (int)Math.Round(((double)d / tot) * 100);
			ViewBag.A = (int)Math.Round(((double)a / tot) * 100);
            return View(bamFights);
        }

		[Authorize]
		public ActionResult GetInfo()
		{
			ViewBag.Attacker = new SelectList(db.BamNames, "Id", "Name");
			ViewBag.Defender = new SelectList(db.BamNames, "Id", "Name");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public ActionResult GetInfo(BamInfoVM vm)
		{
			if (ModelState.IsValid)
			{
				if (vm.Defender != vm.Attacker)
				{
					return RedirectToAction("Stats", new { def = vm.Defender, att = vm.Attacker });
				}
				ModelState.AddModelError("", "Defender and Attacker cannot be the same!");
				ViewBag.Attacker = new SelectList(db.BamNames, "Id", "Name");
				ViewBag.Defender = new SelectList(db.BamNames, "Id", "Name");
			}
			return View();
		}

		[Authorize]
		public ActionResult Stats(int def, int att)
		{
			/*
			 * Defence Wins vs Att %
			 * Attack Wins vs Def %
			 * Defence Wins vs Att in D stance %
			 * Attack Wins vs Def in A stance %
			 * 
			 * Defence Total Win/Loss %
			 * Attack Total Win/Loss %
			 * Defence Total Win/Loss in D stance %
			 * Attack Total Win/Loss in A stance %
			 * 
			 * Defence Total Games Recorded
			 * Attack Total Games Recorded
			 */

			BamName defender =  (from bam in db.BamNames where bam.Id == def select bam).FirstOrDefault();
			BamName attacker = (from bam in db.BamNames where bam.Id == att select bam).FirstOrDefault();

			int dWinsCurrent = (from fight in db.BamFights where fight.Winner == def && fight.Loser == att select fight).Count();
			int aWinsCurrent = (from fight in db.BamFights where fight.Winner == att && fight.Loser == def select fight).Count();

			int gamesTogether = dWinsCurrent + aWinsCurrent;

			int dWinRateCurrent = (int)Math.Round(((double)dWinsCurrent / gamesTogether) * 100);
			int aWinRateCurrent = (int)Math.Round(((double)aWinsCurrent / gamesTogether) * 100);

			int dWinsInStanceCurrent = (from fight in db.BamFights where fight.Winner == def && fight.Loser == att && fight.Stance == false select fight).Count();
			int aWinsInStanceCurrent = (from fight in db.BamFights where fight.Winner == att && fight.Loser == def && fight.Stance == true select fight).Count();

			int gamesTogetherInStance = dWinsInStanceCurrent + aWinsInStanceCurrent;

			int dWinRateStanceCurrent = 0;
			int aWinRateStanceCurrent = 0;

			if (gamesTogetherInStance > 0)
			{
				dWinRateStanceCurrent = (int)Math.Round(((double)dWinsInStanceCurrent / gamesTogetherInStance) * 100);
				aWinRateStanceCurrent = (int)Math.Round(((double)aWinsInStanceCurrent / gamesTogetherInStance) * 100);
			}

			int dWinsOverall = (from fight in db.BamFights where fight.Winner == def select fight).Count();
			int aWinsOverall = (from fight in db.BamFights where fight.Winner == att select fight).Count();

			int dLossesOverall = (from fight in db.BamFights where fight.Loser == def select fight).Count();
			int aLossesOverall = (from fight in db.BamFights where fight.Loser == att select fight).Count();

			int dGameCountOverall = dWinsOverall + dLossesOverall;
			int aGameCountOverall = aWinsOverall + aLossesOverall;

			int dWinRateOverall = (int)Math.Round(((double)dWinsOverall / dGameCountOverall) * 100);
			int aWinRateOverall = (int)Math.Round(((double)aWinsOverall / aGameCountOverall) * 100);

			int dWinsInStanceOverall = (from fight in db.BamFights where fight.Winner == def && fight.Stance == false select fight).Count();
			int aWinsInStanceOverall = (from fight in db.BamFights where fight.Winner == att && fight.Stance == true select fight).Count();

			int dLossesInStanceOverall = (from fight in db.BamFights where fight.Loser == def && fight.Stance == false select fight).Count();
			int aLossesInStanceOverall = (from fight in db.BamFights where fight.Loser == att && fight.Stance == true select fight).Count();

			int dWinRateInStanceOverall = (int)Math.Round(((double)dWinsInStanceOverall / (dWinsInStanceOverall + dLossesInStanceOverall)) * 100);
			int aWinRateInStanceOverall = (int)Math.Round(((double)aWinsInStanceOverall / (aWinsInStanceOverall + aLossesInStanceOverall)) * 100);

			int dGamesRecordedOverall = (from fight in db.BamFights where fight.Winner == def || fight.Loser == def select fight).Count();
			int aGamesRecordedOverall = (from fight in db.BamFights where fight.Winner == att || fight.Loser == att select fight).Count();

			BamStatsVM vm = new BamStatsVM
			{
				Defender = defender,
				Attacker = attacker,
				DWinsCurrent = dWinsCurrent,
				AWinsCurrent = aWinsCurrent,
				DWinRateCurrent = dWinRateCurrent,
				AWinRateCurrent = aWinRateCurrent,
				DWinsInStanceCurrent = dWinsInStanceCurrent,
				AWinsInStanceCurrent = aWinsInStanceCurrent,
				DWinRateStanceCurrent = dWinRateStanceCurrent,
				AWinRateStanceCurrent = aWinRateStanceCurrent,
				DWinsOverall = dWinsOverall,
				AWinsOverall = aWinsOverall,
				DLossesOverall = dLossesOverall,
				ALossesOverall = aLossesOverall,
				DWinRateOverall = dWinRateOverall,
				AWinRateOverall = aWinRateOverall,
				DWinsInStanceOverall = dWinsInStanceOverall,
				AWinsInStanceOverall = aWinsInStanceOverall,
				DLossesInStanceOverall = dLossesInStanceOverall,
				ALossesInStanceOverall = aLossesInStanceOverall,
				DWinRateInStanceOverall = dWinRateInStanceOverall,
				AWinRateInStanceOverall = aWinRateInStanceOverall,
				DGamesRecordedOverall = dGamesRecordedOverall,
				AGamesRecordedOverall = aGamesRecordedOverall
			};

			TempData["Info"] = vm;
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		[AdminAuthorize]
		public ActionResult Stats(BamStatsVM vm)
		{
			if (ModelState.IsValid)
			{
				BamStatsVM stats = TempData["Info"] as BamStatsVM;

				int winner;
				int loser;
				bool stance;
				if (vm.Winner == 0)
				{
					winner = stats.Defender.Id;
					loser = stats.Attacker.Id;
					stance = false;
				}
				else
				{
					winner = stats.Attacker.Id;
					loser = stats.Defender.Id;
					stance = true;
				}

				BamFight fight = new BamFight()
				{
					Winner = winner,
					Loser = loser,
					Stance = stance,
					FightDate = DateTime.Now.AddHours(-5)
				};

				db.BamFights.Add(fight);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(vm);
		}


		// GET: BamFights/Create
		[Authorize]
		[AdminAuthorize]
		public ActionResult Create()
		{
			ViewBag.Loser = new SelectList(db.BamNames, "Id", "Name");
			ViewBag.Winner = new SelectList(db.BamNames, "Id", "Name");
			BamFight fight = new BamFight
			{
				FightDate = DateTime.Now
			};
			return View(fight);
		}

        // POST: BamFights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize]
		[AdminAuthorize]
        public ActionResult Create([Bind(Include = "Winner,Loser,Stance,FightDate")] BamFight bamFight, int count)
        {
            if (ModelState.IsValid)
            {

				if (bamFight.Winner != bamFight.Loser)
				{
					if (count >= 1 && count <= 10)
					{
						for (int i = 0; i < count; i++)
						{
							db.BamFights.Add(bamFight);
							db.SaveChanges();
						}
						return RedirectToAction("Create");
					}
					else
					{
						ModelState.AddModelError("", "Invalid Count");
					}
				}
				else
				{
					ModelState.AddModelError("", "Winner and Loser cannot be the same!");
				}
            }

            ViewBag.Loser = new SelectList(db.BamNames, "Id", "Name", bamFight.Loser);
            ViewBag.Winner = new SelectList(db.BamNames, "Id", "Name", bamFight.Winner);
            return View(bamFight);
        }

        // GET: BamFights/Edit/5
		[Authorize]
		[AdminAuthorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BamFight bamFight = db.BamFights.Find(id);
            if (bamFight == null)
            {
                return HttpNotFound();
            }
            ViewBag.Loser = new SelectList(db.BamNames, "Id", "Name", bamFight.Loser);
            ViewBag.Winner = new SelectList(db.BamNames, "Id", "Name", bamFight.Winner);
            return View(bamFight);
        }

        // POST: BamFights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize]
		[AdminAuthorize]
        public ActionResult Edit([Bind(Include = "Id,Winner,Loser,Stance,FightDate")] BamFight bamFight)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bamFight).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Loser = new SelectList(db.BamNames, "Id", "Name", bamFight.Loser);
            ViewBag.Winner = new SelectList(db.BamNames, "Id", "Name", bamFight.Winner);
            return View(bamFight);
        }

        // GET: BamFights/Delete/5
		[Authorize]
		[AdminAuthorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BamFight bamFight = db.BamFights.Find(id);
            if (bamFight == null)
            {
                return HttpNotFound();
            }
            return View(bamFight);
        }

        // POST: BamFights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize]
		[AdminAuthorize]
        public ActionResult DeleteConfirmed(int id)
        {
            BamFight bamFight = db.BamFights.Find(id);
            db.BamFights.Remove(bamFight);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
