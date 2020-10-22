using CandidateProject.EntityModels;
using CandidateProject.ViewModels;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CandidateProject.Controllers
{
    public class CartonController : Controller
    {
        private CartonContext db = new CartonContext();

        // GET: Carton
        public ActionResult Index()
        {
            // Requirement 1 : To show the count of equipments in the Carton on index page.

            var cartons = db.Cartons
                .Include(cd => cd.CartonDetails)
                .Select(c => new CartonViewModel()
                {
                    Id = c.Id,
                    CartonNumber = c.CartonNumber,
                    EquipmentCount = c.CartonDetails.Count
                })
                .ToList();

            return View(cartons);
        }

        // GET: Carton/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var carton = db.Cartons
                .Where(c => c.Id == id)
                .Select(c => new CartonViewModel()
                {
                    Id = c.Id,
                    CartonNumber = c.CartonNumber
                })
                .SingleOrDefault();
            if (carton == null)
            {
                return HttpNotFound();
            }
            return View(carton);
        }

        // GET: Carton/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Carton/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CartonNumber")] Carton carton)
        {
            if (ModelState.IsValid)
            {
                db.Cartons.Add(carton);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(carton);
        }

        // GET: Carton/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var carton = db.Cartons
                .Where(c => c.Id == id)
                .Select(c => new CartonViewModel()
                {
                    Id = c.Id,
                    CartonNumber = c.CartonNumber
                })
                .SingleOrDefault();
            if (carton == null)
            {
                return HttpNotFound();
            }
            return View(carton);
        }

        // POST: Carton/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CartonNumber")] CartonViewModel cartonViewModel)
        {
            if (ModelState.IsValid)
            {
                var carton = db.Cartons.Find(cartonViewModel.Id);
                carton.CartonNumber = cartonViewModel.CartonNumber;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cartonViewModel);
        }

        // GET: Carton/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carton carton = db.Cartons.Find(id);
            if (carton == null)
            {
                return HttpNotFound();
            }
            return View(carton);
        }

        // POST: Carton/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var cartonnew = db.Cartons
               .Where(c => c.Id == id)
               .Select(c => new CartonDetailsViewModel()
               {
                   CartonNumber = c.CartonNumber,
                   CartonId = c.Id,
                   Equipment = c.CartonDetails
                       .Select(cd => new EquipmentViewModel()
                       {
                           Id = cd.EquipmentId,
                           ModelType = cd.Equipment.ModelType.TypeName,
                           SerialNumber = cd.Equipment.SerialNumber
                       })
               })
               .SingleOrDefault();

            if (((System.Collections.Generic.List<CandidateProject.ViewModels.EquipmentViewModel>)cartonnew.Equipment).Count == 0)
            {
                Carton carton = db.Cartons.Find(id);
                db.Cartons.Remove(carton);
                db.SaveChanges();
               
            }
            else
            {

                ViewBag.Message = "Sorry,Carton cannot be deleted since it has equipment in it";
            }
            
            return RedirectToAction("Index");
        }

        //Requirement 2: To delete all the items at a time.

        public ActionResult DropItems(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carton carton = db.Cartons.Find(id);
            if (carton == null)
            {
                return HttpNotFound();
            }
            return View(carton);
        }

        // POST: Carton/Delete/5
        [HttpPost, ActionName("DropItems")]
        [ValidateAntiForgeryToken]
        public ActionResult DropItemsConfirmed(int id)
        {

          
            var cartonnew = db.Cartons
               .Where(c => c.Id == id)
               .Select(c => new CartonDetailsViewModel()
               {
                   CartonNumber = c.CartonNumber,
                   CartonId = c.Id,
                   Equipment = c.CartonDetails
                       .Select(cd => new EquipmentViewModel()
                       {
                           Id = cd.EquipmentId,
                           ModelType = cd.Equipment.ModelType.TypeName,
                           SerialNumber = cd.Equipment.SerialNumber
                       })
               })
               .SingleOrDefault();

            //Customer Requirement 1: To delete only cartons which are empty.

            if (((System.Collections.Generic.List<CandidateProject.ViewModels.EquipmentViewModel>)cartonnew.Equipment).Count >0)
            {
                Carton carton = db.Cartons.Find(id);
                var cartonDetails = db.CartonDetails.Where(cd => cd.CartonId == id);
                db.CartonDetails.RemoveRange(cartonDetails);
                db.SaveChanges();
            }
            
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

        public ActionResult AddEquipment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var carton = db.Cartons
                .Where(c => c.Id == id)
                .Select(c => new CartonDetailsViewModel()
                {
                    CartonNumber = c.CartonNumber,
                    CartonId = c.Id
                })
                .SingleOrDefault();

            if (carton == null)
            {
                return HttpNotFound();
            }

           //CustomerRequirement 2 :The below code part allows us to add one item for 1 carton alone.

            var equipment = db.Equipments
                .Where(e => !db.CartonDetails.Select(cd => cd.EquipmentId).Contains(e.Id) )
                .Select(e => new EquipmentViewModel()
                {
                    Id = e.Id,
                    ModelType = e.ModelType.TypeName,
                    SerialNumber = e.SerialNumber
                })
                .ToList();
            
            carton.Equipment = equipment;
            return View(carton);
        }

        public ActionResult AddEquipmentToCarton([Bind(Include = "CartonId,EquipmentId")] AddEquipmentViewModel addEquipmentViewModel)
        {
            if (ModelState.IsValid)
            {
                
                var carton = db.Cartons
                    .Include(c => c.CartonDetails)
                    .Where(c => c.Id == addEquipmentViewModel.CartonId)
                    .SingleOrDefault();
                if (carton == null)
                {
                    return HttpNotFound();
                }
                var equipment = db.Equipments
                    .Where(e => e.Id == addEquipmentViewModel.EquipmentId)
                    .SingleOrDefault();
                if (equipment == null)
                {
                    return HttpNotFound();
                }
                var detail = new CartonDetail()
                {
                    Carton = carton,
                    Equipment = equipment
                };

              //Customer Requirement 3 : A cartonm will be limited to only 10 items, by using the below line of code.

                if (((System.Collections.Generic.HashSet<CandidateProject.EntityModels.CartonDetail>)carton.CartonDetails).Count < 10)
                {
                    carton.CartonDetails.Add(detail);
                    db.SaveChanges();
                }
                else
                {

                    ViewBag.Message = "Sorry,Carton exceed 10 equipments";
                }

             

            }
            return RedirectToAction("AddEquipment", new { id = addEquipmentViewModel.CartonId });
        }

        public ActionResult ViewCartonEquipment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var carton = db.Cartons
                .Where(c => c.Id == id)
                .Select(c => new CartonDetailsViewModel()
                {
                    CartonNumber = c.CartonNumber,
                    CartonId = c.Id,
                    Equipment = c.CartonDetails
                        .Select(cd => new EquipmentViewModel()
                        {
                            Id = cd.EquipmentId,
                            ModelType = cd.Equipment.ModelType.TypeName,
                            SerialNumber = cd.Equipment.SerialNumber
                        })
                })
                .SingleOrDefault();
            if (carton == null)
            {
                return HttpNotFound();
            }

            return View(carton);
        }

        //Last remaining Item for the iteration to removeEquipment On Carton.

        public ActionResult RemoveEquipmentOnCarton([Bind(Include = "CartonId,EquipmentId")] RemoveEquipmentViewModel removeEquipmentViewModel)
        {
            var carton = db.Cartons
           .Include(c => c.CartonDetails)
           .Where(c => c.Id == removeEquipmentViewModel.CartonId)
           .SingleOrDefault();
                if (carton == null)
                {
                    return HttpNotFound();
                }
                var equipment = db.Equipments
                    .Where(e => e.Id == removeEquipmentViewModel.EquipmentId)
                    .SingleOrDefault();
                if (equipment == null)
                {
                    return HttpNotFound();
                }

                var cartondetailID = db.CartonDetails
                    .Where(cd => cd.EquipmentId == equipment.Id)
                    .SingleOrDefault();

                var detail = new CartonDetail()
                {
                    Carton = carton,
                    Equipment = equipment
                };

                CartonDetail cartondetail = db.CartonDetails.Find(cartondetailID.Id);
                db.CartonDetails.Remove(cartondetail);

                db.SaveChanges();
                return RedirectToAction("ViewCartonEquipment", new { id = removeEquipmentViewModel.CartonId });

        }
    }
}



