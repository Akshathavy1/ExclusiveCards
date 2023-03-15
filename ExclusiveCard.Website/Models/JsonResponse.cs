namespace ExclusiveCard.Website.Models
{
    public class JsonResponse<T>
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public T Data { get; set; }
        public JsonResponse(bool success, string errorMessage, T data)
        {
            Success = success;
            ErrorMessage = errorMessage;
            Data = data;
        }
        public static JsonResponse<T> SuccessResponse(T data)
        {
            return new JsonResponse<T>(true, null, data);
        }
        public static JsonResponse<T> ErrorResponse(string message)
        {
            return new JsonResponse<T>(false, message, default(T));
        }
    }
}