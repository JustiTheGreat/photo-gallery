namespace PhotoGallery.Common
{
    public class PGConstants
    {
        public static readonly string AuthorizationHeaderParsingErrorMessage = "Problem occured while parsing authorization header";

        public static readonly string JWT = "JWT";
        public static readonly string CORS = "CORS";
        public static readonly string Firebase = "Firebase";
        public static readonly string UIdClaimName = "uid";
        public static readonly string GoogleApplicationCredentialsEnvironementVariable = "GOOGLE_APPLICATION_CREDENTIALS";

        public static readonly string PostCollection = "posts";
        public static readonly string PostCollectionDescriptionField = "Description";
        public static readonly string PostCollectionStoragePathField = "StoragePath";
        public static readonly string PostCollectionUIdField = "UId";

        public static readonly string UserCollection = "users";
        public static readonly string UserCollectionEmailField = "Email";
        public static readonly string UserCollectionPasswordField = "Password";
    }
}
