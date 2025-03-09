using WebUI;

namespace WebUI
{
    public static class GlobalVariable
    {
        public static BreadCumb BreadCrumbData { get; set; } =new BreadCumb();
        public static BreadCumb BreadCrumbDataMaster { get; set; } = new BreadCumb();

        public static UserAuthorizationInfo UserAuthorizationInfo { get; set; }=new UserAuthorizationInfo();
    }
}
