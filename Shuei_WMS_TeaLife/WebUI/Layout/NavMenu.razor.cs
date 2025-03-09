using Microsoft.AspNetCore.Components;
using Radzen;

namespace WebUI.Layout
{
    public partial class NavMenu
    {
        [Parameter]
        public bool sidebarExpanded { get; set; } = true;

        void OnParentClick(MenuItemEventArgs args)
        {
            Console.WriteLine($"{args.Path}|{args.Text}");

            //if (string.IsNullOrEmpty(args.Path))
            {
                GlobalVariable.BreadCrumbData = null;
                GlobalVariable.BreadCrumbData = new BreadCumb();
            }


            GlobalVariable.BreadCrumbData.Add(new BreadCrumbModel()
            {
                Text = args.Text,
                Path = args.Path
            });
        }
    }
}
