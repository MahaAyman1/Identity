namespace cmspro1.Models.ViewModel
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel(){
            Users = new List<string>();
            }
        public string RoleID { get; set; }    
        public string RoleName { get; set; }
        public List <string> Users{ get; set; }
    }
}
