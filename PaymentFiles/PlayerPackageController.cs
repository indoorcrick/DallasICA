using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DallasICA;
using DallasICA.Context;
using Microsoft.AspNet.Identity;

namespace DallasICA.Controllers
{
    public class PlayerPackageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /PlayerPackage/
        public async Task<ActionResult> Index()
        {

            //Get specific player package based on userid
            var userId = User.Identity.GetUserId();
            var playerList = db.PLAYERs.Where(x => x.UserId == userId);

            IQueryable<PLAYER_PACKAGE> player_package = null;
            if(playerList.Any())
            {
                 player_package =  db.PLAYER_PACKAGE.Where(p => p.PLAYER.UserId == userId);
            }
             else
            {
                player_package =  (db.PLAYER_PACKAGE);
            }
          //return View(await  ((playerList.Any()) ? (db.PLAYER_PACKAGE.Include(p => p.PLAYER.UserId == userId)).ToListAsync()
          //      : (db.PLAYER_PACKAGE).ToListAsync()));
         
            return View(await player_package.ToListAsync());
        }

        // GET: /PlayerPackage/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER_PACKAGE player_package = await db.PLAYER_PACKAGE.FindAsync(id);
            if (player_package == null)
            {
                return HttpNotFound();
            }
            return View(player_package);
        }

        // GET: /PlayerPackage/Create
        public ActionResult Create()
        {
            ViewBag.PLAYER_ID = new SelectList(db.PLAYERs, "PLAYER_ID", "FIRST_NAME");
            return View();
        }

        // POST: /PlayerPackage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="ID,PLAYER_ID,PACKAGE_ID,START_DATE,END_DATE,COMMENTS,ACTIVE,SSMA_TimeStamp")] PLAYER_PACKAGE player_package)
        {
            if (ModelState.IsValid)
            {
                db.PLAYER_PACKAGE.Add(player_package);
                await db.SaveChangesAsync();
               // return RedirectToAction("Index");
               
                return RedirectToAction("Index");
            }

           // ViewBag.PLAYER_ID = new SelectList(db.PLAYERs, "PLAYER_ID", "FIRST_NAME", player_package.PLAYER_ID);
           
            return View(player_package);
        }

        // GET: /PlayerPackage/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER_PACKAGE player_package = await db.PLAYER_PACKAGE.FindAsync(id);
            if (player_package == null)
            {
                return HttpNotFound();
            }
           // ViewBag.PLAYER_ID = new SelectList(db.PLAYERs, "PLAYER_ID", "FIRST_NAME", player_package.PLAYER_ID);
            //ViewBag.PACKAGE_ID = new SelectList(db.PACKAGEs, "PACKAGE_ID", "PACKAGE_NAME", player_package.PACKAGE_ID);
            ViewBag.PACKAGE_ID = db.PACKAGEs.Select(p => new SelectListItem()
            {
                Text = p.PACKAGE_NAME,
                Value = p.PACKAGE_ID.ToString()
            });
           
            return View(player_package);
        }

        // POST: /PlayerPackage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="ID,PLAYER_ID,PACKAGE_ID,START_DATE,END_DATE,COMMENTS,ACTIVE,SSMA_TimeStamp")] PLAYER_PACKAGE player_package)
        {
            if (ModelState.IsValid)
            {
                PLAYER_PACKAGE existing = await db.PLAYER_PACKAGE.FindAsync(player_package.ID);
                existing.PACKAGE_ID = player_package.PACKAGE_ID;
                existing.START_DATE = player_package.START_DATE;
                existing.END_DATE = player_package.END_DATE;
                existing.COMMENTS = player_package.COMMENTS;
                
                await db.SaveChangesAsync();
               // return RedirectToAction("Index");
                //-- Getting Package Information
                var pack = db.PACKAGEs.Where(p => p.PACKAGE_ID == player_package.PACKAGE_ID).First();
                //-- Redirect to Payment Page
                return RedirectToAction("PaymentMisc", "Payment", new { packageName = pack.PACKAGE_NAME, price = pack.PACKAGE_PRICE, packageDesc = pack.PACKAGE_DESC });
            }
           // ViewBag.PLAYER_ID = new SelectList(db.PLAYERs, "PLAYER_ID", "FIRST_NAME", player_package.PLAYER_ID);
            ViewBag.PACKAGE_ID = new SelectList(db.PACKAGEs, "PACKAGE_ID", "PACKAGE_NAME", player_package.PACKAGE_ID);
           return View(player_package);
           
        }

        // GET: /PlayerPackage/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER_PACKAGE player_package = await db.PLAYER_PACKAGE.FindAsync(id);
            if (player_package == null)
            {
                return HttpNotFound();
            }
            return View(player_package);
        }

        // POST: /PlayerPackage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PLAYER_PACKAGE player_package = await db.PLAYER_PACKAGE.FindAsync(id);
            db.PLAYER_PACKAGE.Remove(player_package);
            await db.SaveChangesAsync();
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
