using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
using RummikubApp.Models;
namespace RummikubApp.ModelLogics
{
    public partial class FbData : FbDataModel
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
        public override async Task SendPasswordResetEmailAsync(string email, Func<Task, Task> OnCompleteSendEmail)
        {
            // Start Firebase sign-in
            Task firebaseTask = facl.ResetEmailPasswordAsync(email);
            try
            {
                // Await Firebase sign-in
                await firebaseTask;
            }
            catch (Exception ex)
            {
                // Wrap the exception in a Task to pass to the callback
                TaskCompletionSource<Firebase.Auth.UserCredential> tcs = new();
                tcs.SetException(ex);
                firebaseTask = tcs.Task;
            }
            finally
            {
                // Always invoke the callback, even if the sign-in failed
                await OnCompleteSendEmail(firebaseTask);
            }
        }
        public override string SetDocument(object obj, string collectonName, string id, Action<System.Threading.Tasks.Task> OnComplete)
        {
            IDocumentReference dr = string.IsNullOrEmpty(id) ? fs.Collection(collectonName).Document() : fs.Collection(collectonName).Document(id);
            dr.SetAsync(obj).ContinueWith(OnComplete);
            return dr.Id;
        }
        public override IListenerRegistration AddSnapshotListener(string collectonName, Plugin.CloudFirestore.QuerySnapshotHandler OnChange)
        {
            ICollectionReference cr = fs.Collection(collectonName);
            return cr.AddSnapshotListener(OnChange);
        }
        public override IListenerRegistration AddSnapshotListener(string collectonName, string id, Plugin.CloudFirestore.DocumentSnapshotHandler OnChange)
        {
            IDocumentReference cr = fs.Collection(collectonName).Document(id);
            return cr.AddSnapshotListener(OnChange);
        }
        public async void GetDocumentsWhereEqualTo(string collectonName, string fName, object fValue, Action<IQuerySnapshot> OnComplete)
        {
            ICollectionReference cr = fs.Collection(collectonName);
            IQuerySnapshot qs = await cr.WhereEqualsTo(fName, fValue).GetAsync();
            OnComplete(qs);
        }
        public override async void GetDocumentsWhereLessThan(string collectonName, string fName, object fValue, Action<IQuerySnapshot> OnComplete)
        {
            ICollectionReference cr = fs.Collection(collectonName);
            IQuerySnapshot qs = await cr.WhereLessThan(fName, fValue).GetAsync();
            OnComplete(qs);
        }
        public override async void UpdateFields(string collectonName, string id, Dictionary<string, object> dict, Action<Task> OnComplete)
        {
            IDocumentReference dr = fs.Collection(collectonName).Document(id);
            await dr.UpdateAsync(dict).ContinueWith(OnComplete);
        }
        public override async void DeleteDocument(string collectonName, string id, Action<Task> OnComplete)
        {
            IDocumentReference dr = fs.Collection(collectonName).Document(id);
            await dr.DeleteAsync().ContinueWith(OnComplete);
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
