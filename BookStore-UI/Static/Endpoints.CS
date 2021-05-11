namespace UI.Static
{
    public static class Endpoints
    {
        public static string BaseURL = "https://localhost:44382/"; // Needs To Match API Launch Settings Config
        public static string AuthorsEndpoint = BaseURL + "api/authors/";
        public static string BooksEndpoint = BaseURL + "api/books/";
        public static string RegistrationEndpoint = BaseURL + "api/users/register/";
        public static string LoginEndpoint = BaseURL + "api/users/login/";
    }
}