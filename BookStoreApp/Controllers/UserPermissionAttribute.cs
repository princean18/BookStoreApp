using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using BookStoreApp.Models;
using Newtonsoft.Json;

namespace BookStoreApp.Helper
{
    public sealed class UserPermissionAttribute : Attribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context != null)
            {
                var descriptor = context?.ActionDescriptor as ControllerActionDescriptor;
                var ctrorlerlName = descriptor.ControllerName;
                var actionName = descriptor.ActionName.ToLower();

                if (actionName != "login")
                {
                    var session = context.HttpContext.Session.GetString("emailId");


                    if (session != null)
                    {
                        //List<Menu> menuList = new List<Menu>();
                        //List<Menu> menuList2 = new List<Menu>();
                        //menuList = JsonConvert.DeserializeObject<List<Menu>>(context.HttpContext.Session.GetString("MenuList"));
                        //menuList2 = SessionManager.GetObjectFromSession<List<Menu>>(context.HttpContext.Session, "MenuList2");
                        //if (menuList2.Count > 0)
                        //{
                        //    foreach (var menu in menuList2)
                        //    {
                        //        if (menu.ActionName.ToLower() == actionName)
                        //        {
                        //            return;
                        //        }
                        //        else
                        //        {
                        //            context.Result = new RedirectResult("/Home/Login");
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    context.Result = new RedirectResult("/Home/Login");
                        //}
                    }
                    else
                    {
                        context.Result = new RedirectResult("/Login");
                    }
                }
            }
        }
    }
}