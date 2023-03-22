using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProductIdentityClient.Controllers
{

    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> role;

        public RoleController(RoleManager<IdentityRole> role)
        {
            this.role = role;
        }
        //list all role
        public IActionResult Index()
        {
            var r = role.Roles;
            return View(r);
        }

        // GET: RoleController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RoleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole identityRole)
        {
            //check duplicate
            if (!role.RoleExistsAsync(identityRole.Name).GetAwaiter().GetResult())
            {
                role.CreateAsync(new IdentityRole(identityRole.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }
        // GET: RoleController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }    
    }
}
