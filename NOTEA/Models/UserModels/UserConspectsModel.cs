namespace NOTEA.Models.UserModels
{
    public class UserConspectsModel
    {
        public int User_Id { get; set; }
        public int Conspect_Id { get; set; }
        public char Access_Type { get; set; }
        public UserConspectsModel(int user_Id, int conspect_Id)
        {
            User_Id = user_Id;
            Conspect_Id = conspect_Id;
            Access_Type = 'a';
        }
        public UserConspectsModel(int user_Id, int conspect_Id, char access_type)
        {
            User_Id = user_Id;
            Conspect_Id = conspect_Id;
            Access_Type = access_type;
        }
    }
}
