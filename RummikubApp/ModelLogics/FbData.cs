using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
using RummikubApp.Models;

namespace RummikubApp.ModelLogics
{
    internal class FbData : FbDataModel
    {
        public FbData()
        {
            
        }
        public async override void CreateUserWithEmailAndPasswordAsync(string email, string password, string name, Action<System.Threading.Tasks.Task> OnComplete)
        {
            await facl.CreateUserWithEmailAndPasswordAsync(email, password, name).ContinueWith(OnComplete);
        }
        public async override void SignInWithEmailAndPasswordAsync(string email, string password, Action<System.Threading.Tasks.Task> OnComplete)
        {
            await facl.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(OnComplete);
        }
        public override string DisplayName
        {
            get
            {
                string dn = string.Empty;
                if (facl.User != null)
                    dn = facl.User.Info.DisplayName;
                return dn;
            }
        }
        public override string UserId
        {
            get
            {
                return facl.User.Uid;
            }
        }
    }
}
