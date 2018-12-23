using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Menu> menus = context.Menus.ToList();
            return View(menus);
        }

        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View(addMenuViewModel);

        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu
                {
                    Name = addMenuViewModel.Name
                };

                context.Menus.Add(newMenu);
                context.SaveChanges();

                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }
            return View("/Menu/Add");
        }

        public IActionResult ViewMenu(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);

            List<CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel
            {
                Items = items,
                Menu = menu
            };
            return View(viewMenuViewModel);
        }

        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.SingleOrDefault(m => m.ID == id);
            List<Cheese> Cheeses = context.Cheeses.ToList();
            return View(new AddMenuItemViewModel(menu, Cheeses));

        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if (ModelState.IsValid)
            {
                var cheeseID = addMenuItemViewModel.CheeseID;
                var menuID = addMenuItemViewModel.MenuID;

                IList<CheeseMenu> currentCheeses = context
                 .CheeseMenus
                .Where(cm => cm.CheeseID == cheeseID)
                .Where(cm => cm.MenuID == menuID)
                .ToList();


                if(currentCheeses.Count == 0)
                {
                    CheeseMenu menuItem = new CheeseMenu
                    {
                        CheeseID = addMenuItemViewModel.CheeseID,
                        MenuID = addMenuItemViewModel.MenuID
                    };
                    context.CheeseMenus.Add(menuItem);
                    context.SaveChanges();


                    
                }
                return Redirect("/Menu/ViewMenu/" + addMenuItemViewModel.MenuID);



            }
            return View(addMenuItemViewModel);

        }
    }
}
    
    

