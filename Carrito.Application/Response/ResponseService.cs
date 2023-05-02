namespace Carrito.Application.Response
{
    public class ResponseService<T>
    {
        public ResponseService() { Status = 200; Exito = true; }
        public T? Object { get; set; }
        public string? Error { get; set; }
        public int Status { get; set; }
        public bool Exito { get; set; }

        public void AddBadRequest(string message)
        {
            Status = 400;
            Error = message;
            Exito = false;
        }

        public void AddInternalServerError(string message)
        {
            Status = 500;
            Error = message;
            Exito = false;
        }

        public void AddNotFound(string message)
        {
            Status = 404;
            Error = message;
            Exito = false;
        }

    }
}
