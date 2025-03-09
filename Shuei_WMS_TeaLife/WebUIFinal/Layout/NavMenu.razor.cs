using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using WebUIFinal;

namespace WebUIFinal.Layout
{
    public partial class NavMenu
    {
        [Parameter]
        public bool sidebarExpanded { get; set; } = true;

        void OnParentClick(MenuItemEventArgs args)
        {
            Console.WriteLine($"{args.Path}|{args.Text}");


            //var r = GlobalVariable.BreadCrumbData.FirstOrDefault(x => x.Path == args.Path && x.Text == args.Text);

            //if (r == null)
            //{
            //    GlobalVariable.BreadCrumbData.Add(new BreadCrumbModel()
            //    {
            //        Text = args.Text,
            //        Path = args.Path
            //    });
            //}
            //else
            //{
            //    GlobalVariable.BreadCrumbData.Add(new BreadCrumbModel()
            //    {
            //        Text = args.Text,
            //        Path = args.Path
            //    });
            //}
        }

        void OnChildClicked(MenuItemEventArgs args)
        {

        }
    }
}
